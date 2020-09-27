using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropMarker : MonoBehaviour
{
    public Transform marker;  //marker prefab
    public KeyCode markerDrop=KeyCode.M;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(markerDrop))
            Instantiate(marker, transform.position, Quaternion.identity);
    }
}
