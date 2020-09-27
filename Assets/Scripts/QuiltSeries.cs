using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LookingGlass;

[ExecuteAlways]
public class QuiltSeries : MonoBehaviour
{
    public enum Mode { Physical, Simulation };
    public Texture[] quilts;
    public Holoplay hp;
    public Transform simulator;
    public Mode mode;
    Material simulatedLookingGlass;

    //shortcuts for moving through quilt array
    public KeyCode next=KeyCode.RightArrow;
    public KeyCode previous=KeyCode.LeftArrow;
    public int index = 0;

   void Start()
    {
        if (mode == Mode.Physical)
        {
            if (hp == null) //if the holoplay object hasn't been explicitly set, grab the first one you find in the scene
                hp = GameObject.FindObjectOfType<Holoplay>();
            hp.background = new Color(0, 0, 0, 0);
        }
        if(mode==Mode.Simulation)
        {
            //first check if the simulator is explicitly assigned
            Transform display;
            if (simulator != null)
            {
                display = simulator.FindDeepChild("Looking Glass Display");
                simulatedLookingGlass = display.GetComponent<Renderer>().sharedMaterial;
            }
            else  //if not, look in the children -- if you can't find it there, then look in the whole project
            {
                display = transform.FindDeepChild("Looking Glass Display");
                if (display != null)
                    simulatedLookingGlass = display.GetComponent<Renderer>().material;
                else
                    simulatedLookingGlass = GameObject.FindObjectOfType<Renderer>().material;
            }
        }
        updateQuilt();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(next))
        {
            index++;
            if (index >= quilts.Length)
                index = 0;
            updateQuilt();
        }
        if (Input.GetKeyDown(previous))
        {
            index--;
            if (index < 0)
                index = quilts.Length-1;
            updateQuilt();
        }
    }

    void updateQuilt()
    {
        if (quilts[index] != null)
        {
            if(mode==Mode.Physical)
                hp.overrideQuilt = quilts[index];
            if (mode == Mode.Simulation)
                simulatedLookingGlass.SetTexture("_MainTex", quilts[index]);
        }
    }
}
