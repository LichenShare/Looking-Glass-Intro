using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropMarker : MonoBehaviour
{
    public Transform flipMarker;  //marker prefab
    public KeyCode flipMarkerDrop = KeyCode.F;
    public Transform reflectionMarker;  //marker prefab
    public KeyCode reflectionMarkerDrop = KeyCode.R;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(flipMarkerDrop))
            Instantiate(flipMarker, transform.position, Quaternion.identity);
        if (Input.GetKeyDown(reflectionMarkerDrop))
            Instantiate(reflectionMarker, transform.position, Quaternion.identity);
    }
}
