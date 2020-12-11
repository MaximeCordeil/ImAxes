using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamControl : MonoBehaviour
{
    BoxCollider col;
   
    public WirelessAxes ax;
    bool pressed;
    public AxisController axiscontrol;
    Axis selectedAxis;
    Transform objectSel;
    bool enabled;
    bool objectSelected;
    Vector3 objectScale;
    Vector3 objectPos;
    int scaleSlider;
    int posSlider;
    float multiplier = 0.1f;
    void Start()
    {
       
        col = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ax.buttonPress ==1)
        {
            pressed = true;
        }
        else
        {
            pressed = false;
        }
        if (col.enabled ==false)
        {
            objectSelected = false;
            if (selectedAxis != null)
            {
                selectedAxis.transform.parent = null;
            }
            if (objectSel != null)
            {
                objectSel.parent = null;
                objectSel = null;
            }
        }
        else
        {
            if (objectSel != null)
            {
                print(objectSel.position.z - (posSlider - ax.sliderOne));
               // objectSel.localPosition = new Vector3(objectSel.position.x, objectSel.position.y, objectSel.position.z - (posSlider - ax.sliderOne)*multiplier);
                posSlider = ax.sliderOne;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Axis" &&!objectSelected)
        {
            print("Collided with! ");
            objectSelected = true;
            print(other.gameObject.name);
            selectedAxis = other.GetComponent<Axis>(); 
            axiscontrol.SetAxisViaBeam(selectedAxis);
            selectedAxis.transform.parent = ax.transform;
        }
       
    }
    void  OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Object")
        {
            print(objectSel.position.z - (posSlider - ax.sliderOne));
            objectSel.position = new Vector3(objectSel.position.x, objectSel.position.y, objectSel.position.z - (posSlider - ax.sliderOne));
        }
    }
}
