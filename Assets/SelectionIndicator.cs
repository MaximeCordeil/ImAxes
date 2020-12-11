using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public Transform selectionInd;
    public WirelessAxes axes;
    public float multiplier;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int dif = axes.sliderOne - axes.sliderTwo;
        selectionInd.localScale = new Vector3(selectionInd.localScale.x, dif * multiplier, selectionInd.localScale.z);
    }
}
