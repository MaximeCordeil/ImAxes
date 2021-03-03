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

    public enum MathUtilsError
    {
      ESuccess = 10,
      EQuaternionWasNan = 11,
      EZeroTimeDelta = 12,
      EInputIsIdentity = 13,
      ELastRotationIdentity = 14,
      ENoVelocity = 15,
      EUninitialized = 16
    }

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void HighResTimer( IntPtr pTime );

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ViconToOculus(Quat ViconOrientation, Vec ViconPos, IntPtr outptr);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern double ScalarVelocity(Vec HmdOrientation);

    public const string pluginName = "HMDFusionUtils_Unity";
    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateFusionService();

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyFusionService(IntPtr pService);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Reset(IntPtr pService);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern MathUtilsError CalculateAngularVelocity( IntPtr pService,
                                                       Quat i_R,
                                                       double i_T,
                                                       out double o_V);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern MathUtilsError UpdateOrientation2(IntPtr pService,
                                                  double Time,
                                                  Quat i_rHMDOrientation,
                                                  double i_rHMDOrientationV,
                                                  Quat i_rViconOrientation,
                                                  bool i_bViconDataValid,
                                                  IntPtr pUpdatedOrientation);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    public static extern MathUtilsError UpdateOrientation3(IntPtr pService,
                                                  double Time,
                                                  Quat i_rHMDOrientation,
                                                  bool i_bHMDDataValid,
                                                  Quat i_rViconOrientation,
                                                  uint i_ViconFrameNumber,
                                                  bool i_bViconDataValid,
                                                  float i_MaxAngularRateDegrees,
                                                  uint i_WindowSize,
                                                  IntPtr pUpdatedOrientation);

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
    public MathUtilsError GetVelocity( Quat HMDRotation, double i_T, out double o_V )
    { 
        MathUtilsError Result = CalculateAngularVelocity( m_Impl,
                                                          HMDRotation,
                                                          i_T,
                                                          out o_V );
        return Result;
    }
    
    public MathUtilsError GetUpdatedOrientation( double Time,
                                              Quat HMDOrientation,
                                              double HMDOrientationV,
                                              Quat ViconOrientation,
                                              bool ViconDataValid,
                                              out Quat Output )
    {
      Type cType = typeof(Quat);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        MathUtilsError Result = UpdateOrientation2( m_Impl, 
                            Time,
                            HMDOrientation,
                            HMDOrientationV,
                            ViconOrientation,
                            ViconDataValid,
                            ptr);

        Output = (Quat)Marshal.PtrToStructure(ptr, cType);
        return Result;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public MathUtilsError GetUpdatedOrientationWindowed( double Time,
                                              Quat HMDOrientation,
                                              bool HMDDataValid,
                                              Quat ViconOrientation,
                                              uint ViconFrameNumber,
                                              bool ViconDataValid,
                                              out Quat Output )
    {
      Type cType = typeof(Quat);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        MathUtilsError Result = UpdateOrientation3( m_Impl, 
                            Time,
                            HMDOrientation,
                            HMDDataValid,
                            ViconOrientation,
                            ViconFrameNumber,
                            ViconDataValid,
                            1.0f,
                            20,
                            ptr);

        Output = (Quat)Marshal.PtrToStructure(ptr, cType);
        return Result;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public static string StateInString( MathUtilsError FusionState, bool bRequiredVelocityCalculation)
    {
      string FusionError="";
      switch ( FusionState )
      {
        case MathUtilsError.ESuccess:
          if (bRequiredVelocityCalculation)
          {
            FusionError = "Success - Calculated Velocity";
          }
          else
          {
            FusionError = "Success";
          }
          break;
        case MathUtilsError.EQuaternionWasNan:
          FusionError = "Quaternion is Nan";
          break;
        case MathUtilsError.EZeroTimeDelta:
          FusionError = "Zero Time Delta";
          break;
        case MathUtilsError.EInputIsIdentity:
          FusionError = "Input Is Identity";
          break;
        case MathUtilsError.ELastRotationIdentity:
          FusionError = "Last Rotation Is Identity";
          break;
        case MathUtilsError.ENoVelocity:
          FusionError = "No Velocity";
          break;
        case MathUtilsError.EUninitialized:
          FusionError = "Uninitialized";
          break;
      }
      return FusionError;
    }
  }
}
