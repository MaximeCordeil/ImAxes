using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using ViconDataStreamSDK.CSharp;


public class ViconDataStreamClient : MonoBehaviour
{
  [Tooltip("The hostname or ip address of the Datastream server.")]
  public string HostName = "localhost";
  
  [Tooltip("The Datastream port number. Typically 804 for the low latency stream and 801 if off-line review is required.")]
  public string Port = "801";
  
  [Tooltip("Enter a comma separated list of subjects that are required in the stream. If empty, all subjects will be transmitted.")]
  public string SubjectFilter;
  
  [Tooltip("Switches to the pre-fetch streaming mode, which will request new frames from the server as required while minimizing latency, rather than all frames being streamed. This can potentially help minimise the disruption of data delivery lags on the network. See the datastream documentation for more details of operation.")]
  public bool UsePreFetch = false;
  
  [Tooltip("Use retiming output mode. This can help to smooth out temporal artifacts due to differences between render and system frame rates.")]
  public bool IsRetimed = false;
  
  [Tooltip("Adds a fixed time offset to retimed output data. Only valid in retiming mode. Can be used to compensate for known render delays.")]
  public float Offset = 0;
  
  [Tooltip("Log timing information to a file.")]
  public bool Log = false;

  [Tooltip("Enable adapter settings to improve latency on wireless connections.")]
  public bool ConfigureWireless = true;

  private ViconDataStreamSDK.CSharp.Client m_Client;
  private ViconDataStreamSDK.CSharp.RetimingClient m_RetimingClient;

  private bool UseLightweightData = true;
  private bool GetFrameThread = true;
  private static bool bConnected = false;
  private bool bSubjectFilterSet = false;
  private bool bThreadRunning = false;
  Thread m_Thread;

  
public delegate void ConnectionCallback(bool i_bConnected);
  public static void OnConnected(bool i_bConnected)
  {
    bConnected = i_bConnected;
  }

  ConnectionCallback ConnectionHandler = OnConnected;

  private void SetupLog()
  {
    String DateTime = System.DateTime.Now.ToString();
    DateTime = DateTime.Replace(" ", "_");
    DateTime = DateTime.Replace("/", "_");
    DateTime = DateTime.Replace(":", "_");
    String ClientPathName = Application.dataPath + "/../Logs/" + DateTime + "_ClientLog.csv";
    String StreamPathName = Application.dataPath + "/../Logs/" + DateTime + "_StreamLog.csv";

    bool bLogSuccess = false;
    if ( IsRetimed )
    {
      bLogSuccess = m_RetimingClient.SetTimingLogFile(ClientPathName, StreamPathName).Result == Result.Success;
    }
    else
    {
      bLogSuccess = m_Client.SetTimingLogFile(ClientPathName, StreamPathName).Result == Result.Success;
    }

    if ( bLogSuccess )
    {
      print("Writing log to " + ClientPathName + " and " + StreamPathName);
    }
    else
    {
      print("Failed to create logs: " + ClientPathName + ", " + StreamPathName);
    }
  }

  void Start()
  {
    m_Client = new Client();
    m_RetimingClient = new RetimingClient();

    // If we're using the retimer, we don't want to use our own thread for getting frames.
    GetFrameThread = !IsRetimed;

    if (ConfigureWireless)
    {
      Output_ConfigureWireless WifiConfig = m_Client.ConfigureWireless();
      if (WifiConfig.Result != Result.Success)
      {
        print("Failed to configure wireless: " + WifiConfig.ToString());
      }
      else
      {
        print("Configured adapter for wireless settings");
      }
    }

    print("Starting...");
    Output_GetVersion OGV = m_Client.GetVersion();
    print("Using Datastream version " + OGV.Major + "." + OGV.Minor + "." + OGV.Point + "." + OGV.Revision );

    if (Log)
    {
      SetupLog();
    }

    m_Thread = new Thread ( ConnectClient );
    m_Thread.Start();
  }

  void OnValidate()
  {
    if (bConnected)
    {
      if (bThreadRunning)
      {
        bThreadRunning = false;
        m_Thread.Join();

        DisConnect();
        m_Thread = new Thread ( ConnectClient );
        m_Thread.Start();
      }
    }
  }
  void DisConnect()
  {
    if (m_RetimingClient.IsConnected().Connected)
    {
      m_RetimingClient.Disconnect();
    }
    if (m_Client.IsConnected().Connected)
    {
      m_Client.Disconnect();
    }
  }

