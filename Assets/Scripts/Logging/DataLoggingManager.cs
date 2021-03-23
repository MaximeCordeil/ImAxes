using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLoggingManager : MonoBehaviour
{
    [Header("Logging Settings")]
    public string FilePath;
    public string GroupID;
    public float TimeBetweenLogs = 0.05f;

    [Header("Logged Objects")]
    public List<Transform> HeadTransforms;
    public List<ServerAxis> Axes;
    public Transform CameraTransform;

    [HideInInspector]
    public bool IsLogging = false;

    private StreamWriter headTransformsStreamWriter;
    private StreamWriter axesStreamWriter;
    private StreamWriter cameraTransformStreamWriter;
    private float startTime;
    private float timer = 0f;
    private const string format = "F4";

    private void Start()
    {
        if (!FilePath.EndsWith("/"))
            FilePath += "/";
    }

    public void StartLogging()
    {
        // Head transforms
        string path = string.Format("{0}G{1}_HeadTransforms.txt", FilePath, GroupID);
        headTransformsStreamWriter = new StreamWriter(path, true);
        headTransformsStreamWriter.WriteLine("Timestamp\tID\tPosition.x\tPosition.y\tPosition.z\tRotation.x\tRotation.y\tRotation.z\tRotation.w");

        // Axes transforms and properties
        path = string.Format("{0}G{1}_Axes.txt", FilePath, GroupID);
        axesStreamWriter = new StreamWriter(path, true);
        axesStreamWriter.WriteLine("Timestamp\tID\tPosition.x\tPosition.y\tPosition.z\tRotation.x\tRotation.y\tRotation.z\tRotation.w\tDimensionIdx\tMinFilter\tMaxFilter\tInfoboxToggle\tInfoboxPosition");

        // Exteral camera transform
        path = string.Format("{0}G{1}_CameraTransform.txt", FilePath, GroupID);
        cameraTransformStreamWriter = new StreamWriter(path, true);
        cameraTransformStreamWriter.WriteLine("Timestamp\tPosition.x\tPosition.y\tPosition.z\tRotation.x\tRotation.y\tRotation.z\tRotation.w");

        IsLogging = true;
        startTime = Time.time;

        Debug.Log("Logging Started");
    }

    public void Update()
    {
        if (IsLogging)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= TimeBetweenLogs)
            {
                timer = 0;
                LogData();
            }
        }
    }

    public void StopLogging()
    {
        IsLogging = false;
        timer = 0;

        headTransformsStreamWriter.Close();
        axesStreamWriter.Close();
        cameraTransformStreamWriter.Close();

        Debug.Log("Logging Stopped");
    }

    private void LogData()
    {
        float timestamp = Time.unscaledTime - startTime;

        for (int i = 0; i < HeadTransforms.Count; i++)
        {
            Transform head = HeadTransforms[i];

            headTransformsStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                timestamp,
                i,
                head.position.x.ToString(format),
                head.position.y.ToString(format),
                head.position.z.ToString(format),
                head.rotation.x.ToString(format),
                head.rotation.y.ToString(format),
                head.rotation.z.ToString(format),
                head.rotation.w.ToString(format)
            );
        }

        foreach (ServerAxis axis in Axes)
        {
            axesStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}",
                timestamp,
                axis.serverAxisIndex,
                axis.transform.position.x.ToString(format),
                axis.transform.position.y.ToString(format),
                axis.transform.position.z.ToString(format),
                axis.transform.rotation.x.ToString(format),
                axis.transform.rotation.y.ToString(format),
                axis.transform.rotation.z.ToString(format),
                axis.transform.rotation.w.ToString(format),
                axis.fixedDimensionMode ? axis.fixedDimensionIdx : axis.dimensionIdx,
                axis.minFilter.ToString(format),
                axis.maxFilter.ToString(format),
                axis.infoboxToggle.ToString(),
                axis.infoboxPosition.ToString(format)
            );
        }

        cameraTransformStreamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
            timestamp,
            CameraTransform.position.x.ToString(format),
            CameraTransform.position.y.ToString(format),
            CameraTransform.position.z.ToString(format),
            CameraTransform.rotation.x.ToString(format),
            CameraTransform.rotation.y.ToString(format),
            CameraTransform.rotation.z.ToString(format),
            CameraTransform.rotation.w.ToString(format)
        );

        headTransformsStreamWriter.Flush();
        axesStreamWriter.Flush();
        cameraTransformStreamWriter.Flush();
    }

    public void OnApplicationQuit()
    {
        if (IsLogging)
            StopLogging();
    }
}
