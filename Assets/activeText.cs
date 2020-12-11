using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class activeText : MonoBehaviour
{
    TextMeshPro tm;
    void Start()
    {
        tm = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        string name = Axis.CurrentAxis.name.Split(new char[] { '(', ')' })[0]; // gets rid of (clone) in name
        tm.text = name;
    }
}
