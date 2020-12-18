using Photon.Pun;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ServerAxis : MonoBehaviourPun
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
    private bool isUpdateQueued = false;
    private bool followMode;

    private Vector3 prevPosition;
    private Quaternion prevRotation;

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

        try
        {
            sp.Open();
        }
        catch (SystemException f)
        {
            print("FAILED TO OPEN PORT");

        }
        if (sp.IsOpen)
        {
            print("SerialOpen!");

            ReadThread = new Thread(new ThreadStart(ReadSerial));
            ReadThread.Start();
            SetSteppedMode(0);
        }
        else
        {
           // StartCoroutine(CheckPort());
        }
    }

    private void ReadSerial()
    {
        while (ReadThread.IsAlive)
        {
            try
            {
                if (sp.BytesToRead > 1)
                {
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

                    isUpdateQueued = true;
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
        if (prevPosition != transform.position || prevRotation != transform.rotation)
        {
            if (PhotonNetwork.IsConnected)
                photonView.RPC("UpdateClientTransform", RpcTarget.All, transform.position, transform.rotation); //ADDED
            else
                GetComponent<ClientAxis>().UpdateClientTransform(transform.position, transform.rotation);

            prevPosition = transform.position;
            prevRotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        // We send messages to clients in FixedUpdate as these cannot be sent on threads other than the main one
        if (isUpdateQueued)
        {
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

            if (PhotonNetwork.IsConnected)
                photonView.RPC("UpdateClientAxis", RpcTarget.All, dimensionIdx, minNormaliser, maxNormaliser); //ADDED
            else
                GetComponent<ClientAxis>().UpdateClientAxis(dimensionIdx, minNormaliser, maxNormaliser);

            isUpdateQueued = false;
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
        ReadThread.Abort();
    }
}
