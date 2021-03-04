using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayback : MonoBehaviour
{
    [Header("Data Files")]
    public TextAsset HeadTransformsData;
    public TextAsset AxesData;

    [Header("Replay Objects")]
    public Transform[] HeadObjects;
    public ServerAxis[] AxesObjects;

    [Header("Replay Settings")]
    public int LinesPerFrame = 1;

    private bool isLiveReplayRunning = false;
    private bool isLiveReplayPaused = false;

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

        foreach (ServerAxis axis in AxesObjects)
        {
            axis.dataPlaybackMode = true;
        }

        string[] headTransformsDataLines = HeadTransformsData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] axesDataLines = AxesData.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int axesLineIdx = 1;

        for (int i = 1; i < headTransformsDataLines.Length; i += (isLiveReplayPaused) ? 0 : LinesPerFrame)
        {
            if (isLiveReplayPaused)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            float currentTime = float.Parse(headTransformsDataLines[i].Split('\t')[0]);

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

                ServerAxis axis = AxesObjects[int.Parse(axesLine[1])];

                axis.transform.position = new Vector3(float.Parse(axesLine[2]), float.Parse(axesLine[3]), float.Parse(axesLine[4]));
                axis.transform.rotation = new Quaternion(float.Parse(axesLine[5]), float.Parse(axesLine[6]), float.Parse(axesLine[7]), float.Parse(axesLine[8]));
                axis.dimensionIdx = int.Parse(axesLine[9]);
                axis.minFilter = float.Parse(axesLine[10]);
                axis.maxFilter = float.Parse(axesLine[11]);
                axis.infoboxToggle = bool.Parse(axesLine[12]);
                axis.infoboxPosition = float.Parse(axesLine[13]);
            }

            yield return null;
        }

        isLiveReplayRunning = false;
        isLiveReplayPaused = false;
        
        foreach (ServerAxis axis in AxesObjects)
        {
            axis.dataPlaybackMode = false;
        }

    }

    public void PauseLiveReplay()
    {
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
