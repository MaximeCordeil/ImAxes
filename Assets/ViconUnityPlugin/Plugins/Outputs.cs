using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ViconDataStreamSDK.CSharp
{
    public enum Result:int
    {
        Unknown = 0,
        NotImplemented = 1,
        Success = 2,
        InvalidHostName = 3,
        InvalidMulticastIP = 4,
        ClientAlreadyConnected = 5,
        ClientConnectionFailed = 6,
        ServerAlreadyTransmittingMulticast = 7,
        ServerNotTransmittingMulticast = 8,
        NotConnected = 9,
        NoFrame = 10,
        InvalidIndex = 11,
        InvalidCameraName = 12,
        InvalidSubjectName = 13,
        InvalidSegmentName = 14,
        InvalidMarkerName = 15,
        InvalidDeviceName = 16,
        InvalidDeviceOutputName = 17,
        InvalidLatencySampleName = 18,
        CoLinearAxes = 19,
        LeftHandedAxes = 20,
        HapticAlreadySet = 21, 
        EarlyDataRequested = 22, 
        LateDataRequested = 23,
        InvalidOperation = 24,
        NotSupported = 25,
        ConfigurationFailed = 26,
        NotPresent = 27
    }

    public enum StreamMode:int
    {
        ClientPull = 0,
        ClientPullPreFetch = 1,
        ServerPush = 2,
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Forward = 4,
        Backward = 5,
    }

    public enum DeviceType
    {
        Unknown = 0,
        ForcePlate = 1,
        EyeTracker = 2,
    }

    public enum Unit
    {
        Unknown = 0,
        Volt = 1,
        Newton = 2,
        NewtonMeter = 3,
        Meter = 4,
        Kilogram = 5,
        Second = 6,
        Ampere = 7,
        Kelvin = 8,
        Mole = 9,
        Candela = 10,
        Radian = 11,
        Steradian = 12,
        MeterSquared = 13,
        MeterCubed = 14,
        MeterPerSecond = 15,
        MeterPerSecondSquared = 16,
        RadianPerSecond = 17,
        RadianPerSecondSquared = 18,
        Hertz = 19,
        Joule = 20,
        Watt = 21,
        Pascal = 22,
        Lumen = 23,
        Lux = 24,
        Coulomb = 25,
        Ohm = 26,
        Farad = 27,
        Weber = 28,
        Tesla = 29,
        Henry = 30,
        Siemens = 31,
        Becquerel = 32,
        Gray = 33,
        Sievert = 34,
        Katal = 35,
    }

    public enum TimecodeStandard
    {
        None = 0,
        PAL = 1,
        NTSC = 2,
        NTSCDrop = 3,
        Film = 4, 
        NTSCFilm = 5,
        ATSC = 6
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_Connect
    {
        public Result Result;

        //public Output_Connect() { }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetVersion
    {
        public uint Major;
        public uint Minor;
        public uint Point;
        public uint Revision;

        //public Output_GetVersion() { }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsConnected
    {
        
        public bool Connected;

        //public Output_IsConnected() { }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSubjectName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SubjectName;
        //public Output_GetSubjectName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentGlobalRotationQuaternion
    {       
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public double[] Rotation;
        public bool Occluded;
        /*public Output_GetSegmentGlobalRotationQuaternion()
        {
            Result = Result.Unknown;
            //Rotation = new double[] { 0, 0, 0, 1.0f };
            Occluded = false;
        }*/

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_ConnectToMulticast
    {
        public Result Result;
        //public Output_ConnectToMulticast();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableDeviceData
    {
        public Result Result;
        //public Output_DisableDeviceData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableMarkerData
    {
        public Result Result;
        //public Output_DisableMarkerData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableSegmentData
    {
        public Result Result;
        //public Output_DisableSegmentData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableUnlabeledMarkerData
    {
        public Result Result;
       // public Output_DisableUnlabeledMarkerData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableMarkerRayData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableCentroidData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableDebugData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableGreyscaleData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_Disconnect
    {
        public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_UpdateFrame
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_WaitForFrame
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableDeviceData
    {
        public Result Result;
        //public Output_EnableDeviceData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableMarkerData
    {
        public Result Result;
        //public Output_EnableMarkerData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableSegmentData
    {
        public Result Result;
       // public Output_EnableSegmentData();
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableLightweightSegmentData
    {
        public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_DisableLightweightSegmentData
    {
        public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableUnlabeledMarkerData
    {
        public Result Result;
       // public Output_EnableUnlabeledMarkerData();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableMarkerRayData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableDebugData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableCentroidData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_EnableGreyscaleData
    {
      public Result Result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetAxisMapping
    {
        public Direction XAxis;
        public Direction YAxis;
        public Direction ZAxis;
        //public Output_GetAxisMapping();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceCount
    {
        public Result Result;
        public uint DeviceCount;
        // public Output_GetDeviceCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string DeviceName;
        public DeviceType DeviceType;
        //public Output_GetDeviceName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceOutputCount
    {        
        public Result Result;
        public uint DeviceOutputCount;
        //public Output_GetDeviceOutputCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceOutputName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string DeviceOutputName;
        public Unit DeviceOutputUnit;
       // public Output_GetDeviceOutputName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceOutputComponentName
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string DeviceOutputName;
      public string DeviceOutputComponentName;
      public Unit DeviceOutputUnit;
      // public Output_GetDeviceOutputName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceOutputSubsamples
    {
        public Result Result;
        public uint DeviceOutputSubsamples;
        public bool Occluded;        
        //public Output_GetDeviceOutputSubsamples();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetDeviceOutputValue
    {        
        public Result Result;
        public double Value;
        public bool Occluded;
        //public Output_GetDeviceOutputValue();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetEyeTrackerCount
    {
        public Result Result; 
        public uint EyeTrackerCount;        
        //public Output_GetEyeTrackerCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetEyeTrackerGlobalGazeVector
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] GazeVector;
        public bool Occluded;
       // public Output_GetEyeTrackerGlobalGazeVector();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetEyeTrackerGlobalPosition
    {       
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Position;
        public bool Occluded;
        //public Output_GetEyeTrackerGlobalPosition();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetForcePlateCount
    {
        public Result Result;
        public uint ForcePlateCount;
        //public Output_GetForcePlateCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetForcePlateSubsamples
    {
        public Result Result; 
        public uint ForcePlateSubsamples;
        //public Output_GetForcePlateSubsamples();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrame
    {
        public Result Result;
        //public Output_GetFrame();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrameNumber
    {
        public Result Result; 
        public uint FrameNumber;
        //public Output_GetFrameNumber();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrameRate
    {
        public Result Result; 
        public double FrameRateHz;
        //public Output_GetFrameRate();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetGlobalCentreOfPressure
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] CentreOfPressure;
        //public Output_GetGlobalCentreOfPressure();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetGlobalForceVector
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] ForceVector;
        //public Output_GetGlobalForceVector();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetGlobalMomentVector
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] MomentVector;
        //public Output_GetGlobalMomentVector();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLatencySampleCount
    {
        public Result Result; 
        public uint Count;
        //public Output_GetLatencySampleCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLatencySampleName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        //public Output_GetLatencySampleName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLatencySampleValue
    {
        public Result Result;
        public double Value;
       // public Output_GetLatencySampleValue();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLatencyTotal
    {
        public Result Result;
        public double Total;
        //public Output_GetLatencyTotal();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerCount
    {
        public Result Result; 
        public uint MarkerCount;
        //public Output_GetMarkerCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerGlobalTranslation
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Translation;
        public bool Occluded;
        //public Output_GetMarkerGlobalTranslation();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string MarkerName;
        //public Output_GetMarkerName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerParentName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SegmentName;
        //public Output_GetMarkerParentName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentChildCount
    {
        public Result Result;
        public uint SegmentCount;
        //public Output_GetSegmentChildCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentChildName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SegmentName;
        //public Output_GetSegmentChildName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentCount
    {
        public Result Result;
        public uint SegmentCount;
        //public Output_GetSegmentCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentGlobalRotationEulerXYZ
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentGlobalRotationEulerXYZ();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentGlobalRotationHelical
    {       
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentGlobalRotationHelical();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentGlobalRotationMatrix
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentGlobalRotationMatrix();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentGlobalTranslation
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Translation;
        public bool Occluded;
        //public Output_GetSegmentGlobalTranslation();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentLocalRotationEulerXYZ
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentLocalRotationEulerXYZ();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentLocalRotationHelical
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentLocalRotationHelical();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentLocalRotationMatrix
    {       
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentLocalRotationMatrix();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentLocalRotationQuaternion
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public double[] Rotation;
        public bool Occluded;
        //public Output_GetSegmentLocalRotationQuaternion();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentLocalTranslation
    {        
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Translation;
        public bool Occluded;
        //public Output_GetSegmentLocalTranslation();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SegmentName;
        //public Output_GetSegmentName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentParentName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SegmentName;
        //public Output_GetSegmentParentName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticRotationEulerXYZ
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        //public Output_GetSegmentStaticRotationEulerXYZ();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticRotationHelical
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Rotation;
        //public Output_GetSegmentStaticRotationHelical();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticRotationMatrix
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] Rotation;
        //public Output_GetSegmentStaticRotationMatrix();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticRotationQuaternion
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public double[] Rotation;
        //public Output_GetSegmentStaticRotationQuaternion();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticTranslation
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Translation;
        //public Output_GetSegmentStaticTranslation();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSegmentStaticScale
    {
      public Result Result;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] Scale;
      //public Output_GetSegmentStaticScale();
    }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSubjectCount
    {
        public Result Result;
        public uint SubjectCount;
        //public Output_GetSubjectCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetSubjectRootSegmentName
    {
        public Result Result;
        [MarshalAs(UnmanagedType.LPStr)]
        public string SegmentName;
        //public Output_GetSubjectRootSegmentName();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetTimecode
    { 
        public Result Result;
        public uint Hours;
        public uint Minutes;        
        public uint Seconds;
        public uint Frames;
        public uint SubFrame;
        public bool FieldFlag;
        public TimecodeStandard Standard;       
        public uint SubFramesPerFrame;
        public uint UserBits;
        //public Output_GetTimecode();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetUnlabeledMarkerCount
    {
        public Result Result; 
        public uint MarkerCount;
        //public Output_GetUnlabeledMarkerCount();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetUnlabeledMarkerGlobalTranslation
    {
        public Result Result;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] Translation;
        public uint MarkerID;
        //public Output_GetUnlabeledMarkerGlobalTranslation();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsDeviceDataEnabled
    {
        public bool Enabled;
        //public Output_IsDeviceDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsMarkerDataEnabled
    {
        public bool Enabled;
        //public Output_IsMarkerDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsSegmentDataEnabled
    {
        public bool Enabled;
        //public Output_IsSegmentDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsMarkerRayDataEnabled
    {
      public bool Enabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsCentroidDataEnabled
    {
      public bool Enabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsDebugDataEnabled
    {
      public bool Enabled;
      //public Output_IsUnlabeledMarkerDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsGreyscaleDataEnabled
    {
      public bool Enabled;
      //public Output_IsUnlabeledMarkerDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsVideoDataEnabled
    {
      public bool Enabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_IsUnlabeledMarkerDataEnabled
    {
      public bool Enabled;
      //public Output_IsUnlabeledMarkerDataEnabled();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_SetApexDeviceFeedback
    {
        public Result Result;
        //public Output_SetApexDeviceFeedback();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_SetAxisMapping
    {
        public Result Result;
        //public Output_SetAxisMapping();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_SetStreamMode
    {
        public Result Result;
        //public Output_SetStreamMode();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_StartTransmittingMulticast
    {
        public Result Result;
       // public Output_StartTransmittingMulticast();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_StopTransmittingMulticast
    {
        public Result Result;
        //public Output_StopTransmittingMulticast();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCentroidCount
    {
      public Result Result;
      public uint CentroidCount;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerRayContributionCount
    {
    public Result Result;
    public uint RayContributionsCount;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetObjectQuality
    {
      public Result Result;
      public double Quality;
    } 
  
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraUserId
    {
      public Result Result;
      public uint CameraUserId;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCentroidPosition
    {
      public Result Result;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public double[] CentroidPosition;
      public double Radius;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraCount
    {
      public Result Result;
      public uint CameraCount;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraName
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string CameraName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetServerOrientation
    {
      public Result Result;
      public Result Orientation;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetMarkerRayContribution
    {
      public Result Result;
      public uint CameraID;
      public uint CentroidIndex;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrameRateCount
    {
      public Result Result;
      public uint Count;
    } 

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public class Output_GetGreyscaleBlob
    //{
    //  public Result Result;
    //  public uint * BlobLinePositionsX;
    //  public uint BlobLinePositionsXSize;
    //  public uint * BlobLinePositionsY;
    //  public uint BlobLinePositionsYSize;
    //  unsigned char * BlobLinePixelValues;
    //} 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraType
    {

      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string CameraType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraDisplayName
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string CameraDisplayName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraResolution
    {
      public Result Result;
      public uint ResolutionX;
      public uint ResolutionY;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrameRateName
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetFrameRateValue
    {
      public Result Result;
      public double       Value;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLabeledMarkerCount
    {
      public Result Result;
      public uint MarkerCount;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetIsVideoCamera
    {
      public Result Result;
      public bool IsVideoCamera;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCameraId
    {
      public Result Result;
      public uint CameraId;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetHardwareFrameNumber
    {
      public Result Result;
      public uint HardwareFrameNumber;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetLabeledMarkerGlobalTranslation
    {
      public Result Result;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] Translation;
      public uint MarkerID;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetCentroidWeight
    {
      public Result Result;
      public double Weight;
    } 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetGreyscaleBlobCount
    {
      public Result Result;
      public uint BlobCount;
    }


  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class Output_ClearSubjectFilter
  {
    public Result Result;
    //public Output_ClearSubjectFilter();
  }


  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class Output_AddToSubjectFilter
  {
    public Result Result;
    //public Output_AddToSubjectFilter();
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_SetTimingLogFile
    {
      public Result Result;
      //public Output_SetTimingLogFile();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_ConfigureWireless
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string Error;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetRetimedSubjectCount
    {
      public Result Result;
      public uint SubjectCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_GetRetimedSubjectName
    {
      public Result Result;
      [MarshalAs(UnmanagedType.LPStr)]
      public string Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_RetimedSegment
    {
      public bool Occluded;

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] Global_Position;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
      public double[] Global_Rotation;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] Local_Position;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
      public double[] Local_Rotation;


    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_RetimedSubject
    {
      public Result Result;

      [MarshalAs(UnmanagedType.LPStr)]
      public string Name;
      [MarshalAs(UnmanagedType.LPStr)]
      public string RootSegmentName;

      public uint Parent1_FrameNumber;
      public uint Parent2_FrameNumber;

      public void AddSegment(Output_RetimedSegment Segment)
      {
        m_Segments.Add(Segment);
      }

      public int GetSegmentCount()
      {
        return m_Segments.Count;
      }

      public bool GetSegment(int Index, out Output_RetimedSegment Segment)
      {
        if (Index < m_Segments.Count)
        {
          Segment = m_Segments[Index];
          return true;
        }

        Segment = new Output_RetimedSegment();
        return false;
      }

      private List<Output_RetimedSegment> m_Segments = new List<Output_RetimedSegment>();

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Output_RetimedSubjects
    {
      public Result Result;

      public Output_RetimedSubjects()
      {
      }

      public void AddSubject(Output_RetimedSubject Subject)
      {
        m_Subjects.Add(Subject);
      }

      public int GetSubjectCount()
      {
        return m_Subjects.Count;
      }

      public bool GetSubject(int Index, out Output_RetimedSubject Subject)
      {
        if (Index < m_Subjects.Count)
        {
          Subject = m_Subjects[Index];
          return true;
        }

        Subject = new Output_RetimedSubject();
        return false;
      }

      private List<Output_RetimedSubject> m_Subjects = new List<Output_RetimedSubject>();
    };
}