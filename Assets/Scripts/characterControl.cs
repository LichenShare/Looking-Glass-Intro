using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterControl : MonoBehaviour
{
    public float increment = 1;
    public Transform cam;
    public bool animating = false;
    public float period = 3;  //seconds
    public bool loop = false;
    public float angularSweep;
    float wiggleDistance;
    public Transform target;
    public KeyCode startWiggle = KeyCode.Space;
    public KeyCode lookAt = KeyCode.L;
    public bool lookAtTarget = false;
    Vector3 perpendicular, initialPosition;
    float distanceToTarget, startTime;
    public Transform lastDirection;
    public Vector3 lastDir;
    Rigidbody r;
    public mouseLook ml;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        lastDirection = transform;
        lastDirection.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float inc = increment*Time.deltaTime;
        Transform t;
        
            if (lookAtTarget)            
                t = lastDirection;
            else
                t = cam;

            if (Input.GetKey(KeyCode.W))
                r.AddForce(inc * Vector3.Normalize(Vector3.ProjectOnPlane(t.forward, Vector3.up)), ForceMode.Impulse);
            if (Input.GetKey(KeyCode.A))
                r.AddForce(-inc * Vector3.Normalize(Vector3.ProjectOnPlane(t.right, Vector3.up)), ForceMode.Impulse);
            if (Input.GetKey(KeyCode.S))
                r.AddForce(-inc * Vector3.Normalize(Vector3.ProjectOnPlane(t.forward, Vector3.up)), ForceMode.Impulse);
            if (Input.GetKey(KeyCode.D))
                r.AddForce(inc * Vector3.Normalize(Vector3.ProjectOnPlane(t.right, Vector3.up)), ForceMode.Impulse);
            if (Input.GetKey(KeyCode.E))
                transform.localPosition += inc * transform.right;
            if (Input.GetKey(KeyCode.Q))
                transform.localPosition -= inc * transform.right;

        if (Input.GetKey(KeyCode.Space)) //jump
            r.AddForce(inc * transform.up, ForceMode.Impulse);

        if (Input.GetKeyDown(lookAt))
        {
            lookAtTarget = !lookAtTarget;  //toggle look at behavior
            if (lookAtTarget)
            {
                ml.enabled = false;

                lastDirection.position = cam.transform.position;
//                lastDirection.rotation = cam.transform.rotation;
                lastDir = lastDirection.eulerAngles;
            }
            else
                ml.enabled = true;
        }

        if (Input.GetKeyDown(startWiggle))
        {
            if (loop)
                animating = !animating;
            else
                animating = true;
            startTime = Time.time;
            initialPosition = transform.position;
            Vector3 pointer = transform.position - target.position;
            perpendicular = Vector3.Cross(pointer, Vector3.up).normalized;
            float distance = Vector3.Distance(transform.position, target.position);
            wiggleDistance = distance * Mathf.Tan(angularSweep * Mathf.PI / 2 / 180);
        }
        if (animating)
        {
            transform.position = initialPosition + perpendicular * Mathf.Sin(2 * Mathf.PI * (Time.time - startTime) / period) * wiggleDistance;
            cam.transform.LookAt(target);
            if (!loop)
                if (Time.time - startTime > period)
                    animating = false;

        }
        else if (lookAtTarget)
            cam.transform.LookAt(target);
    }
}
