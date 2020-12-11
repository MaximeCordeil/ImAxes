using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAxesMovement : MonoBehaviour
{
    WirelessAxes thisAxes;
    public Transform sliderOne;
    public Transform sliderTwo;
    public Transform knob;
    public GameObject button;
    MeshRenderer buttonLed;
   
    Color c;
    void Start()
    {
        thisAxes = GetComponent<WirelessAxes>();
        buttonLed = button.GetComponent<MeshRenderer>();
        c = buttonLed.material.color;
       
    }

    // Update is called once per frame
    void Update()
    {
        sliderOne.transform.localPosition = new Vector3(-0.392f, ((thisAxes.sliderOne / 256.0f) * -1 + 1.817f), 0.4763f);
        sliderTwo.transform.localPosition = new Vector3(-0.2136f, ((thisAxes.sliderTwo / 256.0f) * -1 + 1.817f), 0.4763f);
        knob.transform.localEulerAngles = new Vector3(-90, 0, thisAxes.rotary*4);
        buttonLed.material.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * (thisAxes.LEDValue/64f));
    }
}
