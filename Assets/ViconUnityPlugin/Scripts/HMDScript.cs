using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if  UNITY_XR_MANAGEMENT
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

using ViconDataStreamSDK.CSharp;


public class HMDScript : BasePoseProvider
{
    public String HmdName = "";
    public ViconDataStreamClient Client;
    public bool Log = false;

    private HMDUtils.FusionService m_Service;
    private StreamWriter m_Log;

    private HMDUtils.FusionService.Pose m_LastGoodPose;
    private bool bAlwaysCalculateVelocity = false;
    private bool bRequiredVelocityCalculation = false;
    private bool bAltAlgo = true;

    bool SetupHMD()
    {
        m_Service = new HMDUtils.FusionService();
        m_Service.Create();
        m_Service.ResetService();

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
        if (HmdName == "" )
        {
            Debug.LogError("Headset tracked in Vicon not provided.");
        }
        if ( Client == null )
        {
            Debug.LogError(string.Format("No data stream client available."));
        }
        if (Log)
        {
            WriteLogHeader();
        }
        SetupHMD();
    }

    HMDUtils.FusionService.Quat Adapt(Quaternion In)
    {
        HMDUtils.FusionService.Quat Output = new HMDUtils.FusionService.Quat(In.x, In.y, In.z, In.w);
        return Output;
    }

    HMDUtils.FusionService.Vec Adapt(Vector3 In)
    {
        HMDUtils.FusionService.Vec Output = new HMDUtils.FusionService.Vec(In.x, In.y, In.z);
        return Output;
    }

   Quaternion Adapt( HMDUtils.FusionService.Quat In )
    {
        //HMDUtils.FusionService.Quat Output = new HMDUtils.FusionService.Quat(-In.x, -In.y, In.z, In.w);
        return new Quaternion( (float)In.X, (float)In.Y, (float)In.Z, (float)In.W );
    }

    Vector3  Adapt( HMDUtils.FusionService.Vec In)
    {
        return new Vector3((float)In.X, (float)In.Y, (float)In.Z);
    }

    public override PoseDataFlags GetPoseFromProvider(out Pose output)
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

            // to headset
            HMDUtils.FusionService.Pose ViconInHMD = HMDUtils.FusionService.GetMappedVicon(ViconOrientation, ViconPosition);

            // headset to unity
            ViconOrientation = ViconInHMD.Rotation;
            ViconPosition = ViconInHMD.Position;
            HMDUtils.FusionService.Quat Rotation = new HMDUtils.FusionService.Quat(-ViconOrientation.X, -ViconOrientation.Y, ViconOrientation.Z, ViconOrientation.W);
            HMDUtils.FusionService.Vec Position = new HMDUtils.FusionService.Vec(ViconPosition.X, ViconPosition.Y, -ViconPosition.Z);
            HMDUtils.FusionService.Pose ViconInUnity = new HMDUtils.FusionService.Pose(Position, Rotation);

            HMDUtils.FusionService.Quat HmdOrtUnity = new HMDUtils.FusionService.Quat( 0, 0, 0, 1 ); 
            HMDUtils.FusionService.Vec HmdOrtVUnity = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdOrtAUnity = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosUnity = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosVUnity = new HMDUtils.FusionService.Vec(0, 0, 0);
            HMDUtils.FusionService.Vec HmdPosAUnity = new HMDUtils.FusionService.Vec(0, 0, 0);

