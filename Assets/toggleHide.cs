﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleHide : MonoBehaviour
{
    public KeyCode key;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
            foreach (Transform child in transform)
                child.gameObject.SetActive(!child.gameObject.activeSelf);
        
    }
}
