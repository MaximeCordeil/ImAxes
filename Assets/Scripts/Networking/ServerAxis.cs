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

    [Header("Client Axis Values")]
    public int dimensionIdx;
    public float minNormaliser;
    public float maxNormaliser;

    private int prevSliderOne;
    private int prevSliderTwo;
    private int prevRotary;
    private bool connected;
    private bool axisInitialised;   // Added these - to reset rotary to zero on if axes not repowered.
    private int rotaryDifference;
    private bool followMode;

    // Bluetooth variables
    private SerialPort sp;
    private Thread ReadThread;
    private Thread CheckPortThread;

    private void Start()
    {
        sp = new SerialPort(COM, 115200);
        sp.ReadTimeout = 2000;
        sp.WriteTimeout = 2000;
        sp.WriteTimeout = 2000;
        sp.Parity = Parity.None;
        sp.DataBits = 8;
        sp.StopBits = StopBits.One;
        sp.RtsEnable = true;
        sp.Handshake = Handshake.None;
        sp.NewLine = "\n";  // Need this or ReadLine() fails

        TryConnectToPort();
    }

    private void TryConnectToPort()
    {
        try
        {
            sp.Open();
        }
        catch (SystemException f)
        {
            print("Failed to open port " + COM);
            return;
        }

        if (sp.IsOpen)
        {
            print("Successfully opened port " + COM);

            ReadThread = new Thread(new ThreadStart(ReadSerial));
            ReadThread.Start();
            SetSteppedMode(0);
            SetLEDValue(255);
        }
    }

    /// <summary>
    /// Reads the serial to determine the input values for the Axis. These values are then send via RPC in the Update() loop.
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
                    minNormaliser = Remap((float)sliderOne, 0f, 255f, -0.505f, 0.505f);
                    maxNormaliser = Remap((float)sliderTwo, 0f, 255f, -0.505f, 0.505f);
                }
            }
            catch (SystemException f)
            {
                print(f);
                ReadThread.Abort();
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (sp.IsOpen)
        {
            // We update the axis properties just for the server. These values are sent to the clients in OnPhotonSerializeView().
            if (PhotonNetwork.IsMasterClient)
            {
                gameObject.GetComponent<ClientAxis>().UpdateClientAxis(dimensionIdx, minNormaliser, maxNormaliser);
            }
        }
#endif
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // We write values to the stream in this script, which gets read by the ClientAxis.cs script instead
        if (stream.IsWriting)
        {
            // Send the transform properties to the stream
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);

            // Now send the axis properties to the stream
            stream.SendNext(dimensionIdx);
            stream.SendNext(minNormaliser);
            stream.SendNext(maxNormaliser);
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
    }

    private float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    private void OnApplicationQuit()
    {
        SetLEDValue(0);
        ReadThread.Abort();
    }
}
