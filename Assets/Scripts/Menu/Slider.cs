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

public class Slider : MonoBehaviour
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
}
