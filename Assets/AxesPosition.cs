using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxesPosition : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 RotOffset;
    public Transform controller;
    void Start()
    {
        //transform.parent = controller;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.eulerAngles = controller.eulerAngles - RotOffset;
        transform.position = controller.position - offset;
    }
}
