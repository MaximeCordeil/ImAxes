using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ServerAxisManager))]
public class ServerAxisManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ServerAxisManager serverAxisManager = (ServerAxisManager) target;

        EditorGUILayout.LabelField("Serial Port Force Reconnections", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Pressing these may interrupt the user!", MessageType.Warning);

        if (GUILayout.Button("Reconnect BlueAxis1"))
        {
            if (serverAxisManager.BlueAxis1 != null)
                serverAxisManager.BlueAxis1.ResetSerialPort();
        }
        if (GUILayout.Button("Reconnect BlueAxis2"))
        {
            if (serverAxisManager.BlueAxis2 != null)
                serverAxisManager.BlueAxis2.ResetSerialPort();
        }
        if (GUILayout.Button("Reconnect BlackAxis1"))
        {
            if (serverAxisManager.BlackAxis1 != null)
                serverAxisManager.BlackAxis1.ResetSerialPort();
        }
        if (GUILayout.Button("Reconnect BlackAxis2"))
        {
            if (serverAxisManager.BlackAxis2 != null)
                serverAxisManager.BlackAxis2.ResetSerialPort();
        }
        if (GUILayout.Button("Reconnect RedAxis1"))
        {
            if (serverAxisManager.RedAxis1 != null)
                serverAxisManager.RedAxis1.ResetSerialPort();
        }
        if (GUILayout.Button("Reconnect RedAxis2"))
        {
            if (serverAxisManager.RedAxis2 != null)
                serverAxisManager.RedAxis2.ResetSerialPort();
        }
    }
}
#endif