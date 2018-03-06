using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleJoint : MonoBehaviour {

	public Rigidbody body1;
	public Rigidbody body2;

    public float repulsion = 0.005f;
    public float length = 0.225f;//0.225f;
    public float springk = 12.2f;


    // Update is called once per frame
    void Update () {
		Debug.DrawLine (body1.position, body2.position);			
	}

	void AddCoplanarTorque(Rigidbody b1, Rigidbody b2){
		Vector3 x1 = Vector3.Cross(
			b1.transform.forward, b2.transform.forward
		);
		AddTorqueAroundVector(b1, x1);
		
		Vector3 x2 = Vector3.Cross(
			b1.transform.up, b2.transform.up
		);
		AddTorqueAroundVector(b1, x2);
	}

	void AddTorqueAroundVector(Rigidbody b1, Vector3 dir){

		float theta1 = Mathf.Asin(dir.magnitude);
		Vector3 w1 = dir.normalized * theta1 / Time.fixedDeltaTime;

		Quaternion q1 = b1.transform.rotation * b1.inertiaTensorRotation;
		Vector3 T1 = q1 * Vector3.Scale(b1.inertiaTensor, Quaternion.Inverse(q1) * w1);

		b1.AddTorque(T1);	
	}


	void FixedUpdate(){

		// make the axis co-planar
		AddCoplanarTorque(body2, body1);
		AddCoplanarTorque(body1, body2);
		
		// align the axis on the plane
		Vector3 joint = body2.position - body1.position;

		Vector3 jlocal1 = body1.transform.InverseTransformVector (-joint);
		Vector3 vel1 = new Vector3 (0, jlocal1.y * 15, 0);
		body2.AddForce (vel1);			

		//Vector3 jlocal2 = body2.transform.InverseTransformVector (joint);
		//Vector3 vel2 = new Vector3 (jlocal2.x * 15, jlocal2.y * 15, 0);
		//body1.AddForce (vel2);		

		// separate axis using a spring layout
		ApplyColombsLaw();
		ApplyHookesLaw();

	}

	
    void ApplyColombsLaw()
    {
        Vector3 d = body1.position - body2.position;
        float distance = d.magnitude + 0.001f;
        Vector3 direction = d.normalized;

        body1.AddForce((direction * repulsion) / (distance * distance * 0.5f));
        body2.AddForce((direction * repulsion) / (distance * distance * -0.5f));

    }

    void ApplyHookesLaw()
    {
        Vector3 d = body2.position - body1.position;
        float displacement = length - d.magnitude;
        Vector3 direction = d.normalized;

        body1.AddForce(springk * direction * displacement * -0.5f);
        body2.AddForce(springk * direction * displacement * 0.5f);


    }

}
