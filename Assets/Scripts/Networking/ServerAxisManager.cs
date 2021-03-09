using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAxisManager : MonoBehaviour
{
    [Header("Server Axis References")]
    public ServerAxis BlueAxis1;
    public ServerAxis BlueAxis2;
    public ServerAxis BlackAxis1;
    public ServerAxis BlackAxis2;
    public ServerAxis RedAxis1;
    public ServerAxis RedAxis2;

    [Header("Server Axis COM Ports")]
    public int BlueAxis1_Port;
    public int BlueAxis2_Port;
    public int BlackAxis1_Port;
    public int BlackAxis2_Port;
    public int RedAxis1_Port;
    public int RedAxis2_Port;

    private void Awake()
    {
        SetCOMPorts();
    }

    private void OnValidate()
    {
        SetCOMPorts();
    }

    private void SetCOMPorts()
    {
        BlueAxis1.COM = "COM" + BlueAxis1_Port;
        BlueAxis2.COM = "COM" + BlueAxis2_Port;
        BlackAxis1.COM = "COM" + BlackAxis1_Port;
        BlackAxis2.COM = "COM" + BlackAxis2_Port;
        RedAxis1.COM = "COM" + RedAxis1_Port;
        RedAxis2.COM = "COM" + RedAxis2_Port;
    }
}
