using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViconOriginSceneCalibrator : SceneCalibrator
{
    public void Start()
    {
        if (Root == null)
            Root = transform;
    }
}
