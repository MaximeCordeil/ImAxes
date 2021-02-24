using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViconOriginSceneCalibrator : MonoBehaviour
{   
    static ViconOriginSceneCalibrator _instance;
    public static ViconOriginSceneCalibrator Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ViconOriginSceneCalibrator>()); }
    }
    
    public Transform Root;

    public void Awake()
    {
        if (Root == null)
            Root = transform;
    }
}
