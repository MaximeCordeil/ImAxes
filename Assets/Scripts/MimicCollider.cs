using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicCollider : MonoBehaviour {

    public SphereCollider Collider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(Collider.radius, Collider.radius , Collider.radius );
        transform.localPosition = Collider.center;

    }
}
