using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DataPlayback : MonoBehaviour
{
    [Header("Data Files")]
    public TextAsset HeadTransformsData;
    public TextAsset AxesData;
    public TextAsset CameraTransformData;

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

        if (LiveVideo.clip != null)
        {
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
        }

        // Read and split data
        string[] headTransformsDataLines = HeadTransformsData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string[]> headTransformsData = new List<string[]>();
        for (int i = 1; i < headTransformsDataLines.Length; i++)
            headTransformsData.Add(headTransformsDataLines[i].Split('\t'));

        string[] axesDataLines = AxesData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string[]> axesData = new List<string[]>();
        for (int i = 1; i < axesDataLines.Length; i++)
            axesData.Add(axesDataLines[i].Split('\t'));

        string[] cameraTransformDataLines = CameraTransformData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string[]> cameraTransformData = new List<string[]>();
        for (int i = 1; i < cameraTransformDataLines.Length; i++)
            cameraTransformData.Add(cameraTransformDataLines[i].Split('\t'));

        float totalTime = float.Parse(headTransformsData[headTransformsData.Count - 1][0]);

        cameraTransformPositionFilter = new OneEuroFilter<Vector3>(10);
        cameraTransformRotationFilter = new OneEuroFilter<Quaternion>(10);

        int axesLineIdx = 1;
        int cameraLineIdx = 1;

        float startUnityTime = Time.time;

        for (int i = 0; i < headTransformsData.Count; i += (isLiveReplayPaused) ? 0 : 1)
        {
            if (TimeScrubber != prevTimeScrubber)
            {
                // Find the nearest i index that matches the new time
                float newTime = TimeScrubber * totalTime;

                int startIdx = Mathf.FloorToInt(TimeScrubber * headTransformsData.Count) - 50;
                startIdx = Mathf.Max(startIdx, 0);
                for (int j = startIdx; j < headTransformsData.Count; j++)
                {
                    float thisTime = float.Parse(headTransformsData[j][0]);
                    if (thisTime > newTime)
                    {
                        newTime = float.Parse(headTransformsData[Mathf.Max(j - 1, 0)][0]);
                        i = j;
                        break;
                    }
                }
                
                axesLineIdx = 1;
                cameraLineIdx = 1;
                prevTimeScrubber = TimeScrubber;
                if (LiveVideo.clip != null)
                    LiveVideo.time = newTime + VideoStartTimeOffset;
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

            float currentTime = float.Parse(headTransformsData[i][0]);
            
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
                string[] headTransformsLine = headTransformsData[i + j];

                Transform headTransform = HeadObjects[int.Parse(headTransformsLine[1])];

                headTransform.position = new Vector3(float.Parse(headTransformsLine[2]), float.Parse(headTransformsLine[3]), float.Parse(headTransformsLine[4]));
                headTransform.rotation = new Quaternion(float.Parse(headTransformsLine[5]), float.Parse(headTransformsLine[6]), float.Parse(headTransformsLine[7]), float.Parse(headTransformsLine[8]));
            }

            // Find the line index of the axes data which has the same time as the current head transforms line
            for (int j = axesLineIdx; j < axesDataLines.Length; j++)
            {
                string[] axesLine = axesData[j];

                if (currentTime == float.Parse(axesLine[0]))
                {
                    axesLineIdx = j;
                    break;
                }
            }

            // Now that we've found it, set the properties of the axes
            for (int j = 0; j < AxesObjects.Length; j++)
            {
                string[] axesLine = axesData[axesLineIdx + j];

                ServerAxis axis = AxesObjects[int.Parse(axesLine[1])];

                // Check if the axis was running while logging. It returns everything as 0 if it does. In this case, ignore the line
                if (int.Parse(axesLine[9]) == 0 && float.Parse(axesLine[10]) == 0 && float.Parse(axesLine[11]) == 0)
                    continue;

                axis.transform.position = new Vector3(float.Parse(axesLine[2]), float.Parse(axesLine[3]), float.Parse(axesLine[4]));
                axis.transform.rotation = new Quaternion(float.Parse(axesLine[5]), float.Parse(axesLine[6]), float.Parse(axesLine[7]), float.Parse(axesLine[8]));
                axis.dimensionIdx = int.Parse(axesLine[9]);
                axis.minFilter = float.Parse(axesLine[10]);
                axis.maxFilter = float.Parse(axesLine[11]);
                axis.infoboxToggle = bool.Parse(axesLine[12]);
                axis.infoboxPosition = float.Parse(axesLine[13]);
            }

            // Set the position and rotation of the external camera
            cameraLineIdx = Mathf.CeilToInt(i / 2f);
            string[] cameraTransformLine = cameraTransformData[cameraLineIdx];
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
