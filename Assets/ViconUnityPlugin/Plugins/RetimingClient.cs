using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ViconDataStreamSDK.CSharp
{

  public class RetimingClient : IDisposable
  {
    private IntPtr mImpl;
    private const string VICON_C_DLL = "ViconDataStreamSDK_C";
    private const int MAX_STRING = 64; //all strings read from Vicon will be truncated to this length (includes '\0')

    public RetimingClient()
    {
      mImpl = RetimingClient_Create();
      if (mImpl == IntPtr.Zero)
        throw new System.InvalidOperationException("Vicon Retiming client could not be created");
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
        RetimingClient_Destroy(this.mImpl);
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
    ~RetimingClient()
    {
      Dispose(false);
    }
    #endregion Destructors

    public Output_GetVersion GetVersion()
    {
      Output_GetVersion outp = new Output_GetVersion();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);

      try
      {
        RetimingClient_GetVersion(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }

    public Output_Connect Connect(string HostName)
    {
      Output_Connect outp = new Output_Connect();
      outp.Result = RetimingClient_Connect(mImpl, HostName);
      return outp;
    }

    public Output_Connect Connect(string HostName, double FrameRate)
    {
      Output_Connect outp = new Output_Connect();
      outp.Result = RetimingClient_ConnectAndStart(mImpl, HostName, FrameRate);
      return outp;
    }

    public Output_IsConnected IsConnected()
    {
      Output_IsConnected outp = new Output_IsConnected();
      outp.Connected = RetimingClient_IsConnected(mImpl);
      return outp;
    }

    public Output_Disconnect Disconnect()
    {
      Output_Disconnect outp = new Output_Disconnect();
      outp.Result = RetimingClient_Disconnect(mImpl);
      return outp;
    }

    public Output_UpdateFrame UpdateFrame()
    {
      Output_UpdateFrame outp = new Output_UpdateFrame();
      outp.Result = RetimingClient_UpdateFrame(mImpl);
      return outp;
    }

    public Output_UpdateFrame UpdateFrame(double Offset)
    {
      Output_UpdateFrame outp = new Output_UpdateFrame();
      outp.Result = RetimingClient_UpdateFrameOffset(mImpl, Offset);
      return outp;
    }

    public Output_WaitForFrame WaitForFrame()
    {
      Output_WaitForFrame outp = new Output_WaitForFrame();
      outp.Result = RetimingClient_WaitForFrame(mImpl);
      return outp;
    }
    public Output_EnableLightweightSegmentData EnableLightweightSegmentData()
    {
      Output_EnableLightweightSegmentData outp = new Output_EnableLightweightSegmentData();
      outp.Result = RetimingClient_EnableLightweightSegmentData(mImpl);
      return outp;
    }
    public Output_DisableLightweightSegmentData DisableLightweightSegmentData()
    {
      Output_DisableLightweightSegmentData outp = new Output_DisableLightweightSegmentData();
      outp.Result = RetimingClient_DisableLightweightSegmentData(mImpl);
      return outp;
    }
    public Output_GetSubjectCount GetSubjectCount()
    {
      Output_GetSubjectCount outp = new Output_GetSubjectCount();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        RetimingClient_GetSubjectCount(mImpl, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetSubjectName GetSubjectName(uint SubjectIndex)
    {
      Output_GetSubjectName outp = new Output_GetSubjectName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = RetimingClient_GetSubjectName(mImpl, SubjectIndex, MAX_STRING, ptr);
        outp.SubjectName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public Output_GetSubjectRootSegmentName GetSubjectRootSegmentName(string SubjectName)
    {
      Output_GetSubjectRootSegmentName outp = new Output_GetSubjectRootSegmentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = RetimingClient_GetSubjectRootSegmentName(mImpl, SubjectName, MAX_STRING, ptr);
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
        RetimingClient_GetSegmentCount(mImpl, SubjectName, gch.AddrOfPinnedObject());
      }
      finally
      {
        gch.Free();
      }
      return outp;
    }
    public Output_GetSegmentName GetSegmentName(string SubjectName, uint SegmentIndex)
    {
      Output_GetSegmentName outp = new Output_GetSegmentName();
      IntPtr ptr = Marshal.AllocHGlobal(MAX_STRING);
      try
      {
        outp.Result = RetimingClient_GetSegmentName(mImpl, SubjectName, SegmentIndex, MAX_STRING, ptr);
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
        RetimingClient_GetSegmentChildCount(mImpl, SubjectName, SegmentName, gch.AddrOfPinnedObject());
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
        outp.Result = RetimingClient_GetSegmentChildName(mImpl, SubjectName, SegmentName, SegmentIndex, MAX_STRING, ptr);
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
        outp.Result = RetimingClient_GetSegmentParentName(mImpl, SubjectName, SegmentName, MAX_STRING, ptr);
        outp.SegmentName = Marshal.PtrToStringAnsi(ptr);
        return outp;
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    public Output_GetSegmentGlobalRotationEulerXYZ GetSegmentGlobalRotationEulerXYZ(string SubjectName, string SegmentName)
    {
      Type cType = typeof(Output_GetSegmentGlobalRotationEulerXYZ);
      IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cType));
      try
      {
        RetimingClient_GetSegmentGlobalRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentGlobalRotationHelical(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentGlobalRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationMatrix)Marshal.PtrToStructure(ptr, cType);
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
        RetimingClient_GetSegmentGlobalRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentGlobalRotationQuaternion)Marshal.PtrToStructure(ptr, cType);

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
        RetimingClient_GetSegmentGlobalTranslation(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentLocalRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentLocalRotationHelical(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentLocalRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentLocalRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentLocalTranslation(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentLocalTranslation)Marshal.PtrToStructure(ptr, cType);
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
        RetimingClient_GetSegmentStaticRotationEulerXYZ(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticRotationEulerXYZ)Marshal.PtrToStructure(ptr, cType);
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
        RetimingClient_GetSegmentStaticRotationHelical(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentStaticRotationMatrix(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentStaticRotationQuaternion(mImpl, SubjectName, SegmentName, ptr);
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
        RetimingClient_GetSegmentStaticTranslation(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticTranslation)Marshal.PtrToStructure(ptr, cType);
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
        RetimingClient_GetSegmentStaticScale(mImpl, SubjectName, SegmentName, ptr);
        return (Output_GetSegmentStaticScale)Marshal.PtrToStructure(ptr, cType);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }
    public void SetOutputLatency( double OutputLatency )
    {
      RetimingClient_SetOutputLatency( mImpl, OutputLatency );
    }

    public double OutputLatency()
    {
      return RetimingClient_OutputLatency( mImpl );
    }

    public void SetMaximumPrediction( IntPtr client, double MaxPrediction )
    {
      RetimingClient_SetMaximumPrediction( mImpl, MaxPrediction );
    }

    public double MaximumPrediction()
    {
      return RetimingClient_MaximumPrediction( mImpl );
    }

    public Output_SetAxisMapping SetAxisMapping(Direction i_XAxis, Direction i_YAxis, Direction i_ZAxis)
    {
      Output_SetAxisMapping outp = new Output_SetAxisMapping();
      outp.Result = RetimingClient_SetAxisMapping(mImpl, i_XAxis, i_YAxis, i_ZAxis);
      return outp;
    }

    public Output_GetAxisMapping GetAxisMapping()
    {
      Output_GetAxisMapping outp = new Output_GetAxisMapping();
      GCHandle gch = GCHandle.Alloc(outp, GCHandleType.Pinned);
      try
      {
        RetimingClient_GetAxisMapping(mImpl, gch.AddrOfPinnedObject());
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
      outp.Result = RetimingClient_ClearSubjectFilter(mImpl);
      return outp;
    }

    public Output_AddToSubjectFilter AddToSubjectFilter(string SubjectName)
    {
      Output_AddToSubjectFilter outp = new Output_AddToSubjectFilter();
      outp.Result = RetimingClient_AddToSubjectFilter(mImpl, SubjectName);
      return outp;
    }

    public Output_SetTimingLogFile SetTimingLogFile(string ClientLog, string StreamLog )
    {
      Output_SetTimingLogFile outp = new Output_SetTimingLogFile();
      outp.Result = RetimingClient_SetTimingLogFile(mImpl, ClientLog, StreamLog );
      return outp;
    }

    #region PInvokes
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern IntPtr RetimingClient_Create();

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern  void RetimingClient_Destroy( IntPtr client );

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetVersion(IntPtr client, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_ConnectAndStart(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string HostName, double FrameRate );

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_Connect(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string HostName);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern bool RetimingClient_IsConnected(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_Disconnect(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_UpdateFrame(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_UpdateFrameOffset(IntPtr client, double Offset);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_WaitForFrame(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_EnableLightweightSegmentData(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_DisableLightweightSegmentData(IntPtr client);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_SetAxisMapping(IntPtr client, Direction XAxis, Direction YAxis, Direction ZAxis);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetAxisMapping(IntPtr client, IntPtr outptr);
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSubjectCount(IntPtr client, IntPtr outptr);
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_GetSubjectName(IntPtr client, uint SubjectIndex, int sizeOfBuffer, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_GetSubjectRootSegmentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                int sizeOfBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_GetSegmentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                      uint SegmentIndex, uint sizeOfBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentChildCount(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_GetSegmentChildName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName,
                                                    uint SegmentIndex, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_GetSegmentParentName(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, int sizeOffBuffer, IntPtr outstr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                         [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentStaticScale(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentGlobalTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                  [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentGlobalRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentGlobalRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentGlobalRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                               [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentGlobalRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                       [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentLocalTranslation(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                 [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentLocalRotationHelical(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                     [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentLocalRotationMatrix(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                    [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentLocalRotationQuaternion(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                               [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern void RetimingClient_GetSegmentLocalRotationEulerXYZ(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string SubjectName,
                                                                      [MarshalAs(UnmanagedType.LPStr)]string SegmentName, IntPtr outptr);

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern  void RetimingClient_SetOutputLatency( IntPtr client, double i_OutputLatency );
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern  double RetimingClient_OutputLatency( IntPtr client );

    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern  void RetimingClient_SetMaximumPrediction( IntPtr client, double i_MaxPrediction );
    
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern double RetimingClient_MaximumPrediction(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_ClearSubjectFilter(IntPtr client);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_AddToSubjectFilter(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rSubjectName);
    [DllImport(VICON_C_DLL, CallingConvention = CallingConvention.Cdecl)]
    static private extern Result RetimingClient_SetTimingLogFile(IntPtr client, [MarshalAs(UnmanagedType.LPStr)]string i_rClientLog, [MarshalAs(UnmanagedType.LPStr)]string i_rStreamLog);

    #endregion PInvokes
  }
}
