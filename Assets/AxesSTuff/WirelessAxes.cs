using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class WirelessAxes : MonoBehaviour
{
  
    public string COM = "COM9";
    

    public int sliderOne;
    public int sliderTwo;
    public int rotary;
    public int rotarypress;
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
                    rotarypress = int.Parse(splits[3]);
                    if (rotarypress == 1) rotarypress = 0;
                    else rotarypress = 1;
                    buttonPress = int.Parse(splits[4]);

                    if (rotarypress != 1 && buttonPress != 1)
                    {
                        sliderOne = int.Parse(splits[1]);
                        oldSliderOne = sliderOne;
                        if (!followMode)        ////////////////////dodgy FollowModeCheat!
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

                    }
                    rotary = int.Parse(splits[2]) - rotaryDifference;

                    
                  
                }
            }
            catch (SystemException f)
            {
                print(f);
                ReadThread.Abort();
            }
        }
    }



    void Update()  // For easy testing only! can be deleted if wishing to tidy things up. 
    {
       
       
      // Remember to click back onto the game window for keystrokes to work. 
        if (Input.GetKeyDown(KeyCode.Z))  /// Test haptics one.   // to do get into arduino code to get better haptic range. 
        {
            hapticPulse(1, hapticsValue1);
        }
        if (Input.GetKeyDown(KeyCode.X))  // test haptics two. 
        {
            hapticPulse(2, hapticsValue2);
        }
        if (Input.GetKeyDown(KeyCode.C))  // test snapTo 
        {
            sendSlider(1, sendingSliderOne);
        }
        if (Input.GetKeyDown(KeyCode.V))  // test snapTo // Tested - slider two snap not working properly. 
        {
            sendSlider(2, sendingSliderTwo);  // 
        }
        if (Input.GetKeyDown(KeyCode.B))  // te
        {
            setLEDValue(LEDValue); //Led test good. 
        }
        if (Input.GetKeyDown(KeyCode.J))  // te
        {
            FollowModeChange(513); ; //
        }
        if (Input.GetKeyDown(KeyCode.F))  // te
        {
            FollowModeChange(followDist);
        }
        if (Input.GetKeyDown(KeyCode.S))  // te
        {
            setSteppedMode(130);
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
