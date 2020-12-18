using Photon.Pun;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class WirelessAxes : MonoBehaviourPun
{
    Vector3 oldPos = new Vector3();
    Quaternion oldRot = new Quaternion();
    public string COM = "COM9";
    

    public int sliderOne;
    public int sliderTwo;
    public int rotary;
    public int rotaryPress;
    public int buttonPress;
    int oldSliderOne;
    int oldSliderTwo;
    bool connected;
    bool firstRead;   // Added these - to reset rotary to zero on if axes not repowered.
    int rotaryDifference;
    
    
    int modeSelect = 0;
   
    public bool joystickMode = true;
    bool followMode;
    public int followDistanceOverride;
    //////////// these are only needed for testing with the keypresses. feel free to delete in order to clean things up. 
    [Range(0, 256)]
    public int LEDValue = 0;
    [Range(0, 600)]
    public int followDist;
    [Range(0, 256)]
    public int sendingSliderOne;
    [Range(0, 256)]
    public int sendingSliderTwo;
    [Range(0, 256)]
    public int hapticsValue1 = 200; 
    [Range(0, 256)]
    public int hapticsValue2 = 200;
 
    /// ////////////////////////////////////////////
  

    SerialPort sp;
    Thread ReadThread;
    Thread CheckPortThread;
    void Start()
    {
        //ReadThread = new Thread(new ThreadStart(ReadSerial));
        //ReadThread.Start();
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
            setSteppedMode(0);
            //  SetJoystickMode(6);
            setLEDValue(255);
        }
        else
        {
           // StartCoroutine(CheckPort());
        }
        //sendSlider(1, 255);
       // sendSlider(2, 0);
    }
    void TryPort()
    {
        print("CAlled");
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
            setSteppedMode(0);
            //  SetJoystickMode(6);
            setLEDValue(255);
        }
        else
        {
                
            StartCoroutine(CheckPort());
        }
    }
    IEnumerator CheckPort()  // Ignore
    {
        yield return new WaitForSeconds(1f);
        CheckPortThread = new Thread(new ThreadStart(TryPort));
        CheckPortThread.Start();


    }
    public void newSendMsg(int mode, int value1)
    {
        string value1String = value1.ToString();
        

        if (value1 >= 0 && value1 <= 9)
        {
            value1String = "00" + value1String;
        }
        else if (value1 >= 10 && value1 <= 99)
        {
            value1String = "0" + value1String;
        }
        else if (value1 >= 100 && value1 <= 999)
        {
            value1String = "" + value1String;
        }

        

        string message = mode.ToString() + value1String;                                                                                                                      
        try
        {
            sp.WriteLine(message);
        }
        catch (SystemException f)
        {
            print("ERROR::A message failed to send");
        }


    }

   

  

    void ReadSerial()
    {
        while (ReadThread.IsAlive)
        {
            try
            {
                if (sp.BytesToRead > 1)
                {

                    string indata = sp.ReadLine();

                    string[] splits = indata.Split(' ');
                    rotaryPress = int.Parse(splits[3]);
                    if (rotaryPress == 1) rotaryPress = 0;
                    else rotaryPress = 1;
                    buttonPress = int.Parse(splits[4]);

                    if (rotaryPress != 1 && buttonPress != 1)
                    {
                        sliderOne = int.Parse(splits[1]);
                        oldSliderOne = sliderOne;
                        if (!followMode)       
                        {
                            sliderTwo = int.Parse(splits[0]);
                            oldSliderTwo = sliderTwo;
                        }
                        else
                        {
                            sliderTwo = sliderOne - followDistanceOverride;
                            oldSliderTwo = sliderTwo;
                        }
                    }
                    else
                    {
                        sliderOne = oldSliderOne; // Prevents slidervalues being changed on button press
                        sliderTwo = oldSliderTwo;
                    }
                    if (!firstRead)  // Makes rotary 0 on first run regardless of resetting axes.
                    {
                        firstRead = true;
                        rotaryDifference = int.Parse(splits[2]);

                        if (PhotonNetwork.IsConnected)
                            photonView.RPC("InitialiseAxis", RpcTarget.All, rotaryDifference); //ADDED
                    }
                    rotary = int.Parse(splits[2]) - rotaryDifference;

                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC("UpdateAxis", RpcTarget.All, sliderOne, sliderTwo, rotary, rotaryPress, buttonPress); //ADDED
                    }
                    else
                    {
                        //GetComponent<AxisController>().UpdateAxis(sliderOne, sliderTwo, rotary, rotaryPress, buttonPress);
                    }
                }
            }
            catch (SystemException f)
            {
                print(f);
                ReadThread.Abort();
            }

        }
    }

    void Update()
    {
        if (transform.position != oldPos || transform.rotation != oldRot)
        {
            //oldRot = transform.rotation;
           // oldPos = transform.position;
         //   photonView.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation); //ADDED
        }
    }

   public void setSteppedMode(int steppedRange) // between 10 and 128 else turns stepped off
    {
        newSendMsg(4, steppedRange);
    }

    public void setLEDValue(int value)  // 0 to 256
    {
        newSendMsg(8, value);
        LEDValue = value;
    }

    public void sendSlider(int slider, int targetValue) // 0 to 256, slider 1 or 2
    {
       // print(slider);
      //  print(targetValue); 
        if (slider == 1)
        {
           newSendMsg(0, targetValue); 
        }
        else
        {
            newSendMsg(1, targetValue);
        }
    }

    public void FollowModeChange(int distance) 
    {
        if (distance < 512)
        {
            followMode = true;
           // followDistanceOverride = 128 - distance;
        }
        else
        {
            followMode = false;
        }
           newSendMsg(2, distance);  //0-256 one follows 2. 257 - 512, two follows  one, > follow off.  0-128 is neg, 128 - 256 positive
    }
    public void TurnOffFollowMode()
    {
        newSendMsg(2, 999);
       
    }

    public void SetJoystickMode(int mode)  // 1 = on on. 2 = 2 on. 3 = both on. 4 = 1 off. 5 = 2 off. 6 = both off. 
    {
        
            newSendMsg(3, mode );
        
    }
    

    public void hapticPulse(int slider, int value) // 0 -256
    {
        
        if (slider == 1)
        {
            newSendMsg(6, value);
        }
        else
        {
            newSendMsg(7, value);

        }
    }

    void OnApplicationQuit()
    {
        ReadThread.Abort();
    }

}
