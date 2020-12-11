using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextMenuControl : MonoBehaviour
{
    public Transform target;
    TextMeshPro tm;
    public int index;
    int currentAxes;
    void Start()
    {
        tm = GetComponent<TextMeshPro>();
        tm.text = name;
    }

    // Update is called once per frame
    void Update()
    {
        
            if (Axis.CurrentAxis.axisId == index)
        {
            tm.color = Color.cyan;
            tm.fontStyle = FontStyles.Underline;
        }
       // if (transform.localPosition.z >0.24f && transform.localPosition.z < 0.26f) // this sucks will need to change...
        //{
            
       // }
        else
        {
            tm.color = Color.grey;
            tm.fontStyle = FontStyles.Normal;
        }
        this.transform.LookAt(target);
        
    }

}
