using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobTravel : MonoBehaviour
{
    public WirelessAxes axes;
    public Transform indicator;
    public bool sliderOne;
    public float multiplier;
    public float offset;
    
    public float indicatorMult;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderOne)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, axes.sliderOne * multiplier + offset, transform.localPosition.z);
            indicator.localScale = new Vector3(indicator.localScale.x, indicator.localScale.y, (axes.sliderOne * -1 + 255) * indicatorMult);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, axes.sliderTwo * multiplier + offset, transform.localPosition.z);
            indicator.localScale = new Vector3(indicator.localScale.x, indicator.localScale.y, (axes.sliderTwo ) * indicatorMult);

        }
        
    }
}