  private void ConnectClient()
  {
    bThreadRunning = true;

    // We have to handle the multi-route syntax, which is of the form HostName1:Port;Hostname2:Port
    String CombinedHostnameString = "";
    String[] Hosts = HostName.Split( ';' );
    foreach( String Host in Hosts )
    {
      String TrimmedString = Host.Trim();
      String HostWithPort = null;

      // Check whether the hostname already contains a port and add if it doesn't
      if (TrimmedString.Contains(":"))
      {
        HostWithPort = TrimmedString;
      }
      else
      {
        HostWithPort = TrimmedString + ":" + Port;
      }

      if( !String.IsNullOrEmpty( CombinedHostnameString ) )
      {
        CombinedHostnameString += ";";
      }

      CombinedHostnameString += HostWithPort;
    }

    print("Connecting to " + CombinedHostnameString + "...");

    if (IsRetimed)
    {
      while ( bThreadRunning == true && !m_RetimingClient.IsConnected().Connected)
      {
        Output_Connect OC = m_RetimingClient.Connect(CombinedHostnameString);
        print("Connect result: " + OC.Result);

        System.Threading.Thread.Sleep(200);
      }

      print("Connected using retimed client.");

      if (UseLightweightData)
      {
        // Retiming client will have segment data enabled by default
        if( m_RetimingClient.EnableLightweightSegmentData().Result == Result.Success )
        {
          print("Using lightweight segment data");
        }
        else
        {
          print("Unable to use lightweight segment data: Using standard segment data");
        }
      }
      else
      {
        print("Using standard segment data");
      }

      // get a frame from the data stream so we can inspect the list of subjects
      SetAxisMapping(Direction.Forward, Direction.Left, Direction.Up);
      //SetAxisMapping(Direction.Right, Direction.Up, Direction.Backward);
      ConnectionHandler( true );

      bThreadRunning = false;
      return;
    }

    while ( bThreadRunning == true && !m_Client.IsConnected().Connected)
    {
      Output_Connect OC = m_Client.Connect(CombinedHostnameString);
      print("Connect result: " + OC.Result);

      System.Threading.Thread.Sleep(200);
    }
    
    if( UsePreFetch )
    {
      m_Client.SetStreamMode( StreamMode.ClientPullPreFetch );
      print("Using pre-fetch streaming mode");
    }
    else
    {
      m_Client.SetStreamMode( StreamMode.ServerPush );
    }
    
    // Get a frame first, to ensure we have received supported type data from the server before
    // trying to determine whether lightweight data can be used.
    GetNewFrame();
    
    if( UseLightweightData )
    { 
      if( m_Client.EnableLightweightSegmentData().Result != Result.Success )
      {
        print("Unable to use lightweight segment data: Using standard segment data");
        m_Client.EnableSegmentData();
      }
      else
      {
        print("Using lightweight segment data");
      }
    }
    else
    {
      print("Using standard segment data");
      m_Client.EnableSegmentData();
    }

    SetAxisMapping(Direction.Forward, Direction.Left, Direction.Up);
    //SetAxisMapping(Direction.Right, Direction.Up, Direction.Backward);
    ConnectionHandler( true );

    // Get frames in this separate thread if we've asked for it.
    while( GetFrameThread && bThreadRunning )
    {
      GetNewFrame();
    }
    
    bThreadRunning = false;
  }

  void LateUpdate()
  {   
    // Get frame on late update if we've not got a separate frame acquisition thread
    if( !GetFrameThread )
    {
      if (!bConnected)
      {
        return;
      }
      GetNewFrame();
    }
  }
    
