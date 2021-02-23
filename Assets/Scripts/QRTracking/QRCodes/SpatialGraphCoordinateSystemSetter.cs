﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR.WSA;
#if WINDOWS_UWP
using Microsoft.Windows.Perception.Spatial;
#endif

namespace MRTKExtensions.QRCodes
{
    public class SpatialGraphCoordinateSystemSetter : MonoBehaviour
    {
        public EventHandler<Pose> PositionAcquired;
        public EventHandler PositionAcquisitionFailed;

        private Tuple<Guid, float> locationIdSize = null;

        private PositionalLocatorState CurrentState { get; set; }


        void Awake()
        {
            CurrentState = PositionalLocatorState.Unavailable;
        }

        void Start()
        {
            WorldManager.OnPositionalLocatorStateChanged += WorldManager_OnPositionalLocatorStateChanged;
            CurrentState = WorldManager.state;
        }

        private void WorldManager_OnPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
        {
            CurrentState = newState;
            gameObject.SetActive(newState == PositionalLocatorState.Active);
        }

        public void SetLocationIdSize(Guid spatialGraphNodeId, float physicalSideLength)
        {
            if (locationIdSize == null)
            {
                locationIdSize = new Tuple<Guid, float>(spatialGraphNodeId, physicalSideLength);
            }
        }

        void Update()
        {
            if (locationIdSize != null)
            {
                UpdateLocation(locationIdSize.Item1, locationIdSize.Item2);
                locationIdSize = null;
            }
        }

        private void UpdateLocation(Guid spatialGraphNodeId, float physicalSideLength)
        {
            // if (CurrentState != PositionalLocatorState.Active)
            // {
            //     PositionAcquisitionFailed?.Invoke(this, null);
            //     return;
            // }

            System.Numerics.Matrix4x4? relativePose = System.Numerics.Matrix4x4.Identity;
#if WINDOWS_UWP

            SpatialCoordinateSystem coordinateSystem = Microsoft.Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(spatialGraphNodeId);

            if (coordinateSystem == null)
            {
                PositionAcquisitionFailed?.Invoke(this, null);
                return;
            }

            SpatialCoordinateSystem rootSpatialCoordinateSystem = Microsoft.Windows.Perception.Spatial.SpatialCoordinateSystem.FromNativePtr(UnityEngine.XR.WindowsMR.WindowsMREnvironment.OriginSpatialCoordinateSystem);

            // Get the relative transform from the unity origin
            relativePose = coordinateSystem.TryGetTransformTo(rootSpatialCoordinateSystem);
#endif

            if (relativePose == null)
            {
                PositionAcquisitionFailed?.Invoke(this, null);
                return;
            }

            System.Numerics.Matrix4x4 newMatrix = relativePose.Value;

            // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
            // from rhs-rhs to lhs-lhs
            // Convert from right to left coordinate system
            newMatrix.M13 = -newMatrix.M13;
            newMatrix.M23 = -newMatrix.M23;
            newMatrix.M43 = -newMatrix.M43;

            newMatrix.M31 = -newMatrix.M31;
            newMatrix.M32 = -newMatrix.M32;
            newMatrix.M34 = -newMatrix.M34;

            System.Numerics.Vector3 scale;
            System.Numerics.Quaternion rotation1;
            System.Numerics.Vector3 translation1;

            System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1);
            var translation = new Vector3(translation1.X, translation1.Y, translation1.Z);
            var rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W);
            var pose = new Pose(translation, rotation);

            // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
            // to these objects so apply the inverse
            if (CameraCache.Main.transform.parent != null)
            {
                pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
            }
            // Rotate 90 degrees 'forward' over 'right' so 'up' is pointing straight up from the QR code
            pose.rotation *= Quaternion.Euler(90, 0, 0);

            // Move the anchor point to the *center* of the QR code
            var deltaToCenter = physicalSideLength * 0.5f;
            pose.position += (pose.rotation * (deltaToCenter * Vector3.right) -
                              pose.rotation * (deltaToCenter * Vector3.forward));
            gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
            PositionAcquired?.Invoke(this, pose);
        }
    }
}