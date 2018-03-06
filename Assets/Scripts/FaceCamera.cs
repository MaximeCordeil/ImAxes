using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Camera.main != null)
        {
            Vector3 s = transform.localScale;

            var us = Mathf.Sign(Vector3.Dot(transform.up, Vector3.up));
            s.y = us * Mathf.Abs(s.y);
            s.x = Mathf.Sign(Vector3.Dot(transform.forward, Camera.main.transform.forward * us)) * Mathf.Abs(s.x);

            transform.localScale = s;
        }
	}
    
}
