using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramWiggle : MonoBehaviour
{
    Camera camera;
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

    // Start is called before the first frame update
    void Start()
    {
        camera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                target = hit.transform;
            }
        }
        */
        if (Input.GetKeyDown(lookAt))
            lookAtTarget = !lookAtTarget;  //toggle look at behavior

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
            transform.LookAt(target);
            if (!loop)
                if (Time.time - startTime > period)
                    animating = false;

        }
        else if (lookAtTarget)
            transform.LookAt(target);
    }
}