            List<UnityEngine.XR.XRNodeState> XRNodeStates = new List<UnityEngine.XR.XRNodeState>();
            UnityEngine.XR.InputTracking.GetNodeStates(XRNodeStates);
            bool bOK = false;
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
                        HmdOrtUnity = Adapt(Ort);
                        HmdOrtVUnity = Adapt(OrtV);
                        HmdOrtAUnity = Adapt(OrtA);
                        HmdPosUnity = Adapt(Pos);
                        HmdPosVUnity = Adapt(PosV);
                        HmdPosAUnity = Adapt(PosA);
                    }
                }
            }
            // Not sure whether we actually require this, plus XR doesn't give it.
            double HmdTime = 0;

            if ( m_Service != null)
            {
                HMDUtils.FusionService.MathUtilsError FusionState = HMDUtils.FusionService.MathUtilsError.ENoVelocity;
                double V = HMDUtils.FusionService.ScalarVelocity(HmdOrtVUnity);
                if (V == 0 || bAlwaysCalculateVelocity)
                {
                  bRequiredVelocityCalculation = true;
                  FusionState = m_Service.GetVelocity(HmdOrtUnity, Time, out V);
                }
                else
                {
                  bRequiredVelocityCalculation = false;
                  FusionState = HMDUtils.FusionService.MathUtilsError.ESuccess;
                }

                HMDUtils.FusionService.Quat Output = new HMDUtils.FusionService.Quat(0,0,0,1);

                if (FusionState == HMDUtils.FusionService.MathUtilsError.ESuccess)
                {
                   if( bAltAlgo )
                    FusionState = m_Service.GetUpdatedOrientationWindowed(Time, HmdOrtUnity, true,  ViconInUnity.Rotation, 0, true, out Output);
                   else
                    FusionState = m_Service.GetUpdatedOrientation(Time, HmdOrtUnity, V, ViconInUnity.Rotation, bViconPoseValid, out Output);
                }

                if (FusionState != HMDUtils.FusionService.MathUtilsError.ESuccess)
                {
                  //fall back to vicon rotation
                  Output = ViconInUnity.Rotation;
                }
                
                if (Log)
                {
                    m_Log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36}",
                                      Time, HmdTime,
                                      HmdOrtUnity.X, HmdOrtUnity.Y, HmdOrtUnity.Z, HmdOrtUnity.W,
                                      HmdOrtVUnity.X, HmdOrtVUnity.Y, HmdOrtVUnity.Z,
                                      HmdOrtAUnity.X, HmdOrtAUnity.Y, HmdOrtAUnity.Z,
                                      HmdPosUnity.X, HmdPosUnity.Y, HmdPosUnity.Z,
                                      HmdPosVUnity.X, HmdPosVUnity.Y, HmdPosVUnity.Z,
                                      HmdPosAUnity.X, HmdPosAUnity.Y, HmdPosAUnity.Z,
                                      ViconFrameNumber,
                                      ViconInUnity.Rotation.X, ViconInUnity.Rotation.Y, ViconInUnity.Rotation.Z, ViconInUnity.Rotation.W,
                                      ViconInUnity.Position.X, ViconInUnity.Position.Y, ViconInUnity.Position.Z,
                                      ViconOrientation.X, ViconOrientation.Y, ViconOrientation.Z, ViconOrientation.W,
                                      ViconPosition.X, ViconPosition.Y, ViconPosition.Z,
                                      HMDUtils.FusionService.StateInString( FusionState, bRequiredVelocityCalculation));
                }

                if ( FusionState == HMDUtils.FusionService.MathUtilsError.ESuccess )
                {
                    output = new Pose(Adapt(ViconInUnity.Position), Adapt(Output));
                    return PoseDataFlags.Position | PoseDataFlags.Rotation;
                }

            }

            if (m_LastGoodPose == null)
            {
                output = new Pose(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
                return PoseDataFlags.NoData;
            }
            else
            {
                output = new Pose(Adapt(m_LastGoodPose.Position), Adapt(m_LastGoodPose.Rotation));
                Debug.LogWarning("using last postion");
                return PoseDataFlags.Position | PoseDataFlags.Rotation;
            }
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogError(string.Format("XR must be enabled for this project to use the HMD fusion script: Error {0}", ex.Message));
            output = new Pose( Adapt( m_LastGoodPose.Position), Adapt(m_LastGoodPose.Rotation) );
            return PoseDataFlags.NoData;
        }
    }
}
#endif
