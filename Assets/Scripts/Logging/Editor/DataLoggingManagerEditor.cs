using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DataLoggingManager))]
public class DataLoggingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataLoggingManager dataLogger = (DataLoggingManager) target;

        if (!dataLogger.IsLogging)
        {
            if (GUILayout.Button("Start Logging"))
            {
                dataLogger.StartLogging();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Currently logging Group " + dataLogger.GroupID + "!", MessageType.Info);
            if (GUILayout.Button("Stop Logging"))
            {
                dataLogger.StopLogging();
            }
        }
    }
}
#endif