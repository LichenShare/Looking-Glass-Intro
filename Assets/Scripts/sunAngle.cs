using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class sunAngle : MonoBehaviour
{
    bool shiftPressed = false;
    mouseLook ml;
    bool previousState;
    public float latitude, longitude;
    public int day, month, hour, minute;
    public Text dateText;
    // Start is called before the first frame update
    void Start()
    {
        ml = GameObject.FindObjectOfType<mouseLook>();
        setSunPosition(DateTime.Now);
    }
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            previousState = ml.enabled;
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            ml.enabled = previousState;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))  //if you press shift, you're changing sun angle
        {
            ml.enabled = false;
            float time = map(Input.mousePosition.x, 0, Screen.width, 0, 24);
            float date = map(Input.mousePosition.y, 0, Screen.height, 0, 365);
            //transform.localEulerAngles = new Vector3(axis, 0, 0);
            month = Mathf.Clamp((int)(date / 30)+1,1,12);
            day = Mathf.Clamp((int)date % 30,1,30);
            hour = (int)Mathf.Clamp(time,0,23);
            minute = Mathf.Clamp((int)(60 * (time % 1)), 0, 59);
            DateTime d = new DateTime(2020, month, day, hour, minute, 0);
            setSunPosition(d);
        }        
    }

    void setSunPosition(DateTime d)
    {
        Vector3 angles = new Vector3();
        float alt;
        float azi;
        SunPosition.CalculateSunPosition(d, (float)latitude, (float)longitude, out azi, out alt);
        angles.x = (float)alt * Mathf.Rad2Deg;
        angles.y = (float)azi * Mathf.Rad2Deg;
        //UnityEngine.Debug.Log(angles);
        transform.localRotation = Quaternion.Euler(angles);
        GetComponent<Light>().intensity = Mathf.InverseLerp(-12, 0, angles.x);
        string dateString = d.ToString("MMMM d h:mm tt");
        dateText.text = "Date:  " + dateString;

    }
}
/*  Thanks to Paul Hayes!  https://gist.github.com/paulhayes/54a7aa2ee3cccad4d37bb65977eb19e2
     * The following source came from this blog:
     * http://guideving.blogspot.co.uk/2010/08/sun-position-in-c.html
     */
public static class SunPosition
{
    const float Deg2Rad = (float)(Mathf.PI / 180.0f);
    const float Rad2Deg = (float)(180.0f / Mathf.PI);

    /*! 
     * \brief Calculates the sun light. 
     * 
     * CalcSunPosition calculates the suns "position" based on a 
     * given date and time in local time, latitude and longitude 
     * expressed in decimal degrees. It is based on the method 
     * found here: 
     * http://www.astro.uio.no/~bgranslo/aares/calculate.html 
     * The calculation is only satisfiably correct for dates in 
     * the range March 1 1900 to February 28 2100. 
     * \param dateTime Time and date in local time. 
     * \param latitude Latitude expressed in decimal degrees. 
     * \param longitude Longitude expressed in decimal degrees. 
     */
    public static void CalculateSunPosition(
        DateTime dateTime, float latitude, float longitude, out float outAzimuth, out float outAltitude)
    {

        // Convert to UTC  
        dateTime = dateTime.ToUniversalTime();

        // Number of days from J2000.0.  
        float julianDate = 367 * dateTime.Year -
            (int)((7.0f / 4.0f) * (dateTime.Year +
            (int)((dateTime.Month + 9.0f) / 12.0f))) +
            (int)((275.0 * dateTime.Month) / 9.0f) +
            dateTime.Day - 730531.5f;

        float julianCenturies = julianDate / 36525.0f;

        // Sidereal Time  
        float siderealTimeHours = 6.6974f + 2400.0513f * julianCenturies;

        float siderealTimeUT = siderealTimeHours +
            (366.2422f / 365.2422f) * (float)dateTime.TimeOfDay.TotalHours;

        float siderealTime = siderealTimeUT * 15 + longitude;

        // Refine to number of days (fractional) to specific time.  
        julianDate += (float)dateTime.TimeOfDay.TotalHours / 24.0f;
        julianCenturies = julianDate / 36525.0f;

        // Solar Coordinates  
        float meanLongitude = CorrectAngle(Deg2Rad *
            (280.466f + 36000.77f * julianCenturies));

        float meanAnomaly = CorrectAngle(Deg2Rad *
            (357.529f + 35999.05f * julianCenturies));

        float equationOfCenter = Deg2Rad * ((1.915f - 0.005f * julianCenturies) *
            Mathf.Sin(meanAnomaly) + 0.02f * Mathf.Sin(2 * meanAnomaly));

        float elipticalLongitude =
            CorrectAngle(meanLongitude + equationOfCenter);

        float obliquity = (23.439f - 0.013f * julianCenturies) * Deg2Rad;

        // Right Ascension  
        float rightAscension = Mathf.Atan2(
            Mathf.Cos(obliquity) * Mathf.Sin(elipticalLongitude),
            Mathf.Cos(elipticalLongitude));

        float declination = Mathf.Asin(
            Mathf.Sin(rightAscension) * Mathf.Sin(obliquity));

        // Horizontal Coordinates  
        float hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

        if (hourAngle > Mathf.PI)
        {
            hourAngle -= 2 * Mathf.PI;
        }

        float altitude = Mathf.Asin(Mathf.Sin(latitude * Deg2Rad) *
            Mathf.Sin(declination) + Mathf.Cos(latitude * Deg2Rad) *
            Mathf.Cos(declination) * Mathf.Cos(hourAngle));

        // Nominator and denominator for calculating Azimuth  
        // angle. Needed to test which quadrant the angle is in.  
        float aziNom = -Mathf.Sin(hourAngle);
        float aziDenom =
            Mathf.Tan(declination) * Mathf.Cos(latitude * Deg2Rad) -
            Mathf.Sin(latitude * Deg2Rad) * Mathf.Cos(hourAngle);

        float azimuth = Mathf.Atan(aziNom / aziDenom);

        if (aziDenom < 0) // In 2nd or 3rd quadrant  
        {
            azimuth += Mathf.PI;
        }
        else if (aziNom < 0) // In 4th quadrant  
        {
            azimuth += 2 * Mathf.PI;
        }

        outAltitude = altitude;
        outAzimuth = azimuth;
    }

    /*! 
    * \brief Corrects an angle. 
    * 
    * \param angleInRadians An angle expressed in radians. 
    * \return An angle in the range 0 to 2*PI. 
    */
    private static float CorrectAngle(float angleInRadians)
    {
        if (angleInRadians < 0)
        {
            return 2 * Mathf.PI - (Mathf.Abs(angleInRadians) % (2 * Mathf.PI));
        }
        else if (angleInRadians > 2 * Mathf.PI)
        {
            return angleInRadians % (2 * Mathf.PI);
        }
        else
        {
            return angleInRadians;
        }
    }
}
