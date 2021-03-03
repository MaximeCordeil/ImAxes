using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ViconDataStreamSDK.CSharp
{

  public class Client : IDisposable
  {
    private IntPtr mImpl;
    private const string VICON_C_DLL = "ViconDataStreamSDK_C";
    private const int MAX_STRING = 64; //all strings read from Vicon will be truncated to this length (includes '\0')
    public Client()
    {
      mImpl = Client_Create();
      if (mImpl == IntPtr.Zero)
        throw new System.InvalidOperationException("Vicon client could not be created");
    }

    #region Destructors
    public void Dispose()
    {
      Dispose(true);
    }
    protected virtual void Dispose(bool bDisposing)
    {
      if (this.mImpl != IntPtr.Zero)
      {
        // Call the DLL Export to dispose this class
        Client_Destroy(this.mImpl);
        this.mImpl = IntPtr.Zero;
      }

      if (bDisposing)
      {
        // No need to call the finalizer since we've now cleaned
        // up the unmanaged memory
        GC.SuppressFinalize(this);
      }
    }

    // This finalizer is called when Garbage collection occurs, but only if
    // the IDisposable.Dispose method wasn't already called.
    ~Client()
    {
      Dispose(false);
    }
    #endregion Destructors

    public Output_Connect Connect(string HostName)
    {
      Output_Connect outp = new Output_Connect();
      outp.Result = Client_Connect(mImpl, HostName);
      return outp;
    }
    public Output_GetVersion GetVersion()
    {
      Output_GetVersion outp = new Output_GetVersion();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);

      try
      {
        Client_GetVersion(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_IsConnected IsConnected()
    {
      Output_IsConnected outp = new Output_IsConnected();
      outp.Connected = Client_IsConnected(mImpl);
      return outp;


    }
    public Output_GetSubjectName GetSubjectName(uint SubjectIndex)
    {
      Output_GetSubjectName outp = new Output_GetSubjectName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetSubjectName(mImpl, SubjectIndex, MAX_STRING, ptr);
        outp.SubjectName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentGlobalRotationQuaternion GetSegmentGlobalRotationQuaternion(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalRotationQuaternion);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentGlobalRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationQuaternion)Marshal.PtrToStructure(ptr, cType);

      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_ConnectToMulticast ConnectToMulticast(string HostName, string MulticastIP)
    {
      Output_ConnectToMulticast outp = new Output_ConnectToMulticast();
      outp.Result = Client_ConnectToMulticast(mImpl, HostName, MulticastIP);
      return outp;
    }
    public Output_DisableDeviceData DisableDeviceData()
    {
      Output_DisableDeviceData outp = new Output_DisableDeviceData();
      outp.Result = Client_DisableDeviceData(mImpl);
      return outp;
    }
    public Output_DisableMarkerData DisableMarkerData()
    {
      Output_DisableMarkerData outp = new Output_DisableMarkerData();
      outp.Result = Client_DisableMarkerData(mImpl);
      return outp;
    }
    public Output_DisableSegmentData DisableSegmentData()
    {
      Output_DisableSegmentData outp = new Output_DisableSegmentData();
      outp.Result = Client_DisableSegmentData(mImpl);
      return outp;
    }
    public Output_DisableUnlabeledMarkerData DisableUnlabeledMarkerData()
    {
      Output_DisableUnlabeledMarkerData outp = new Output_DisableUnlabeledMarkerData();
      outp.Result = Client_DisableUnlabeledMarkerData(mImpl);
      return outp;
    }
    public Output_Disconnect Disconnect()
    {
      Output_Disconnect outp = new Output_Disconnect();
      outp.Result = Client_Disconnect(mImpl);
      return outp;
    }
    public Output_EnableDeviceData EnableDeviceData()
    {
      Output_EnableDeviceData outp = new Output_EnableDeviceData();
      outp.Result = Client_EnableDeviceData(mImpl);
      return outp;
    }
    public Output_EnableMarkerData EnableMarkerData()
    {
      Output_EnableMarkerData outp = new Output_EnableMarkerData();
      outp.Result = Client_EnableMarkerData(mImpl);
      return outp;
    }
    public Output_EnableSegmentData EnableSegmentData()
    {
      Output_EnableSegmentData outp = new Output_EnableSegmentData();
      outp.Result = Client_EnableSegmentData(mImpl);
      return outp;
    }
    public Output_EnableLightweightSegmentData EnableLightweightSegmentData()
    {
      Output_EnableLightweightSegmentData outp = new Output_EnableLightweightSegmentData();
      outp.Result = Client_EnableLightweightSegmentData(mImpl);
      return outp;
    }
    public Output_DisableLightweightSegmentData DisableLightweightSegmentData()
    {
      Output_DisableLightweightSegmentData outp = new Output_DisableLightweightSegmentData();
      outp.Result = Client_DisableLightweightSegmentData(mImpl);
      return outp;
    }
    public Output_EnableUnlabeledMarkerData EnableUnlabeledMarkerData()
    {
      Output_EnableUnlabeledMarkerData outp = new Output_EnableUnlabeledMarkerData();
      outp.Result = Client_EnableUnlabeledMarkerData(mImpl);
      return outp;
    }
    public Output_GetAxisMapping GetAxisMapping()
    {
      Output_GetAxisMapping outp = new Output_GetAxisMapping();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetAxisMapping(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetDeviceCount GetDeviceCount()
    {
      Output_GetDeviceCount outp = new Output_GetDeviceCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetDeviceCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetDeviceName GetDeviceName(uint DeviceIndex)
    {
      Output_GetDeviceName outp = new Output_GetDeviceName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        //DeviceType dType = new DeviceType();
        //DeviceType dType;
        outp.Result = Client_GetDeviceName(mImpl, DeviceIndex, MAX_STRING, ptr, ref outp.DeviceType);
        outp.DeviceName = Marshal.PtrToStringAnsi(ptr);
        //outp.DeviceType = dType;
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetDeviceOutputCount GetDeviceOutputCount(string DeviceName)
    {
      Output_GetDeviceOutputCount outp = new Output_GetDeviceOutputCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetDeviceOutputCount(mImpl, DeviceName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetDeviceOutputName GetDeviceOutputName(string DeviceName, uint DeviceOutputIndex)
    {
      Output_GetDeviceOutputName outp = new Output_GetDeviceOutputName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetDeviceOutputName(mImpl, DeviceName, DeviceOutputIndex, MAX_STRING, ptr, ref outp.DeviceOutputUnit);
        outp.DeviceOutputName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetDeviceOutputComponentName GetDeviceOutputComponentName(string DeviceName, uint DeviceOutputIndex)
    {
      Output_GetDeviceOutputComponentName outp = new Output_GetDeviceOutputComponentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      IntPtr ptr2 = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetDeviceOutputComponentName(mImpl, DeviceName, DeviceOutputIndex, MAX_STRING, ptr, MAX_STRING, ptr2, ref outp.DeviceOutputUnit);
        outp.DeviceOutputName = Marshal.PtrToStringAnsi(ptr);
        outp.DeviceOutputComponentName = Marshal.PtrToStringAnsi(ptr2);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetDeviceOutputSubsamples GetDeviceOutputSubsamples(string DeviceName, string DeviceOutputName)
    {
      Type cType = typeof(Output_GetDeviceOutputSubsamples);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputSubsamples(mImpl, DeviceName, DeviceOutputName, ptr);
        return (Output_GetDeviceOutputSubsamples)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetDeviceOutputSubsamples GetDeviceOutputSubsamples(string DeviceName, string DeviceOutputName, string DeviceOutputComponentName )
    {
      Type cType = typeof(Output_GetDeviceOutputSubsamples);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputComponentSubsamples(mImpl, DeviceName, DeviceOutputName, DeviceOutputComponentName, ptr);
        return (Output_GetDeviceOutputSubsamples)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetDeviceOutputValue GetDeviceOutputValue(string DeviceName, string DeviceOutputName)
    {
      Type cType = typeof(Output_GetDeviceOutputValue);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputValue(mImpl, DeviceName, DeviceOutputName, ptr);
        return (Output_GetDeviceOutputValue)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetDeviceOutputValue GetDeviceOutputValue(string DeviceName, string DeviceOutputName, string DeviceOutputComponentName)
    {
      Type cType = typeof(Output_GetDeviceOutputValue);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputComponentValue(mImpl, DeviceName, DeviceOutputName, DeviceOutputComponentName, ptr);
        return (Output_GetDeviceOutputValue)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetDeviceOutputValue GetDeviceOutputValue(string DeviceName, string DeviceOutputName, uint Subsample)
    {
      Type cType = typeof(Output_GetDeviceOutputValue);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputValueForSubsample(mImpl, DeviceName, DeviceOutputName, Subsample, ptr);
        return (Output_GetDeviceOutputValue)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetDeviceOutputValue GetDeviceOutputValue(string DeviceName, string DeviceOutputName, string DeviceOutputComponentName, uint Subsample)
    {
      Type cType = typeof(Output_GetDeviceOutputValue);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetDeviceOutputComponentValueForSubsample(mImpl, DeviceName, DeviceOutputName, DeviceOutputComponentName, Subsample, ptr);
        return (Output_GetDeviceOutputValue)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetEyeTrackerCount GetEyeTrackerCount()
    {
      Output_GetEyeTrackerCount outp = new Output_GetEyeTrackerCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetEyeTrackerCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetEyeTrackerGlobalGazeVector GetEyeTrackerGlobalGazeVector(uint EyeTrackerIndex)
    {
      Type cType = typeof(Output_GetEyeTrackerGlobalGazeVector);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetEyeTrackerGlobalGazeVector(mImpl, EyeTrackerIndex, ptr);
        return (Output_GetEyeTrackerGlobalGazeVector)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetEyeTrackerGlobalPosition GetEyeTrackerGlobalPosition(uint EyeTrackerIndex)
    {
      Type cType = typeof(Output_GetEyeTrackerGlobalPosition);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetEyeTrackerGlobalPosition(mImpl, EyeTrackerIndex, ptr);
        return (Output_GetEyeTrackerGlobalPosition)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetForcePlateCount GetForcePlateCount()
    {
      Output_GetForcePlateCount outp = new Output_GetForcePlateCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetForcePlateCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetForcePlateSubsamples GetForcePlateSubsamples(uint ForcePlateIndex)
    {
      Output_GetForcePlateSubsamples outp = new Output_GetForcePlateSubsamples();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetForcePlateSubsamples(mImpl, ForcePlateIndex, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetFrame GetFrame()
    {
      Output_GetFrame outp = new Output_GetFrame();
      outp.Result = Client_GetFrame(mImpl);
      return outp;
    }
    public Output_GetFrameNumber GetFrameNumber()
    {
      Output_GetFrameNumber outp = new Output_GetFrameNumber();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetFrameNumber(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetFrameRate GetFrameRate()
    {
      Output_GetFrameRate outp = new Output_GetFrameRate();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetFrameRate(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetGlobalCentreOfPressure GetGlobalCentreOfPressure(uint ForcePlateIndex)
    {
      Type cType = typeof(Output_GetGlobalCentreOfPressure);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalCentreOfPressure(mImpl, ForcePlateIndex, ptr);
        return (Output_GetGlobalCentreOfPressure)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetGlobalCentreOfPressure GetGlobalCentreOfPressure(uint ForcePlateIndex, uint Subsample)
    {
      Type cType = typeof(Output_GetGlobalCentreOfPressure);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalCentreOfPressureForSubsample(mImpl, ForcePlateIndex, Subsample, ptr);
        return (Output_GetGlobalCentreOfPressure)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetGlobalForceVector GetGlobalForceVector(uint ForcePlateIndex)
    {
      Type cType = typeof(Output_GetGlobalForceVector);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalForceVector(mImpl, ForcePlateIndex, ptr);
        return (Output_GetGlobalForceVector)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetGlobalForceVector GetGlobalForceVector(uint ForcePlateIndex, uint Subsample)
    {
      Type cType = typeof(Output_GetGlobalForceVector);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalForceVectorForSubsample(mImpl, ForcePlateIndex, Subsample, ptr);
        return (Output_GetGlobalForceVector)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetGlobalMomentVector GetGlobalMomentVector(uint ForcePlateIndex)
    {
      Type cType = typeof(Output_GetGlobalMomentVector);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalMomentVector(mImpl, ForcePlateIndex, ptr);
        return (Output_GetGlobalMomentVector)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetGlobalMomentVector GetGlobalMomentVector(uint ForcePlateIndex, uint Subsample)
    {
      Type cType = typeof(Output_GetGlobalMomentVector);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetGlobalMomentVectorForSubsample(mImpl, ForcePlateIndex, Subsample, ptr);
        return (Output_GetGlobalMomentVector)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetLatencySampleCount GetLatencySampleCount()
    {
      Output_GetLatencySampleCount outp = new Output_GetLatencySampleCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetLatencySampleCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetLatencySampleName GetLatencySampleName(uint LatencySampleIndex)
    {
      Output_GetLatencySampleName outp = new Output_GetLatencySampleName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetLatencySampleName(mImpl, LatencySampleIndex, MAX_STRING, ptr);
        outp.Name = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetLatencySampleValue GetLatencySampleValue(string LatencySampleName)
    {
      Output_GetLatencySampleValue outp = new Output_GetLatencySampleValue();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetLatencySampleValue(mImpl, LatencySampleName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetLatencyTotal GetLatencyTotal()
    {
      Output_GetLatencyTotal outp = new Output_GetLatencyTotal();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetLatencyTotal(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetMarkerCount GetMarkerCount(string SubjectName)
    {
      Output_GetMarkerCount outp = new Output_GetMarkerCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetMarkerCount(mImpl, SubjectName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetMarkerGlobalTranslation GetMarkerGlobalTranslation(string SubjectName, string MarkerName)
    {
      Type cType = typeof(Output_GetMarkerGlobalTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetMarkerGlobalTranslation(mImpl, SubjectName, MarkerName, ptr);
        return (Output_GetMarkerGlobalTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetMarkerName GetMarkerName(string SubjectName, uint MarkerIndex)
    {
      Output_GetMarkerName outp = new Output_GetMarkerName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetMarkerName(mImpl, SubjectName, MarkerIndex, MAX_STRING, ptr);
        outp.MarkerName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetMarkerParentName GetMarkerParentName(string SubjectName, string MarkerName)
    {
      Output_GetMarkerParentName outp = new Output_GetMarkerParentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetMarkerParentName(mImpl, SubjectName, MarkerName, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentChildCount GetSegmentChildCount(string SubjectName, string SegmentName)
    {
      Output_GetSegmentChildCount outp = new Output_GetSegmentChildCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetSegmentChildCount(mImpl, SubjectName, SegmentName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetSegmentChildName GetSegmentChildName(string SubjectName, string SegmentName, uint SegmentIndex)
    {
      Output_GetSegmentChildName outp = new Output_GetSegmentChildName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetSegmentChildName(mImpl, SubjectName, SegmentName, SegmentIndex, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentCount GetSegmentCount(string SubjectName)
    {
      Output_GetSegmentCount outp = new Output_GetSegmentCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetSegmentCount(mImpl, SubjectName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetSegmentGlobalRotationEulerXYZ GetSegmentGlobalRotationEulerXYZ(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalRotationEulerXYZ);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentGlobalRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationEulerXYZ)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentGlobalRotationHelical GetSegmentGlobalRotationHelical(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalRotationHelical);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentGlobalRotationHelical(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationHelical)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentGlobalRotationMatrix GetSegmentGlobalRotationMatrix(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalRotationMatrix);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentGlobalRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationMatrix)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetSegmentGlobalTranslation GetSegmentGlobalTranslation(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentGlobalTranslation(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentLocalRotationEulerXYZ GetSegmentLocalRotationEulerXYZ(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentLocalRotationEulerXYZ);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentLocalRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalRotationEulerXYZ)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentLocalRotationHelical GetSegmentLocalRotationHelical(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentLocalRotationHelical);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentLocalRotationHelical(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalRotationHelical)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentLocalRotationMatrix GetSegmentLocalRotationMatrix(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentLocalRotationMatrix);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentLocalRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalRotationMatrix)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentLocalRotationQuaternion GetSegmentLocalRotationQuaternion(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentLocalRotationQuaternion);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentLocalRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalRotationQuaternion)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentLocalTranslation GetSegmentLocalTranslation(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentLocalTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentLocalTranslation(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentName GetSegmentName(string SubjectName, uint SegmentIndex)
    {
      Output_GetSegmentName outp = new Output_GetSegmentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetSegmentName(mImpl, SubjectName, SegmentIndex, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentParentName GetSegmentParentName(string SubjectName, string SegmentName)
    {
      Output_GetSegmentParentName outp = new Output_GetSegmentParentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetSegmentParentName(mImpl, SubjectName, SegmentName, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticRotationEulerXYZ GetSegmentStaticRotationEulerXYZ(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticRotationEulerXYZ);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticRotationEulerXYZ)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticScale GetSegmentStaticScale(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticScale);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticScale(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticScale)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticRotationHelical GetSegmentStaticRotationHelical(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticRotationHelical);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticRotationHelical(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticRotationHelical)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticRotationMatrix GetSegmentStaticRotationMatrix(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticRotationMatrix);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticRotationMatrix)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticRotationQuaternion GetSegmentStaticRotationQuaternion(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticRotationQuaternion);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticRotationQuaternion)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSegmentStaticTranslation GetSegmentStaticTranslation(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentStaticTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetSegmentStaticTranslation(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSubjectCount GetSubjectCount()
    {
      Output_GetSubjectCount outp = new Output_GetSubjectCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetSubjectCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetSubjectRootSegmentName GetSubjectRootSegmentName(string SubjectName)
    {
      Output_GetSubjectRootSegmentName outp = new Output_GetSubjectRootSegmentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetSubjectRootSegmentName(mImpl, SubjectName, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetTimecode GetTimecode()
    {
      Type cType = typeof(Output_GetTimecode);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetTimecode(mImpl, ptr);
        return (Output_GetTimecode)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetUnlabeledMarkerCount GetUnlabeledMarkerCount()
    {
      Output_GetUnlabeledMarkerCount outp = new Output_GetUnlabeledMarkerCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetUnlabeledMarkerCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetUnlabeledMarkerGlobalTranslation GetUnlabeledMarkerGlobalTranslation(uint MarkerIndex)
    {
      Type cType = typeof(Output_GetUnlabeledMarkerGlobalTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetUnlabeledMarkerGlobalTranslation(mImpl, MarkerIndex, ptr);
        return (Output_GetUnlabeledMarkerGlobalTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_IsDeviceDataEnabled IsDeviceDataEnabled()
    {
      Output_IsDeviceDataEnabled outp = new Output_IsDeviceDataEnabled();
      outp.Enabled = Client_IsDeviceDataEnabled(mImpl);
      return outp;
    }
    public Output_IsMarkerDataEnabled IsMarkerDataEnabled()
    {
      Output_IsMarkerDataEnabled outp = new Output_IsMarkerDataEnabled();
      outp.Enabled = Client_IsMarkerDataEnabled(mImpl);
      return outp;
    }
    public Output_IsSegmentDataEnabled IsSegmentDataEnabled()
    {
      Output_IsSegmentDataEnabled outp = new Output_IsSegmentDataEnabled();
      outp.Enabled = Client_IsSegmentDataEnabled(mImpl);
      return outp;
    }
    public Output_IsSegmentDataEnabled IsLightweightSegmentDataEnabled()
    {
      Output_IsSegmentDataEnabled outp = new Output_IsSegmentDataEnabled();
      outp.Enabled = Client_IsLightweightSegmentDataEnabled(mImpl);
      return outp;
    }
    public Output_IsUnlabeledMarkerDataEnabled IsUnlabeledMarkerDataEnabled()
    {
      Output_IsUnlabeledMarkerDataEnabled outp = new Output_IsUnlabeledMarkerDataEnabled();
      outp.Enabled = Client_IsUnlabeledMarkerDataEnabled(mImpl);
      return outp;
    }
    public Output_SetApexDeviceFeedback SetApexDeviceFeedback(string DeviceName, bool bOn)
    {
      Output_SetApexDeviceFeedback outp = new Output_SetApexDeviceFeedback();
      outp.Result = Client_SetApexDeviceFeedback(mImpl, DeviceName, bOn);
      return outp;
    }
    public Output_SetAxisMapping SetAxisMapping(Direction i_XAxis, Direction i_YAxis, Direction i_ZAxis)
    {
      Output_SetAxisMapping outp = new Output_SetAxisMapping();
      outp.Result = Client_SetAxisMapping(mImpl, i_XAxis, i_YAxis, i_ZAxis);
      return outp;
    }
    public Output_SetStreamMode SetStreamMode(StreamMode Mode)
    {
      Output_SetStreamMode outp = new Output_SetStreamMode();
      outp.Result = Client_SetStreamMode(mImpl, Mode);
      return outp;
    }
    public Output_StartTransmittingMulticast StartTransmittingMulticast(string ServerIP, string MulticastIP)
    {
      Output_StartTransmittingMulticast outp = new Output_StartTransmittingMulticast();
      outp.Result = Client_StartTransmittingMulticast(mImpl, ServerIP, MulticastIP);
      return outp;
    }
    public Output_StopTransmittingMulticast StopTransmittingMulticast()
    {
      Output_StopTransmittingMulticast outp = new Output_StopTransmittingMulticast();
      outp.Result = Client_StopTransmittingMulticast(mImpl);
      return outp;
    }

    public Output_GetCameraUserId GetCameraUserId( string i_rCameraName )
    {
      Output_GetCameraUserId outp = new Output_GetCameraUserId();
      GCHandle gch = GCHandle.Alloc( outp, GCHandleType.Pinned );
      try
      {
        Client_GetCameraUserId(mImpl, i_rCameraName, gch.AddrOfPinnedObject() );
      }
      finally 
      {
        gch.Free();
      }
      return outp;
    }

    public Output_EnableGreyscaleData EnableGreyscaleData()
    {
      Output_EnableGreyscaleData outp = new Output_EnableGreyscaleData();
      outp.Result = Client_EnableGreyscaleData(mImpl);
      return outp;
    }

    public Output_DisableGreyscaleData DisableGreyscaleData()
    {
      Output_DisableGreyscaleData outp = new Output_DisableGreyscaleData();
      outp.Result = Client_DisableGreyscaleData(mImpl);
      return outp;
    }

    public Output_IsGreyscaleDataEnabled IsGreyscaleDataEnabled()
    {
      Output_IsGreyscaleDataEnabled outp = new Output_IsGreyscaleDataEnabled();
      outp.Enabled = Client_IsGreyscaleDataEnabled(mImpl);
      return outp;
    }

    public Output_IsVideoDataEnabled IsVideoDataEnabled()
    {
      Output_IsVideoDataEnabled outp = new Output_IsVideoDataEnabled();
      outp.Enabled = Client_IsVideoDataEnabled(mImpl);
      return outp;
    }

    public Output_GetFrameRateCount GetFrameRateCount()
    {
      Output_GetFrameRateCount outp = new Output_GetFrameRateCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetFrameRateCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally 
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetIsVideoCamera GetIsVideoCamera(string i_rCameraName)
    {
      Type cType = typeof(Output_GetIsVideoCamera);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetIsVideoCamera(mImpl, i_rCameraName, ptr);
        return (Output_GetIsVideoCamera)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetCameraCount GetCameraCount()
    {
      Output_GetCameraCount outp = new Output_GetCameraCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetCameraCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetLabeledMarkerCount GetLabeledMarkerCount()
    {
      Output_GetLabeledMarkerCount outp = new Output_GetLabeledMarkerCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetLabeledMarkerCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_IsMarkerRayDataEnabled IsMarkerRayDataEnabled()
    {
      Output_IsMarkerRayDataEnabled outp = new Output_IsMarkerRayDataEnabled();
      outp.Enabled = Client_IsMarkerRayDataEnabled(mImpl);
      return outp;
    }

    public Output_GetObjectQuality GetObjectQuality(string ObjectName)
    {
      Output_GetObjectQuality outp = new Output_GetObjectQuality();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetObjectQuality(mImpl, ObjectName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_IsCentroidDataEnabled IsCentroidDataEnabled()
    {
      Output_IsCentroidDataEnabled outp = new Output_IsCentroidDataEnabled();
      outp.Enabled = Client_IsCentroidDataEnabled(mImpl);
      return outp;
    }

    public Output_EnableDebugData EnableDebugData()
    {
      Output_EnableDebugData outp = new Output_EnableDebugData();
      outp.Result = Client_EnableDebugData(mImpl);
      return outp;
    }

    public Output_DisableDebugData DisableDebugData()
    {
      Output_DisableDebugData outp = new Output_DisableDebugData();
      outp.Result = Client_DisableDebugData(mImpl);
      return outp;
    }

    public Output_IsDebugDataEnabled IsDebugDataEnabled()
    {
      Output_IsDebugDataEnabled outp = new Output_IsDebugDataEnabled();
      outp.Enabled = Client_IsDebugDataEnabled(mImpl);
      return outp;
    }
    public void SetBufferSize( uint BufferSize )
    {
      Client_SetBufferSize(mImpl, BufferSize );
    }

    public Output_GetFrameRateValue GetFrameRateValue(string FrameRateName)
    {
      Output_GetFrameRateValue outp = new Output_GetFrameRateValue();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetFrameRateValue(mImpl, FrameRateName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetMarkerRayContribution GetMarkerRayContribution(string SubjectName, string MarkerName, uint MarkerRayContributionIndex)
    {
      Output_GetMarkerRayContribution outp = new Output_GetMarkerRayContribution();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetMarkerRayContribution(mImpl,SubjectName, MarkerName, MarkerRayContributionIndex, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetCentroidCount GetCentroidCount(string CameraName)
    {
      Output_GetCentroidCount outp = new Output_GetCentroidCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetCentroidCount(mImpl, CameraName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_EnableMarkerRayData EnableMarkerRayData()
    {
      Output_EnableMarkerRayData outp = new Output_EnableMarkerRayData();
      outp.Result = Client_EnableMarkerRayData(mImpl);
      return outp;
    }

    public Output_EnableCentroidData EnableCentroidData()
    {
      Output_EnableCentroidData outp = new Output_EnableCentroidData();
      outp.Result = Client_EnableCentroidData(mImpl);
      return outp;
    }

    public Output_DisableMarkerRayData DisableMarkerRayData()
    {
      Output_DisableMarkerRayData outp = new Output_DisableMarkerRayData();
      outp.Result = Client_DisableMarkerRayData(mImpl);
      return outp;
    }

    public Output_DisableCentroidData DisableCentroidData()
    {
      Output_DisableCentroidData outp = new Output_DisableCentroidData();
      outp.Result = Client_DisableCentroidData(mImpl);
      return outp;
    }

    public Output_GetCameraType GetCameraType(string CameraName)
    {
      Output_GetCameraType outp = new Output_GetCameraType();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetCameraType(mImpl, CameraName, MAX_STRING, ptr);
        outp.CameraType = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetServerOrientation GetServerOrientation()
    {
      Output_GetServerOrientation outp = new Output_GetServerOrientation();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetServerOrientation(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetCameraDisplayName GetCameraDisplayName(string CameraName)
    {
      Output_GetCameraDisplayName outp = new Output_GetCameraDisplayName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetCameraDisplayName(mImpl, CameraName, MAX_STRING, ptr);
        outp.CameraDisplayName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    
    public Output_GetCameraId GetCameraId(string CameraName)
    {
      Output_GetCameraId outp = new Output_GetCameraId();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetCameraId(mImpl, CameraName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetHardwareFrameNumber GetHardwareFrameNumber()
    {
      Output_GetHardwareFrameNumber outp = new Output_GetHardwareFrameNumber();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetHardwareFrameNumber(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetCentroidWeight GetCentroidWeight(string CameraName, uint CentroidIndex)
    {
      Output_GetCentroidWeight outp = new Output_GetCentroidWeight();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetCentroidWeight(mImpl, CameraName, CentroidIndex, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetFrameRateName GetFrameRateName(uint FrameRateIndex)
    {
      Output_GetFrameRateName outp = new Output_GetFrameRateName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetFrameRateName(mImpl, FrameRateIndex, MAX_STRING, ptr);
        outp.Name = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetCentroidPosition GetCentroidPosition(string CameraName, uint CentroidIndex)
    {
      Type cType = typeof(Output_GetCentroidPosition);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetCentroidPosition(mImpl, CameraName, CentroidIndex, ptr);
        return (Output_GetCentroidPosition)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetCameraName GetCameraName(uint CameraIndex)
    {
      Output_GetCameraName outp = new Output_GetCameraName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_GetCameraName(mImpl, CameraIndex, MAX_STRING, ptr);
        outp.CameraName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetLabeledMarkerGlobalTranslation GetLabeledMarkerGlobalTranslation(uint MarkerIndex)
    {
      Type cType = typeof(Output_GetLabeledMarkerGlobalTranslation);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        Client_GetLabeledMarkerGlobalTranslation(mImpl, MarkerIndex, ptr);
        return (Output_GetLabeledMarkerGlobalTranslation)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetCameraResolution GetCameraResolution(string CameraName)
    {
      Output_GetCameraResolution outp = new Output_GetCameraResolution();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetCameraResolution(mImpl, CameraName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_GetMarkerRayContributionCount GetMarkerRayContributionCount(string SubjectName, string MarkerName)
    {
      Output_GetMarkerRayContributionCount outp = new Output_GetMarkerRayContributionCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        Client_GetMarkerRayContributionCount(mImpl, SubjectName, MarkerName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;

    }

    public Output_ClearSubjectFilter ClearSubjectFilter()
    {
      Output_ClearSubjectFilter outp = new Output_ClearSubjectFilter();
      outp.Result = Client_ClearSubjectFilter(mImpl);
      return outp;
    }

    public Output_AddToSubjectFilter AddToSubjectFilter( string SubjectName )
    {
      Output_AddToSubjectFilter outp = new Output_AddToSubjectFilter();
      outp.Result = Client_AddToSubjectFilter(mImpl, SubjectName);
      return outp;
    }

    public Output_SetTimingLogFile SetTimingLogFile( string ClientLog, string StreamLog )
    {
      Output_SetTimingLogFile outp = new Output_SetTimingLogFile();
      outp.Result = Client_SetTimingLogFile(mImpl, ClientLog, StreamLog );
      return outp;
    }

    public Output_ConfigureWireless ConfigureWireless()
    {
      Output_ConfigureWireless outp = new Output_ConfigureWireless();

      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = Client_ConfigureWireless(mImpl, MAX_STRING, ptr);
        outp.Error = Marshal.PtrToStringAnsi(ptr);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
      return outp;
    }


    #region PInvokes
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern IntPtr Client_Create();

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_Destroy(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetVersion(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_Connect(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string HostName);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsConnected(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetSubjectName(IntPtr client, uint SubjectIndex, int sizeOfBuffer, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentGlobalRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                        [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_ConnectToMulticast(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string LocalIP,
                                                          [MarshalAs(UnmanagedType.LPStr)]string MulticastIP);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_Disconnect(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_StartTransmittingMulticast(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string ServerIP,
                                                                  [MarshalAs(UnmanagedType.LPStr)]string MulticastIP);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_StopTransmittingMulticast(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableSegmentData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableLightweightSegmentData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableLightweightSegmentData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableMarkerData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableUnlabeledMarkerData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableDeviceData(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableSegmentData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableMarkerData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableUnlabeledMarkerData(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableDeviceData(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsSegmentDataEnabled(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsLightweightSegmentDataEnabled(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsMarkerDataEnabled(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsUnlabeledMarkerDataEnabled(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsDeviceDataEnabled(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_SetStreamMode(IntPtr client, StreamMode Mode);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_SetApexDeviceFeedback(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rDeviceName, bool i_bOn);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_SetAxisMapping(IntPtr client, Direction XAxis, Direction YAxis, Direction ZAxis);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetAxisMapping(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetFrame(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetFrameNumber(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetTimecode(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetFrameRate(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetLatencySampleCount(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetLatencySampleName(IntPtr client, uint LatencySampleIndex, int sizeOfBuffer, IntPtr outstr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetLatencySampleValue(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string LatencySampleName,
                                                            IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetLatencyTotal(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSubjectCount(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetSubjectRootSegmentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                int sizeOfBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetSegmentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                      uint SegmentIndex, uint sizeOfBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentChildCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetSegmentChildName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName,
                                                    uint SegmentIndex, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetSegmentParentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                         [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentStaticScale(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentGlobalTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                  [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentGlobalRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentGlobalRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentGlobalRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentLocalTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                 [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentLocalRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentLocalRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentLocalRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                               [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetSegmentLocalRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetMarkerCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetMarkerName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                    uint MarkerIndex, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetMarkerParentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                   [MarshalAs(UnmanagedType.LPStr)]string MarkerName, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetMarkerGlobalTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                 [MarshalAs(UnmanagedType.LPStr)]string MarkerName, IntPtr outptr);


    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetUnlabeledMarkerCount(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetUnlabeledMarkerGlobalTranslation(IntPtr client, uint MarkerIndex, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceCount(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetDeviceName(IntPtr client, uint DeviceIndex, int sizeOfBuffer, IntPtr outstr, ref DeviceType dtype);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetDeviceOutputName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
                                                    uint DeviceOutputIndex, int sizeOfBuffer, IntPtr outstr, ref Unit dUnit);


    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputValue(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
                                                     [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputSubsamples(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
                                                               [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputValueForSubsample(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
                                                     [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName,
                                                      uint Subsample, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetDeviceOutputComponentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
      uint DeviceOutputIndex, int sizeOfOutputBuffer, IntPtr OutputOutstr, int sizeOfComponentBuffer, IntPtr ComponentOutstr, ref Unit DeviceOutputUnit);



    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputComponentValue(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
      [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName, [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputComponentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputComponentSubsamples(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
      [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName, [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputComponentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetDeviceOutputComponentValueForSubsample(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string DeviceName,
      [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputName, [MarshalAs(UnmanagedType.LPStr)]string DeviceOutputComponentName,
      uint Subsample, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetForcePlateCount(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalForceVector(IntPtr client, uint ForcePlateIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalMomentVector(IntPtr client, uint ForcePlateIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalCentreOfPressure(IntPtr client, uint ForcePlateIndex, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetForcePlateSubsamples(IntPtr client, uint ForcePlateIndex, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalForceVectorForSubsample(IntPtr client, uint ForcePlateIndex, uint Subsample, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalMomentVectorForSubsample(IntPtr client, uint ForcePlateIndex, uint Subsample, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGlobalCentreOfPressureForSubsample(IntPtr client, uint ForcePlateIndex, uint Subsample, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetEyeTrackerCount(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetEyeTrackerGlobalPosition(IntPtr client, uint EyeTrackerIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetEyeTrackerGlobalGazeVector(IntPtr client, uint EyeTrackerIndex, IntPtr outptr);
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCameraUserId( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableGreyscaleData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsGreyscaleDataEnabled( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsVideoDataEnabled( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetFrameRateCount( IntPtr client, IntPtr outptr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_SetCameraFilter(IntPtr client, UIntPtr i_rCameraIdsForCentroids, int i_numOfCentroidIds, UIntPtr i_rCameraIdsForBlobs, int i_numOfBlobIds);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetIsVideoCamera( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCameraCount(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetLabeledMarkerCount(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsMarkerRayDataEnabled( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetObjectQuality(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string ObjectName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGreyscaleBlobCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsCentroidDataEnabled( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableDebugData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetFrameRateValue(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string FrameRateName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetMarkerRayContribution( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName, [MarshalAs(UnmanagedType.LPStr)]string MarkerName, uint MarkerRayContributionIndex, IntPtr outptr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCentroidCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableMarkerRayData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetCameraType( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, int sizeOfBuffer, IntPtr outstr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetGreyscaleBlob(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, uint i_BlobIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetServerOrientation(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetCameraDisplayName( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, int sizeOfBuffer, IntPtr outstr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableGreyscaleData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCameraId(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetHardwareFrameNumber(IntPtr client, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableCentroidData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_IsDebugDataEnabled( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool Client_SetBufferSize( IntPtr client, uint bufferSize );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCentroidWeight(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, uint i_CentroidIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetFrameRateName( IntPtr client, uint FrameRateIndex, int sizeOfBuffer, IntPtr outstr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCentroidPosition(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, uint i_CentroidIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_GetCameraName( IntPtr client, uint i_CameraIndex, int sizeOfBuffer, IntPtr outstr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableMarkerRayData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetLabeledMarkerGlobalTranslation(IntPtr client, uint MarkerIndex, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetCameraResolution(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rCameraName, IntPtr outptr);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_DisableDebugData( IntPtr client );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void Client_GetMarkerRayContributionCount( IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName, [MarshalAs(UnmanagedType.LPStr)]string MarkerName, IntPtr outptr );
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_EnableCentroidData( IntPtr client );

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_ClearSubjectFilter(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_AddToSubjectFilter(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rSubjectName);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_SetTimingLogFile(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rClientLog, [MarshalAs(UnmanagedType.LPStr)]string i_rStreamLog );

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result Client_ConfigureWireless(IntPtr client, int sizeOffBuffer, IntPtr outstr );

    #endregion PInvokes
  }
}
