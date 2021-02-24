using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.Windows.Perception.Spatial;
using Microsoft.Windows.Perception.Spatial.Preview;
using MRTKExtensions.QRCodes;
using UnityEngine;

public class QRCodeSceneCalibrator : SceneCalibrator
{
    public QRTrackerController TrackerController;

    protected void Start()
    {
        if (Root == null)
            Root = new GameObject("Root").transform;

        TrackerController.PositionSet += PoseFound;
    }

    private void PoseFound(object sender, Pose pose)
    {
        Root.SetPositionAndRotation(pose.position, pose.rotation);
    }
}
