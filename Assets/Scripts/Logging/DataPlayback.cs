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

    private bool isLiveReplayRunning = false;
    private bool isLiveReplayPaused = false;

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
        LiveVideo.gameObject.SetActive(true);
        LiveVideo.Play();
        LiveVideo.time = VideoStartTimeOffset;

        foreach (ServerAxis axis in AxesObjects)
        {
            axis.dataPlaybackMode = true;
            axis.transform.GetChild(0).gameObject.SetActive(false);
        }

        string[] headTransformsDataLines = HeadTransformsData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] axesDataLines = AxesData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] cameraTransformDataLines = CameraTransformData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        cameraTransformPositionFilter = new OneEuroFilter<Vector3>(10);
        cameraTransformRotationFilter = new OneEuroFilter<Quaternion>(10);

        int axesLineIdx = 1;
        int cameraLineIdx = 1;

        float startUnityTime = Time.time;

        for (int i = 1; i < headTransformsDataLines.Length; i += (isLiveReplayPaused) ? 0 : 1)
        {
            if (isLiveReplayPaused)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            else if (LiveVideo.isPaused)
            {
                LiveVideo.Play();
            }

            float currentTime = float.Parse(headTransformsDataLines[i].Split('\t')[0]);
            float currentUnityTime = Time.time - startUnityTime;
            
            // If the dataset's timestamp is smaller than our Unity timestamp, skip a frame
            if (currentTime + 1.2f > currentUnityTime)
            {
                i--;
                yield return null;
                continue;
            }
            
            // Set the positions and rotations of the heads
            for (int j = 0; j < HeadObjects.Length; j++)
            {
                string[] headTransformsLine = headTransformsDataLines[i + j].Split('\t');

                Transform headTransform = HeadObjects[int.Parse(headTransformsLine[1])];

                headTransform.position = new Vector3(float.Parse(headTransformsLine[2]), float.Parse(headTransformsLine[3]), float.Parse(headTransformsLine[4]));
                headTransform.rotation = new Quaternion(float.Parse(headTransformsLine[5]), float.Parse(headTransformsLine[6]), float.Parse(headTransformsLine[7]), float.Parse(headTransformsLine[8]));
            }

            // Find the line index of the axes data which has the same time as the current head transforms line
            for (int j = axesLineIdx; j < axesDataLines.Length; j++)
            {
                string[] axesLine = axesDataLines[j].Split('\t');

                if (currentTime == float.Parse(axesLine[0]))
                {
                    axesLineIdx = j;
                    break;
                }
            }

            // Now that we've found it, set the properties of the axes
            for (int j = 0; j < AxesObjects.Length; j++)
            {
                string[] axesLine = axesDataLines[axesLineIdx + j].Split('\t');

                ServerAxis axis = AxesObjects[int.Parse(axesLine[9])];

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
            string[] cameraTransformLine = cameraTransformDataLines[cameraLineIdx].Split('\t');
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