  public Output_GetSegmentLocalRotationQuaternion GetSegmentRotation(string SubjectName, string SegmentName)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.GetSegmentLocalRotationQuaternion(SubjectName, SegmentName);
    }
    else
    {
      return m_Client.GetSegmentLocalRotationQuaternion(SubjectName, SegmentName);
    }

  }
  public Output_GetSegmentLocalTranslation GetSegmentTranslation(string SubjectName, string SegmentName)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.GetSegmentLocalTranslation(SubjectName, SegmentName);
    }
    else
    {
      return m_Client.GetSegmentLocalTranslation(SubjectName, SegmentName);
    }

  }
  public Output_GetSegmentStaticScale GetSegmentScale(string SubjectName, string SegmentName)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.GetSegmentStaticScale(SubjectName, SegmentName);
    }
    else
    {
      return m_Client.GetSegmentStaticScale(SubjectName, SegmentName);
    }

  }

  /// Returns the local translation for a bone, scaled according to its scale and the scale of the bones above it 
  /// in the heirarchy, apart from the root translation
  public Output_GetSegmentLocalTranslation GetScaledSegmentTranslation( string SubjectName, string SegmentName )
  {
    double[] OutputScale = new double[3];
    OutputScale[0] = OutputScale[1] = OutputScale[2] = 1.0;

    // Check first whether we have a parent, as we don't wish to scale the root node's position
    Output_GetSegmentParentName Parent = GetSegmentParentName(SubjectName, SegmentName);

    string CurrentSegmentName = SegmentName;
    if ( Parent.Result == Result.Success)
    {

      do
      {
        // We have a parent. First get our scale, and then iterate through the nodes above us
        Output_GetSegmentStaticScale Scale = GetSegmentScale(SubjectName, CurrentSegmentName);
        if (Scale.Result == Result.Success)
        {
          for (uint i = 0; i < 3; ++i)
          {
            if (Scale.Scale[i] != 0.0) OutputScale[i] = OutputScale[i] * Scale.Scale[i];
          }
        }

        Parent = GetSegmentParentName(SubjectName, CurrentSegmentName);
        if( Parent.Result == Result.Success )
        {
          CurrentSegmentName = Parent.SegmentName;
        }
      } while (Parent.Result == Result.Success);
    }

    Output_GetSegmentLocalTranslation Translation = GetSegmentTranslation(SubjectName, SegmentName);
    if( Translation.Result == Result.Success )
    {
      for (uint i = 0; i < 3; ++i)
      {
        Translation.Translation[i] = Translation.Translation[i] / OutputScale[i];
      }
    }
    return Translation;
  }

  public Output_GetSubjectRootSegmentName GetSubjectRootSegmentName(string SubjectName)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.GetSubjectRootSegmentName(SubjectName);
    }
    else
    {
      return m_Client.GetSubjectRootSegmentName(SubjectName);
    }

  }
  public Output_GetSegmentParentName GetSegmentParentName(string SubjectName, string SegmentName)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.GetSegmentParentName(SubjectName, SegmentName);
    }
    else
    {
      return m_Client.GetSegmentParentName(SubjectName, SegmentName);
    }

  }
  public Output_SetAxisMapping SetAxisMapping( Direction X, Direction Y, Direction Z)
  {
    if (IsRetimed)
    {
      return m_RetimingClient.SetAxisMapping(X, Y, Z);
    }
    else
    {
      return m_Client.SetAxisMapping(X, Y, Z);
    }
  }
  public void GetNewFrame()
  {
    if (IsRetimed)
    {
      m_RetimingClient.UpdateFrame(Offset);
    }
    else
    {
      m_Client.GetFrame();
    }
  UpdateSubjectFilter();
}
public uint GetFrameNumber()
  {
    if (IsRetimed)
    {
      return 0;
    }
    else
    {
      return m_Client.GetFrameNumber().FrameNumber;
    }
}

  private void OnDisable()
  {
    if (bThreadRunning)
    {
      bThreadRunning = false;
      m_Thread.Join();
    }

  }
  private void UpdateSubjectFilter()
  {
    if (!String.IsNullOrEmpty( SubjectFilter ) && !bSubjectFilterSet)
    {
      string[] Subjects = SubjectFilter.Split(',');
      foreach (string Subject in Subjects)
      {
        if (IsRetimed)
        {
          if( m_RetimingClient.AddToSubjectFilter(Subject.Trim()).Result == Result.Success )
          {
            bSubjectFilterSet = true;
          }
        }
        else
        {
          if( m_Client.AddToSubjectFilter(Subject.Trim()).Result == Result.Success )
          {
            bSubjectFilterSet = true;
          }
        }
      }
    }
  }
  void OnDestroy()
  {
    DisConnect();

    m_Client = null;
    m_RetimingClient = null;
  }
}

