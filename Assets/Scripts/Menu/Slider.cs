using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public interface UIComponent
{
    void OnComponentValueChange(float value);
}

public class Slider : MonoBehaviour, Grabbable
{

    public GameObject parentSlider;

    Vector3 initialScale = Vector3.one;
    Vector3 rescaled = Vector3.one;

    float MinPositionC;
    float MaxPositionC;

    [SerializeField]
    public TextMeshPro label;
    string textLabel = "init";

    [SerializeField]
    public string eventName = "";

    // Use this for initialization
    void Start () {
        MinPositionC = transform.position.x -parentSlider.transform.localScale.x/2f;
        MaxPositionC = transform.position.x + parentSlider.transform.localScale.x/2f;
        textLabel = label.text;

    }

    // Update is called once per frame
    void Update () {
		
	}

    public int GetPriority()
    {
        return 0;
    }

    // Grabbable interface

    public void OnDrag(WandController controller)
    {
        // float offset = CalculateLinearMapping(controller.transform, MaxPositionC, MinPositionC);

        Vector3 previousP = transform.position;
        Vector3 p = controller.transform.position;
        if (p.x < MinPositionC) p.x = MinPositionC;
        if (p.x > MaxPositionC) p.x = MaxPositionC;

        p.y = previousP.y;
        p.z = previousP.z;

        transform.position = p;
        float normalisedValue = p.x / (MaxPositionC - MinPositionC) - 0.5f;
        label.text = textLabel + " " + normalisedValue;
        //OnValueChange(normalisedValue);

        EventManager.TriggerEvent(eventName, normalisedValue);
    }

    public bool OnGrab(WandController controller)
    {
        return true;
    }

    public void OnRelease(WandController controller)
    {
    }

    public void OnEnter(WandController controller)
    {
    }

    public void OnExit(WandController controller)
    {
    }
}
