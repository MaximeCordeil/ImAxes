using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using ViconDataStreamSDK.CSharp;

public class HMDScript : MonoBehaviour
{
    public String HmdName = "";
    public ViconDataStreamClient Client;
    public bool Log = false;
    
    private bool XRCalls = true;

    private HMDUtils.FusionService m_Service;
    private StreamWriter m_Log;

    private HMDUtils.FusionService.Pose m_LastGoodPose;

    bool SetupHMD()
    {
        m_Service = new HMDUtils.FusionService();
        m_Service.Create();
        m_Service.ResetService();

        // Disable positional tracking
        if (!XRCalls)
        {
            HMDUtils.OVRPluginServices.ovrp_SetTrackingPositionEnabled(HMDUtils.OVRPluginServices.Bool.False);
        }
        else
        {
            UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        }

        // Cache child camera
        Camera AttachedCamera = GetComponentInChildren<Camera>();
        if (AttachedCamera == null)
        {
            Debug.LogError("Missing Camera component!");
        }
        else
        {
            // We get a offset in the attached camera using SteamVR. Null it out here 
            if (AttachedCamera.transform.localPosition != Vector3.zero)
            {
                AttachedCamera.transform.localPosition = Vector3.zero;
            }
        }

        return true;
    }

    private void WriteLogHeader()
    {
        String DateTime = System.DateTime.Now.ToString();
        DateTime = DateTime.Replace(" ", "_");
        DateTime = DateTime.Replace("/", "_");
        DateTime = DateTime.Replace(":", "_");
        String PathName = Application.dataPath + "/" + DateTime + "_Log.csv";
        print("Writing log to " + PathName);
        m_Log = new StreamWriter(PathName);
        m_Log.AutoFlush = false;
        m_Log.WriteLine("Wall Timer, Oculus Timestamp, Oculus rot QX, Oculus rot QY, Oculus rot QZ, Oculus rot QW, Oculus rot X V, Oculus rot Y V, Oculus rot Z V, Oculus rot X A, Oculus rot Y A, Oculus rot Z A, Oculus pos X, Oculus pos Y, Oculus pos Z, Oculus pos X V, Oculus pos Y V, Oculus pos Z V, Oculus pos X A, Oculus pos Y A, Oculus pos Z A, Vicon Frame Number, Vicon rot QX, Vicon rot QY, Vicon rot QZ, Vicon rot QW, Vicon pos X, Vicon pos Y, Vicon pos Z, Vicon raw rot QX, Vicon raw rot QY, Vicon raw rot QZ, Vicon raw rot QW, Vicon raw Pos X, Vicon raw pos Y, Vicon raw pos Z");
    }

