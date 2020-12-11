using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniVis : MonoBehaviour
{
    public static List<Mesh> meshes = new List<Mesh>();
    MeshFilter meshfilt;
    public bool top;
    public MiniVis underneath;
    public GameObject faderKnob;// work out whether knob is above or below in order to flip mesh scale
     ClippingPlane faderKnobScript;
    //MeshCollider meshcol;
    float existingBounds;
    void Start()
    {
        meshfilt = GetComponent<MeshFilter>();
       // meshcol = GetComponent<MeshCollider>();
        existingBounds = meshfilt.mesh.bounds.extents.x;
        faderKnobScript = faderKnob.GetComponent<ClippingPlane>();
    }

    
    void Update()
    {
       
            if (faderKnobScript.inverted && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            if (!faderKnobScript.inverted && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }


        

    }

    public void SetMesh(int index)
    {

       if (top)
        {
            underneath.SetMesh(index);
        }
        if (meshes != null)
        {
            meshfilt.mesh = meshes[index];
           // meshcol.sharedMesh = meshfilt.mesh;
        }
        float boundSize = meshfilt.mesh.bounds.size.x;
        if (boundSize!=1f)
        {
           // print("NOT1");
            print( boundSize);
            transform.localScale = new Vector3 (1.09f/boundSize, 1.1f, 1.1f);
        }
    }
}
