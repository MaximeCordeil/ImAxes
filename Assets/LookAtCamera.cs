using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform target;
    
   
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(target.position.x,this.transform.position.y,target.position.z);
        this.transform.LookAt(targetPostition);
    }
}   
