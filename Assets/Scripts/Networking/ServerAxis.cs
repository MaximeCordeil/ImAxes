using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ServerAxis : MonoBehaviourPun, IPunObservable
{
    [Header("Bluetooth Settings")]
    public string COM = "COM9";

    [Header("Physical Axis Controller Values")]
    public int sliderOne;
    public int sliderTwo;
    public int rotary;
    public int rotaryPress;
    public int buttonPress;

    [Header("Misc Settings")]
    public int followDistanceOverride;
    public int serverAxisIndex;

    [Header("Client Axis Values")]
    public int dimensionIdx;
    public float minFilter;
    public float maxFilter;
    public bool infoboxToggle;
    public float infoboxPosition;

    [Header("Axis Override Settings")]
    public bool fixedDimensionMode = false;
    public int fixedDimensionIdx = 0;
    public bool dataPlaybackMode = false;

    private int prevSliderOne;
    private int prevSliderTwo;
    private int prevRotary;
    private bool connected;
    private bool axisInitialised;   // Added these - to reset rotary to zero on if axes not repowered.
    private int rotaryDifference;
    private bool followMode;
    private bool ignoreSliderMovement;
    private bool ignoreButtonPresses;


    // Bluetooth variables
    private SerialPort sp;
    private Thread ReadThread;

    private void Start()
    {
        TryConnectToPort();
    }

    private void TryConnectToPort()
    {
        try
        {
            sp = new SerialPort(COM, 115200);
            sp.ReadTimeout = 2000;
            sp.WriteTimeout = 2000;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
            sp.RtsEnable = true;
            sp.Handshake = Handshake.None;
            sp.NewLine = "\n";  // Need this or ReadLine() fails
            sp.Open();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to open port " + COM);
            sp.Close();
            sp.Dispose();
            return;
        }

        if (sp.IsOpen)
        {
            Debug.Log("Successfully opened port " + COM);

            ReadThread = new Thread(new ThreadStart(ReadSerial));
            ReadThread.Start();
            SetSteppedMode(0);
            SetLEDValue(255);
        }
    }

    /// <summary>
    /// Reads the serial to determine the input values for the Axis. These values are then send via RPC in the OnPhotonSerializeView() loop.
    /// </summary>
    private void ReadSerial()
    {
        while (ReadThread.IsAlive)
        {
            try
            {
                if (sp.BytesToRead > 1)
                {
                    // Read in the raw values from the serial
                    string indata = sp.ReadLine();
                    string[] splits = indata.Split(' ');
                    rotaryPress = (int.Parse(splits[3]) == 1) ? 0 : 1;
                    buttonPress = int.Parse(splits[4]);
                    if (rotaryPress != 1 && buttonPress != 1)
                    {
                        sliderOne = int.Parse(splits[1]);
                        prevSliderOne = sliderOne;
                        if (!followMode)
                        {
                            sliderTwo = int.Parse(splits[0]);
                            prevSliderTwo = sliderTwo;
                        }
                        else
                        {
                            sliderTwo = sliderOne - followDistanceOverride;
                            prevSliderTwo = sliderTwo;
                        }
                    }
                    else
                    {
                        sliderOne = prevSliderOne; // Prevents slidervalues being changed on button press
                        sliderTwo = prevSliderTwo;
                    }

                    if (!axisInitialised)  // Makes rotary 0 on first run regardless of resetting axes.
                    {
                        axisInitialised = true;
                        rotaryDifference = int.Parse(splits[2]);
                    }
                    rotary = int.Parse(splits[2]) - rotaryDifference;

                    // Now we can convert these raw values into input variables that the client axis can understand
                    // Update direction which the rotary was spun, and increment dimension accordingly
                    if (prevRotary > rotary)
                    {
                        dimensionIdx = (++dimensionIdx) % SceneManager.Instance.dataObject.NbDimensions;
                    }
                    else if (prevRotary < rotary)
                    {
                        dimensionIdx--;
                        if (dimensionIdx < 0)
                            dimensionIdx += SceneManager.Instance.dataObject.NbDimensions;
                    }
                    prevRotary = rotary;
                }
            }
            catch (SystemException f)
            {
                Debug.LogError(f);
                ReadThread.Abort();
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR    // We assume that the server is always a  Unity Editor
        if (sp.IsOpen)
        {
            // Check for button toggle. Add a delay as well to prevent multiple button presses to be given in quick succession
            if (buttonPress == 1 && !ignoreButtonPresses)
            {
                infoboxToggle = !infoboxToggle;

                // Adjust modes depending on the new toggle
                if (infoboxToggle)
                {
                    FollowModeChange(128);
                    SendSlider(0, (int)Remap(infoboxPosition, 0.505f, -0.505f, 0f, 255f));
                    SendSlider(1, (int)Remap(infoboxPosition, 0.505f, -0.505f, 0f, 255f));
                }
                else
                {
                    TurnOffFollowMode();
                    SendSlider(0, (int)Remap(minFilter, 0.505f, -0.505f, 0f, 255f));
                    SendSlider(1, (int)Remap(maxFilter, 0.505f, -0.505f, 0f, 255f));
                }

                StartCoroutine(IgnoreSliderMovementForDuration(0.8f));
                StartCoroutine(IgnoreButtonPressesForDuration(1f));
            }

            if (!ignoreSliderMovement)
            {
                // Send different values depending if the infobox is enabled or not
                if (infoboxToggle)
                {
                    infoboxPosition = Remap(Mathf.RoundToInt(((float)sliderOne + (float)sliderTwo)) / 2, 0, 255f, 0.505f, -0.505f);
                }
                else
                {
                    minFilter = Remap((float)sliderOne, 0f, 255f, 0.505f, -0.505f);
                    maxFilter = Remap((float)sliderTwo, 0f, 255f, 0.505f, -0.505f);
                }
            }

            // Check for rotary press. Use this to reset the axis object in case of a bug
            if (rotaryPress == 1 && !ignoreButtonPresses)
            {
                photonView.RPC("ResetAxisObject", RpcTarget.Others);
                PhotonNetwork.SendAllOutgoingCommands();
                gameObject.GetComponent<ClientAxis>().ResetAxisObject();
            }
        }

        if (sp.IsOpen || dataPlaybackMode)
        {
            // We update the axis properties just for the server. These values are sent to the clients in OnPhotonSerializeView()
            // If the fixed dimension mode is enabled, we just override any dimensionIdx we calculate with the fixed index value
            if (fixedDimensionMode && !dataPlaybackMode)
                gameObject.GetComponent<ClientAxis>().UpdateClientAxis(fixedDimensionIdx, minFilter, maxFilter, infoboxPosition);
            else
                gameObject.GetComponent<ClientAxis>().UpdateClientAxis(dimensionIdx, minFilter, maxFilter, infoboxPosition);
            gameObject.GetComponent<ClientAxis>().ToggleInfoboxMode(infoboxToggle);
        }
#endif
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // We write values to the stream in this script, which gets read by the ClientAxis.cs script instead
        if (stream.IsWriting)
        {
            transform.SetParent(ViconOriginSceneCalibrator.Instance.Root);

            // Send the transform properties to the stream
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localRotation);

            // Now send the axis properties to the stream
            // If the fixed dimension mode is enabled, we just override any dimensionIdx we calculate with the fixed index value
            if (fixedDimensionMode)
                stream.SendNext(fixedDimensionIdx);
            else
                stream.SendNext(dimensionIdx);
            stream.SendNext(minFilter);
            stream.SendNext(maxFilter);
            stream.SendNext(infoboxPosition);

            // Now send any boolean values to the stream
            stream.SendNext(infoboxToggle);

            transform.parent = null;
        }
    }

    public void NewSendMsg(int mode, int value)
    {
        string valueString = value.ToString();
        if (value >= 0 && value <= 9)
        {
            valueString = "00" + valueString;
        }
        else if (value >= 10 && value <= 99)
        {
            valueString = "0" + valueString;
        }
        else if (value >= 100 && value <= 999)
        {
            valueString = "" + valueString;
        }

        try
        {
            string message = mode.ToString() + valueString;
            sp.WriteLine(message);
        }
        catch (SystemException f)
        {
            print("ERROR::A message failed to send");
        }
    }

    public void SetSteppedMode(int steppedRange)
    {
        NewSendMsg(4, steppedRange);
    }

    public void SendSlider(int slider, int value)
    {
        if (slider == 1)
        {
            NewSendMsg(0, value);
        }
        else
        {
            NewSendMsg(1, value);
        }
    }
    public void SetLEDValue(int value)  // 0 to 256
    {
        NewSendMsg(8, value);
        //LEDValue = value;
    }
    public void FollowModeChange(int distance)
    {
        if (distance < 512)
        {
            followMode = true;
        }
        else
        {
            followMode = false;
        }
        NewSendMsg(2, distance);  //0-256 one follows 2. 257 - 512, two follows  one, > follow off.  0-128 is neg, 128 - 256 positive
    }

    public void TurnOffFollowMode()
    {
        NewSendMsg(2, 999);
        followMode = false;
    }
    
    public void ResetSerialPort()
    {
        if (sp != null && sp.IsOpen)
        {
            Debug.Log("Force resetting port " + COM);
            sp.Close();
            sp.Dispose();

            if (ReadThread.IsAlive)
                ReadThread.Abort();
        }

        // Reconnect after a short delay
        Invoke("TryConnectToPort", 0.25f);
    }

    private float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    private IEnumerator IgnoreButtonPressesForDuration(float time)
    {
        ignoreButtonPresses = true;
        yield return new WaitForSeconds(time);
        ignoreButtonPresses = false;

    }

    private IEnumerator IgnoreSliderMovementForDuration(float time)
    {
        ignoreSliderMovement = true;
        yield return new WaitForSeconds(time);
        ignoreSliderMovement = false;
    }

    private void OnApplicationQuit()
    {
        SetLEDValue(0);
        if (ReadThread != null)
            ReadThread.Abort();
    }
}