    private void OnDestroy()
    {
        m_Service.ResetService();
        m_Service.Destroy();
        if (Log)
        {
            m_Log.Flush();
            m_Log.Close();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (Log)
        {
            WriteLogHeader();
        }
        SetupHMD();
    }

    HMDUtils.FusionService.Quat Adapt(Quaternion In)
    {
        HMDUtils.FusionService.Quat Output = new HMDUtils.FusionService.Quat(-In.x, -In.y, In.z, In.w);
        return Output;
    }

    HMDUtils.FusionService.Vec Adapt(Vector3 In)
    {
        HMDUtils.FusionService.Vec Output = new HMDUtils.FusionService.Vec(In.x, In.y, In.z);
        return Output;
    }

    HMDUtils.FusionService.Quat Adapt(HMDUtils.OVRPluginServices.Quatf In)
    {
        HMDUtils.FusionService.Quat Output = new HMDUtils.FusionService.Quat(In.x, In.y, In.z, In.w);
        return Output;
    }

    HMDUtils.FusionService.Vec Adapt(HMDUtils.OVRPluginServices.Vector3f In)
    {
        HMDUtils.FusionService.Vec Output = new HMDUtils.FusionService.Vec(In.x, In.y, In.z);
        return Output;
    }

    void LateUpdate()
    {

        try
        {
            double Time = HMDUtils.FusionService.GetTime();
            uint ViconFrameNumber = Client.GetFrameNumber();

            //position
            Output_GetSubjectRootSegmentName RootName = Client.GetSubjectRootSegmentName(HmdName);
            Output_GetSegmentLocalTranslation Translation = Client.GetSegmentTranslation(HmdName, RootName.SegmentName);

            // Raw Vicon position, scale is in mm. The data here is in the datastream default; x-forward, y-left, z-up for the global coordinate system
            HMDUtils.FusionService.Vec ViconPosition = new HMDUtils.FusionService.Vec(Translation.Translation[0], Translation.Translation[1], Translation.Translation[2]);

            //orientation. The local coordinate system of the HMD object is x-right, y-up, z-back
            Output_GetSegmentLocalRotationQuaternion Rot = Client.GetSegmentRotation(HmdName, RootName.SegmentName);

            // Raw Vicon orientation
            HMDUtils.FusionService.Quat ViconOrientation = new HMDUtils.FusionService.Quat(Rot.Rotation[0], Rot.Rotation[1], Rot.Rotation[2], Rot.Rotation[3]);

            // If we don't get a result, or the pose returned from the datastream is occluded, then we will use the last known good position that we received.
            bool bViconPoseValid = true;

            if (Rot.Result != ViconDataStreamSDK.CSharp.Result.Success || Rot.Occluded || Translation.Occluded)
            {
                // We use this flag to determine whether to initialize the fusion algorithm; we don't want to initialize it on occluded frames
                bViconPoseValid = false;

                if (m_LastGoodPose != null)
                {
                    ViconPosition = m_LastGoodPose.Position;
                    ViconOrientation = m_LastGoodPose.Rotation;
                }
                else
                {
                    // If all else fails, we will return the origin :(
                    ViconOrientation = new HMDUtils.FusionService.Quat(0, 0, 0, 1);
                }
            }
            else
            {
                if (m_LastGoodPose == null)
                {
                    m_LastGoodPose = new HMDUtils.FusionService.Pose(ViconPosition, ViconOrientation);
                }
                else
                {
                    m_LastGoodPose.Position = ViconPosition;
                    m_LastGoodPose.Rotation = ViconOrientation;
                }
            }

            //Oculus space. We need to translate to the oculus coordinate system here so that the fusion algorithm can work with all data in the same coordinate system.
            // The Vicon data comes in as z-up, x-forward rhs. We convert to y-up, z-back rhs. The local coordinate system of the tracked Oculus object in the Vicon data is already y-up, z-back.
            // The conversion also scales from mm to m.

            //           Vicon Oculus  Unity
            // forward    x     -z     z
            // up         z      y     y
            // right     -y      x     x
            // https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system

            //HMDUtils.FusionService.Quat ViconOrientationInOculus = new HMDUtils.FusionService.Quat(-ViconOrientation.Y, ViconOrientation.Z, -ViconOrientation.X, ViconOrientation.W);
            //HMDUtils.FusionService.Vec ViconPositionInOculus = new HMDUtils.FusionService.Vec(-ViconPosition.Y * 0.001, ViconPosition.Z * 0.001, -ViconPosition.X * 0.001);
            //HMDUtils.FusionService.Pose ViconInOculus = new HMDUtils.FusionService.Pose( ViconPositionInOculus, ViconOrientationInOculus );


            // For XR, convert the vicon data to Unity coordinate system
            //HMDUtils.FusionService.Quat Rotation = new HMDUtils.FusionService.Quat(ViconOrientation.Y, -ViconOrientation.Z, -ViconOrientation.X, ViconOrientation.W);
            //HMDUtils.FusionService.Vec Position = new HMDUtils.FusionService.Vec(-ViconPosition.Y * 0.001, ViconPosition.Z * 0.001, ViconPosition.X * 0.001);
            //HMDUtils.FusionService.Pose ViconInUnity = new HMDUtils.FusionService.Pose(Position, Rotation);

            // Otherwise convert to Oculus
            HMDUtils.FusionService.Pose ViconInOculus = HMDUtils.FusionService.GetMappedVicon(ViconOrientation, ViconPosition);

            bool bOK = false;

            HMDUtils.FusionService.Quat HmdOrtOculus = new HMDUtils.FusionService.Quat( 0, 0, 0, 0 ); 
            HMDUtils.FusionService.Vec HmdOrtVOculus = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdOrtAOculus = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosOculus = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosVOculus = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosAOculus = new HMDUtils.FusionService.Vec(0, 0, 0);


            // Not sure whether we actually require this, plus XR doesn't give it.
            double HmdTime = 0; 

            if (!XRCalls)
            {
                // We have to assume this
                bOK = true;

                // The pose from the oculus; this is already in oculus coordinate system - y - up, z - back, rhs
                HMDUtils.OVRPluginServices.PoseStatef HMDState = HMDUtils.OVRPluginServices.ovrp_GetNodePoseState(HMDUtils.OVRPluginServices.Step.Render, HMDUtils.OVRPluginServices.Node.EyeCenter);

                HmdOrtOculus = Adapt(HMDState.Pose.Orientation);
                HmdOrtVOculus = Adapt(HMDState.AngularVelocity);
                HmdOrtAOculus = Adapt(HMDState.AngularAcceleration);
                HmdPosOculus = Adapt(HMDState.Pose.Position);
                HmdPosVOculus = Adapt(HMDState.Velocity);
                HmdPosAOculus = Adapt(HMDState.Acceleration);

                HmdTime = HMDState.Time;

            }
            else
            {
                List<UnityEngine.XR.XRNodeState> XRNodeStates = new List<UnityEngine.XR.XRNodeState>();
                UnityEngine.XR.InputTracking.GetNodeStates(XRNodeStates);
                foreach (var State in XRNodeStates)
                {
                    if (State.nodeType == UnityEngine.XR.XRNode.CenterEye)
                    {
                        Quaternion Ort = new Quaternion();
                        Vector3 OrtV = new Vector3();
                        Vector3 OrtA = new Vector3();
                        Vector3 Pos = new Vector3();
                        Vector3 PosV = new Vector3();
                        Vector3 PosA = new Vector3();

                        bOK = State.TryGetRotation(out Ort);

                        // If this is present, we will use it. If not, we will calculate it.
                        if (!State.TryGetAngularVelocity(out OrtV))
                        {
                          OrtV = Vector3.zero;
                        }

                        // We don't use these, but continue to obtain them for logging purposes
                        State.TryGetAngularAcceleration(out OrtA);
                        State.TryGetPosition(out Pos);
                        State.TryGetVelocity(out PosV); 
                        State.TryGetAcceleration(out PosA); 

                        if (bOK)
                        {
                            HmdOrtOculus = Adapt(Ort);
                            HmdOrtVOculus = Adapt(OrtV);
                            HmdOrtAOculus = Adapt(OrtA);
                            HmdPosOculus = Adapt(Pos);
                            HmdPosVOculus = Adapt(PosV);
                            HmdPosAOculus = Adapt(PosA);
                        }
                    }
                }
            }

            if (bOK && m_Service != null)
            {
                HMDUtils.FusionService.Quat Output;
                bool bOutputOk = m_Service.GetUpdatedOrientation(Time, HmdOrtOculus, HmdOrtVOculus, ViconInOculus.Rotation, ViconInOculus.Position, bViconPoseValid, (float)1, (float)0.0137, (float)0.00175, out Output);
                if ( bOutputOk )
                {

                    if (Log)
                    {
                        m_Log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35}",
                                          Time, HmdTime,
                                          HmdOrtOculus.X, HmdOrtOculus.Y, HmdOrtOculus.Z, HmdOrtOculus.W,
                                          HmdOrtVOculus.X, HmdOrtVOculus.Y, HmdOrtVOculus.Z,
                                          HmdOrtAOculus.X, HmdOrtAOculus.Y, HmdOrtAOculus.Z,
                                          HmdPosOculus.X, HmdPosOculus.Y, HmdPosOculus.Z,
                                          HmdPosVOculus.X, HmdPosVOculus.Y, HmdPosVOculus.Z,
                                          HmdPosAOculus.X, HmdPosAOculus.Y, HmdPosAOculus.Z,
                                          ViconFrameNumber,
                                          ViconInOculus.Rotation.X, ViconInOculus.Rotation.Y, ViconInOculus.Rotation.Z, ViconInOculus.Rotation.W,
                                          ViconInOculus.Position.X, ViconInOculus.Position.Y, ViconInOculus.Position.Z,
                                          ViconOrientation.X, ViconOrientation.Y, ViconOrientation.Z, ViconOrientation.W,
                                          ViconPosition.X, ViconPosition.Y, ViconPosition.Z);
                    }

                    Quaternion OutputOrt = new Quaternion((float)-Output.X, (float)-Output.Y, (float)Output.Z, (float)Output.W);
                    Quaternion OculusOrt = new Quaternion((float)-HmdOrtOculus.X, (float)-HmdOrtOculus.Y, (float)HmdOrtOculus.Z, (float)HmdOrtOculus.W);
                    transform.localPosition = new Vector3((float)ViconInOculus.Position.X, (float)ViconInOculus.Position.Y, (float)-ViconInOculus.Position.Z);
                    transform.localRotation = OutputOrt * Quaternion.Inverse(OculusOrt);
                }

            }
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogError(string.Format("XR must be enabled for this project to use the HMD fusion script: Error {0}", ex.Message));
        }
    }
}
