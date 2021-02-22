using System;
using System.Runtime.InteropServices;

namespace HMDUtils
{
  ////////////////////////////////////////////////////////////////////////////////////////////
  public class FusionService
  {

    private IntPtr m_Impl;
    [StructLayout(LayoutKind.Sequential)]
    public class Vec
    {
      public double X;
      public double Y;
      public double Z;
      public Vec(double x, double y, double z)
      {
        X = x;
        Y = y;
        Z = z;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Quat
    {
      public double X;
      public double Y;
      public double Z;
      public double W;

      public Quat(double x, double y, double z, double w )
      {
        X = x;
        Y = y;
        Z = z;
        W = w;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Pose
    {
      public Quat Rotation;
      public Vec Position;
      public Pose(Vec Pos, Quat Rot)
      {
        Position = Pos;
        Rotation = Rot;
      }
    }

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void HighResTimer( IntPtr pTime );

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ViconToOculus(Quat ViconOrientation, Vec ViconPos, IntPtr outptr);

    public const string pluginName = "HMDFusionUtils_Unity";
    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateFusionService();

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyFusionService(IntPtr pService);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Reset(IntPtr pService);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool UpdateOrientation2(IntPtr pService,
                                                  double Time,
                                                  Quat HMDOrientation,
                                                  Vec HMDOrientationV,
                                                  Quat ViconOrientation,
                                                  Vec ViconPos,
                                                  bool ViconPosValid,
                                                  IntPtr UpdatedOrientation,
                                                  double t, double p, double c);

    ////////////////////////////////////////////////////////////////////////////////////
    static public double GetTime()
    {
      Type cType = typeof(double);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        HighResTimer(ptr);
        return (double)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    static public Pose GetMappedVicon(Quat ViconOrientation, Vec ViconPos)
    {
      Type cType = typeof(Pose);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        ViconToOculus(ViconOrientation, ViconPos, ptr);
        return (Pose)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public void Create()
    {
      m_Impl = CreateFusionService();
      if (m_Impl == IntPtr.Zero)
        throw new System.InvalidOperationException("Service could not be created");
    }
    public void Destroy()
    {
      DestroyFusionService(m_Impl);
    }

    public void ResetService()
    {
      Reset(m_Impl);
    }
    
    public bool GetUpdatedOrientation( double Time,
                                              Quat HMDOrientation,
                                              Vec HMDOrientationV,
                                              Quat ViconOrientation,
                                              Vec ViconPos,
                                              bool ViconPosValid,
                                              double t, double p, double c,
                                              out Quat Output )
    {
      Type cType = typeof(Quat);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        bool bResult = UpdateOrientation2( m_Impl, 
                            Time,
                            HMDOrientation,
                            HMDOrientationV,
                            ViconOrientation,
                            ViconPos,
                            ViconPosValid,
                            ptr,
                            t, p, c );

        Output = (Quat)Marshal.PtrToStructure(ptr, cType);
        return bResult;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
  }

    //////////////////////////////////////////////////////////////////////////////////////////
    public static class OVRPluginServices
    {
        public enum Bool
        {
            False = 0,
            True
        }
        public enum Step
        {
            Render = -1,
            Physics = 0,
        }
        public enum Node
        {
            None = -1,
            EyeLeft = 0,
            EyeRight = 1,
            EyeCenter = 2,
            HandLeft = 3,
            HandRight = 4,
            TrackerZero = 5,
            TrackerOne = 6,
            TrackerTwo = 7,
            TrackerThree = 8,
            Head = 9,
            DeviceObjectZero = 10,
            Count,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}", x, y, z);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Quatf
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Posef
        {
            public Quatf Orientation;
            public Vector3f Position;
            public override string ToString()
            {
                return string.Format("Position ({0}), Orientation({1})", Position, Orientation);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PoseStatef
        {
            public Posef Pose;
            public Vector3f Velocity;
            public Vector3f Acceleration;
            public Vector3f AngularVelocity;
            public Vector3f AngularAcceleration;
            public double Time;
        }

        public const string OvrPluginName = "OVRPlugin";
        [DllImport(OvrPluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool ovrp_SetTrackingPositionEnabled(Bool value);


        [DllImport(OvrPluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern PoseStatef ovrp_GetNodePoseState(Step stateId, Node nodeId);

    }
}
