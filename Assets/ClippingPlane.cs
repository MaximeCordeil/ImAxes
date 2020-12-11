using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    
    public Material mat;
    public bool inverted;
    public Transform faderTwo;
    int invert1;
    int invert2;
   // public Transform miniVis1;
    //public Transform miniVis2;
    //execute every frame
    void Update()
    {
        if (faderTwo.localPosition.y > this.transform.localPosition.y&& !inverted)
        {
            inverted = true;
            invert1 = -1;
            invert2 = 1;
           // flipMeshes();
        }
        if (faderTwo.localPosition.y <this.transform.localPosition.y && inverted)
        {
            inverted = false;
            invert1 = 1;
            invert2 = -1;
        //    flipMeshes();
        }

            Plane plane = new Plane(transform.right *invert1, transform.position);
           
            Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
           
        
            Plane plane2 = new Plane(faderTwo.transform.right * invert2, faderTwo.transform.position);
          
            Vector4 planeRepresentation2 = new Vector4(plane2.normal.x, plane2.normal.y, plane2.normal.z, plane2.distance);
        
            mat.SetVector("_Plane", planeRepresentation);
            mat.SetVector("_Plane1", planeRepresentation2);
      
    }
    void flipMeshes()
    {
        
           // miniVis1.localScale = new Vector3(miniVis1.localScale.x * -1, miniVis1.localScale.y, miniVis1.localScale.z);
            //miniVis2.localScale = new Vector3(miniVis2.localScale.x * -1, miniVis2.localScale.y, miniVis2.localScale.z);
        

       


    }
}
