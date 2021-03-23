using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class DataPlayback : MonoBehaviour
{
    [Header("Data Files")]
    public TextAsset HeadTransformsData;
    public TextAsset AxesData;
    public TextAsset CameraTransformData;
    public VideoClip VideoData;

    [Header("Replay Objects")]
    public Transform[] HeadObjects;
    public ServerAxis[] AxesObjects;
    public Transform CameraObject;

    [Header("Replay Settings")]
    public VideoPlayer LiveVideo;
    public float VideoStartTimeOffset;
    public bool UseRealTime = true;
    [Range(0, 1)] public float TimeScrubber;
    public float CurrentTime = 0;

    private bool isLiveReplayRunning = false;
    private bool isLiveReplayPaused = false;
    private float prevTimeScrubber;

    private OneEuroFilter<Vector3> cameraTransformPositionFilter;
    private OneEuroFilter<Quaternion> cameraTransformRotationFilter;
    private List<OneEuroFilter<Vector3>> axesPositionFilters = new List<OneEuroFilter<Vector3>>();
    private List<OneEuroFilter<Quaternion>> axesRotationFilters = new List<OneEuroFilter<Quaternion>>();

    public void StartLiveReplay()
    {
        if (!isLiveReplayRunning)
        {
            isLiveReplayPaused = false;
            StartCoroutine(LiveReplay());
        }
        else if (isLiveReplayPaused)
        {
            isLiveReplayPaused = false;
        }
    }

    public IEnumerator LiveReplay()
    {
        isLiveReplayRunning = true;

        if (VideoData != null)
        {
            LiveVideo.clip = VideoData;
            LiveVideo.gameObject.SetActive(true);
            LiveVideo.Prepare();
            // Wait for video to be prepared
            if (!LiveVideo.isPrepared)
            {
                yield return null;
            }

            LiveVideo.skipOnDrop = true;
            LiveVideo.Play();
            LiveVideo.time = VideoStartTimeOffset;
        }

        foreach (ServerAxis axis in AxesObjects)
        {
            axis.dataPlaybackMode = true;
            axis.transform.GetChild(0).gameObject.SetActive(false);
            axesPositionFilters.Add(new OneEuroFilter<Vector3>(4));
            axesRotationFilters.Add(new OneEuroFilter<Quaternion>(4));
        }

        // Read and split data
        string[] headTransformsDataLines = HeadTransformsData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<float, List<string[]>> headTransformsData = new Dictionary<float, List<string[]>>();
        for (int i = 1; i < headTransformsDataLines.Length; i++)
        {
            string[] lines = headTransformsDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!headTransformsData.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                headTransformsData[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        string[] axesDataLines = AxesData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<float, List<string[]>> axesData = new Dictionary<float, List<string[]>>();
        for (int i = 1; i < axesDataLines.Length; i++)
        {
            string[] lines = axesDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            List<string[]> timestampLines;
            if (!axesData.TryGetValue(timestamp, out timestampLines))
            {
                timestampLines = new List<string[]>();
                axesData[timestamp] = timestampLines;
            }

            timestampLines.Add(lines);
        }

        string[] cameraTransformDataLines = CameraTransformData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<float, string[]> cameraTransformData = new Dictionary<float, string[]>();
        for (int i = 1; i < cameraTransformDataLines.Length; i++)
        {
            string[] lines = cameraTransformDataLines[i].Split('\t');
            float timestamp = float.Parse(lines[0]);

            cameraTransformData[timestamp] = lines;
        }

        float[] timestamps = headTransformsData.Keys.Distinct().OrderBy(x => x).ToArray();
        float totalTime = headTransformsData.Keys.Max();

        cameraTransformPositionFilter = new OneEuroFilter<Vector3>(6);
        cameraTransformRotationFilter = new OneEuroFilter<Quaternion>(6);

        for (int i = 0; i < timestamps.Length; i += (isLiveReplayPaused) ? 0 : 1)
        {
            if (TimeScrubber != prevTimeScrubber)
            {
                // Find the nearest timestamp that matches the new time
                float newTime = TimeScrubber * totalTime;
                float newTimestamp = timestamps.Aggregate((x, y) => Math.Abs(x - newTime) < Math.Abs(y - newTime) ? x : y);
                // Set this timestamp as our new video time, and set i as its index
                i = Array.IndexOf(timestamps, newTimestamp);
                if (LiveVideo.clip != null)
                    LiveVideo.time = newTimestamp + VideoStartTimeOffset;

                prevTimeScrubber = TimeScrubber;
            }

            if (isLiveReplayPaused)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            else if (LiveVideo.isPaused)
            {
                LiveVideo.Play();
            }

            float currentTime = timestamps[i];
            
            if (UseRealTime && LiveVideo.clip != null)
            {
                // If the dataset's timestamp is larger than our video timestamp, skip a frame
                if (currentTime > (LiveVideo.time - VideoStartTimeOffset))
                {
                    i--;
                    yield return null;
                    continue;
                }
            }

            CurrentTime = currentTime;
            TimeScrubber = currentTime / totalTime;
            prevTimeScrubber = TimeScrubber;

            // Set the positions and rotations of the heads
            for (int j = 0; j < HeadObjects.Length; j++)
            {
                string[] headTransformsLine = headTransformsData[CurrentTime][j];

                Transform headTransform = HeadObjects[int.Parse(headTransformsLine[1])];

                headTransform.position = new Vector3(float.Parse(headTransformsLine[2]), float.Parse(headTransformsLine[3]), float.Parse(headTransformsLine[4]));
                headTransform.rotation = new Quaternion(float.Parse(headTransformsLine[5]), float.Parse(headTransformsLine[6]), float.Parse(headTransformsLine[7]), float.Parse(headTransformsLine[8]));
            }


            // Now that we've found it, set the positions, rotations, and properties of the axes
            for (int j = 0; j < AxesObjects.Length; j++)
            {
                string[] axesLine = axesData[CurrentTime][j];

                int axisIdx = int.Parse(axesLine[1]);
                ServerAxis axis = AxesObjects[axisIdx];

                // Check if the axis was running while logging. It returns everything as 0 if it does. In this case, ignore the line
                if (int.Parse(axesLine[9]) == 0 && float.Parse(axesLine[10]) == 0 && float.Parse(axesLine[11]) == 0)
                    continue;

                Vector3 newPosition = new Vector3(float.Parse(axesLine[2]), float.Parse(axesLine[3]), float.Parse(axesLine[4]));
                Quaternion newRotation = new Quaternion(float.Parse(axesLine[5]), float.Parse(axesLine[6]), float.Parse(axesLine[7]), float.Parse(axesLine[8]));

                axis.transform.position = axesPositionFilters[axisIdx].Filter<Vector3>(newPosition);
                axis.transform.rotation = axesRotationFilters[axisIdx].Filter<Quaternion>(newRotation);
                axis.dimensionIdx = int.Parse(axesLine[9]);
                axis.minFilter = float.Parse(axesLine[10]);
                axis.maxFilter = float.Parse(axesLine[11]);
                axis.infoboxToggle = bool.Parse(axesLine[12]);
                axis.infoboxPosition = float.Parse(axesLine[13]);
            }

            // Set the position and rotation of the external camera
            string[] cameraTransformLine = cameraTransformData[CurrentTime];
            Vector3 newCameraPosition = new Vector3(float.Parse(cameraTransformLine[1]), float.Parse(cameraTransformLine[2]), float.Parse(cameraTransformLine[3]));
            Quaternion newCameraRotation = new Quaternion(float.Parse(cameraTransformLine[4]), float.Parse(cameraTransformLine[5]), float.Parse(cameraTransformLine[6]), float.Parse(cameraTransformLine[7]));;
            // Filter camera position and rotation since it jitters in the Vicon
            CameraObject.position = cameraTransformPositionFilter.Filter<Vector3>(newCameraPosition);
            CameraObject.rotation = cameraTransformRotationFilter.Filter<Quaternion>(newCameraRotation);

            yield return null;
        }

        isLiveReplayRunning = false;
        isLiveReplayPaused = false;
        
        foreach (ServerAxis axis in AxesObjects)
        {
            axis.dataPlaybackMode = false;
            axis.transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    public void PauseLiveReplay()
    {
        LiveVideo.Pause();
        isLiveReplayPaused = true;
    }

    public void RestartLiveReplay()
    {
        if (isLiveReplayRunning)
        {
            StopCoroutine(LiveReplay());
            isLiveReplayRunning = false;
        }

        StartLiveReplay();
    }
}
