using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTest : MonoBehaviour
{
    Axis axis;
    public bool Found;
    public float minFilter;
    public float maxFilter;
    public float MinNormaliser;
    public float MaxNormaliser;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (axis != null)
        {
            Found = true;
            if (Input.GetKeyDown(KeyCode.A))
            {
                maxFilter += -0.01f;
                axis.SetMaxFilter(maxFilter);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                minFilter += 0.01f;
                axis.SetMinFilter(minFilter);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                MaxNormaliser += -0.01f;
                axis.SetMaxNormalizer(MaxNormaliser);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MinNormaliser += 0.01f;
                axis.SetMinNormalizer(MinNormaliser);
            }
        }
        else
        {
            Found = false;
            if (Input.GetKeyDown(KeyCode.F))
            {
                GetAxis();
            }
        }
    }
    void GetAxis()
    {
        GameObject theAxis = GameObject.Find("axis citric acid");
        if (theAxis != null)
        {
            axis = theAxis.GetComponent<Axis>();
            minFilter = axis.MinFilter;
            maxFilter = axis.MaxFilter;
            MinNormaliser = axis.MinNormaliser;
            MaxNormaliser = axis.MaxNormaliser;
        }
    }
}
