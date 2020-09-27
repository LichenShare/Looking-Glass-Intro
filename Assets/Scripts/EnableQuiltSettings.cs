using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LookingGlass;
public class EnableQuiltSettings : MonoBehaviour
{
    Holoplay hp;
    public float viewCone = 40;
    Calibration defaultCalibration;
    // Start is called before the first frame update
    void Start()
    {
        hp = GameObject.FindObjectOfType<Holoplay>();
        Debug.Log(hp.cal);
        Debug.Log(hp.cal.viewCone);
        hp.cal.viewCone = viewCone;
        Debug.Log(hp.cal.viewCone);
            defaultCalibration.index = 0;
            defaultCalibration.screenWidth = 1600;
            defaultCalibration.screenHeight = 900;
            defaultCalibration.subp = 0;
            defaultCalibration.viewCone = 40;
            defaultCalibration.aspect = 16f / 9f;
            defaultCalibration.pitch = 10;
            defaultCalibration.slope = 1;
            defaultCalibration.center = 0;
            defaultCalibration.fringe = 0;
            defaultCalibration.serial = "";
            defaultCalibration.LKGname = "default";
            defaultCalibration.unityIndex = 0;
            defaultCalibration.xpos = 0;
            defaultCalibration.ypos = 0;
    }

    // Update is called once per frame
    void Update()
    {
        hp.cal = defaultCalibration;

    }
}
