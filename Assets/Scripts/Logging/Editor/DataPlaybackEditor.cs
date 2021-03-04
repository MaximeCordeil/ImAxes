using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DataPlayback))]
public class DataPlaybackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("DO NOT USE DURING ACTUAL LIVE EXPERIMENTS! THIS IS ONLY FOR ANALYSIS PURPOSES!", MessageType.Error);

        DrawDefaultInspector();

        DataPlayback dataPlayback = (DataPlayback) target;

        if (GUILayout.Button("Start Replay"))
        {
            dataPlayback.StartLiveReplay();
        }
        if (GUILayout.Button("Pause Replay"))
        {
            dataPlayback.PauseLiveReplay();
        }
        if (GUILayout.Button("Restart Replay"))
        {
            dataPlayback.RestartLiveReplay();
        }
    }
}
#endif