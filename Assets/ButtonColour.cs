using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonColour : MonoBehaviour
{
    public Color active;
    public Color inactive;
    public WirelessAxes ax;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ax.buttonPress == 1)
        {
            rend.material.color = active;
        }
        else
        {
            rend.material.color = inactive; 
        }
    }
}
