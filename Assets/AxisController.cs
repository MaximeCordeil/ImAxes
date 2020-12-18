using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AxisController : MonoBehaviour
{
    WirelessAxes Wax;
    Axis axis;
    Axis LastAxis;
    GameObject CreatedAxis;
    public int indx;
    public GameObject menuParent;
    public Transform target;
    public Transform creationTarget;
    public GameObject pointer;
    BoxCollider pointerCollider;
    Renderer pointerRend;
    public GameObject menuText;
    public Color pointInactive;
    public Color pointActive;
    List<int> activeIds = new List<int>();
    List<GameObject> menuTexts = new List<GameObject>();
    Vector3 bottomPos;
    Vector3 topPos;
    bool firstCreated;
     Axis CurrentAxis;
    float initialMin;
    float initialMax;
    public float menuTextSpacing;
    int OldSliderOne;
    int OldSliderTwo;
    int OldRotary;
    bool found;
    bool sending;
    public bool click;
    bool topClick;
    bool topClicked;
    bool beamSelected;
    public Transform miniAxes;
    public Vector3 miniAxesOffset;
    int rotIndex;
    bool RotSelection;
    bool clickedDelay;
    public bool clicked;
    float GarbageDump = -1f;
    float creationRotation;
    Vector3 ClickedPosition;
    Vector3 CreatedPosition;
    public bool menuActive;
    public bool inTrig;
    public bool menuDelay;
    MiniVis miniVis;
    void Start()
    {
        Wax = GetComponent<WirelessAxes>();
        OldSliderOne = Wax.sliderOne;
        OldSliderTwo = Wax.sliderTwo;
        OldRotary = Wax.rotary;
        pointerCollider = pointer.GetComponent<BoxCollider>();
        pointerRend = pointer.GetComponent<MeshRenderer>();
        pointerCollider.enabled = false;
        pointerRend.material.color = pointInactive;
        miniVis = GetComponentInChildren<MiniVis>();

    }
    public void SetAxisViaBeam(Axis selected)
    {
        
        CurrentAxis = selected;
    }
    void OnTriggerEnter(Collider other)   
    {
        if (other.tag == "Axis")
        {
            inTrig = true;
            
        }
       

    
    }
    void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Axis")
        {
            inTrig = false;
        }
    }
    void CreateAxis(bool rot)
    {

        Axis ProtoAxis;
        if (!firstCreated)
        {
             ProtoAxis = Axis.AxisList[indx];
            firstCreated = true;
        }
        else
        {
             ProtoAxis = Axis.AxisList[rotIndex];
        }
       // Quaternion lastRot = Axis.CurrentAxis.transform.rotation;
         
        GameObject newAxis = ProtoAxis.Clone();
       // newAxis.transform.rotation = lastRot;
        newAxis.transform.position = this.transform.position;
        print(this.transform.position);
        axis = newAxis.GetComponent<Axis>();
        axis.transform.parent = this.transform;
        axis.transform.localPosition = new Vector3(0, 0, 0);
        axis.isClone = true;
       // miniVis.SetMesh(axis.axisId);
       CurrentAxis = axis;
       
        
        SetSliders();
        axis.HideHandles();
       
     
        if (rotIndex < Axis.AxisList.Count - 1 &&!rot)
        {
            rotIndex++;
        }
        else 
        {
            if (!rot)
            {
                rotIndex = 0;
            }
        }
        
        
       
           // axis.transform.position = ClickedPosition;
       // Vector3 targetPostition = new Vector3(target.position.x, axis.transform.position.y, target.position.z);
        if (!axis.IsHorizontal)
            {
          //  axis.transform.LookAt(targetPostition);
        }

       
        
       
       


        // LastAxis = axis;
        OldRotary = Wax.rotary;
    }
        void Update()
        {
      

        click = Convert.ToBoolean(Wax.rotaryPress);
        topClick = Convert.ToBoolean(Wax.buttonPress);

        if (!found)
            {
                GetInitialAxis();
            }

            else
            {

                ControlAxis();

            }
        if (topClick && !topClicked)
        {
            topClicked = true;
            pointerCollider.enabled = true;
            pointerRend.material.color = pointActive;

        }
        if (!topClick&&topClicked)
        {
            topClicked = false;
            pointerCollider.enabled = false;
            pointerRend.material.color = pointInactive;
        }

            if (click && !clicked)  // first press.
            {
            
                ButtonPress();
                StartCoroutine(ClickDelay()); 
                clicked = true;
           
                menuParent.SetActive(true);
            ShiftMenu(true); // do this in buttonPRess?
                
            
        
        }
        if (!click && clicked) // released button
        {
            clicked = false;
            menuParent.SetActive(false);

        }

      
         // TODO clean this up. Release button to create.  hold and pop up data menu...

    }

    void ButtonPress()
    {
        if (!clickedDelay)
        {
            if (!inTrig)
            {
                creationRotation = transform.eulerAngles.y;
                ClickedPosition = creationTarget.position;
                CreateAxis(false);
                CreatedPosition = transform.position;
                

            }
            else
            {
                if (axis.isClone)
                {
                    axis.transform.position = new Vector3(0, GarbageDump, 0);  //Delete selected
                                                                               // GetInitialAxis();
                    inTrig = false;
                    axis = Axis.AxisList[rotIndex];
                    CurrentAxis = axis;
                }


            }
        }
       // clicked = false;
    }
      void RotarySpun(bool up)
    {
        // ClickedPosition = axis.transform.position;
      
        OldRotary = Wax.rotary;
        if (up)
        {
            rotIndex++;
            if (rotIndex == Axis.AxisList.Count)
            {
                rotIndex = 0;
            }

            ShiftMenu(true);
           

        }
        else
        {
            rotIndex--;
            //rotIndex--;
            if (rotIndex < 0)
            {
                rotIndex = Axis.AxisList.Count - 1;
            }
            ShiftMenu(false);
            

        }
       // miniVis.SetMesh(axis.axisId);
        //if (!menuActive)
        //{
        // miniVis.SetMesh(rotIndex);
        LastAxis = axis;
       
         CreateAxis(true);
        //if (LastAxis.isClone)
        //{
        SceneManager.Instance.DestroyAxis(LastAxis);
        //}
        //}
    }
    void ShiftMenu(bool up)
    {
        if (up)
        {
            foreach (GameObject m in menuTexts)
            {
                m.transform.localPosition = new Vector3(m.transform.localPosition.x, m.transform.localPosition.y, m.transform.localPosition.z - menuTextSpacing);
                if (m.transform.localPosition.z > topPos.z)
                {
                    m.transform.localPosition = bottomPos;
                }
            }
        }
        else
        {
            foreach (GameObject t in menuTexts)
            {
                Vector3 currentPos = t.transform.localPosition;  // put all this in a method. CAll up and down rotary or button press. 
                t.transform.localPosition = new Vector3(t.transform.localPosition.x, t.transform.localPosition.y, t.transform.localPosition.z + menuTextSpacing);

               
                if (t.transform.localPosition.z < bottomPos.z)
                {
                    t.transform.localPosition = topPos;//new Vector3(m.transform.localPosition.x, m.transform.localPosition.y, 1.1f);
                }
            }
        }
    }
        void ControlAxis()
        {
            if (CurrentAxis != null)
            {  
                    if (axis.grabbed)
            {
                ClickedPosition = axis.transform.position;
              //  miniAxes.transform.position = axis.ReportPosition() - miniAxesOffset;

            }
           

                    if (OldRotary < Wax.rotary)
                     {
               
                    RotarySpun(true);
                      }
                    else if (OldRotary > Wax.rotary)
                     {
                     RotarySpun(false);
                     }
                     /////////////////////////////// rest to go in rotarySpun.
              
                // get the boxcollider of pointer to change Axis here.


                if (axis != CurrentAxis) // will need to set currentAxis when rotary overtwrites axis selection.
               {
                  axis = CurrentAxis;

                   // print("New Axis Selected " + axis.axisId);
                    SetSliders();
                    axis.HideHandles();
                miniVis.SetMesh(axis.axisId);
                // miniAxes.transform.position = axis.ReportPosition() - miniAxesOffset;

                //  Axis.AxisRot = axis.transform.rotation; /////////////////////////////////////////////////////
            }



            if (OldSliderOne != Wax.sliderOne && !sending)
                {
                    OldSliderOne = Wax.sliderOne;

                    float val = Remap((float)Wax.sliderOne, 0f, 255f, -0.505f, 0.505f);
                   // print(val);
                    axis.SetMaxNormalizer(val);
                }
                if (OldSliderTwo != Wax.sliderTwo && !sending)
                {
                    OldSliderTwo = Wax.sliderTwo;

                    float val = Remap((float)Wax.sliderTwo, 0f, 255f, -0.505f, 0.505f);

                    axis.SetMinNormalizer(val);
                }

            }
        }

        void SetSliders()   // Snaps sliders to new axis values, plus short delay to allow them to move into place.

        {
            int SendOne = RemapToAxes(axis.MaxNormaliser, -0.505f, 0.505f, 0f, 255f);
            Wax.sendSlider(2, SendOne);

            int SendTwo = RemapToAxes(axis.MinNormaliser, -0.505f, 0.505f, 0f, 255f);
            Wax.sendSlider(1, SendTwo);
            sending = true;
            StartCoroutine(Delay(0.8f));
            
        }

        IEnumerator Delay(float time)
        {
            yield return new WaitForSeconds(time);
            sending = false;

        }
    IEnumerator MenuOffDelay()
    {
        yield return new WaitForSeconds(1f);
        menuActive = false;
        clicked = false;

    }

    IEnumerator ClickDelay()
    {
        clickedDelay = true;
        yield return new WaitForSeconds(0.5f);
        clickedDelay = false;
        sending = false;
      
        if (clicked)  // is still held down after one second?
        {
            SetMenu(true);
            menuActive = true;
           
        }
      //  yield return new WaitForSeconds(1f);
       
      //  menuDelay = true; //additional delay to not trigger new axis. 

    }

    void SetMenu(bool Active)
    {
        menuParent.SetActive(Active);
    }

    public float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;

        }
        public int RemapToAxes(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            float map = (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;

            return (int)map;
        }

        void GetInitialAxis()  
        {
        print("INITAL AXIS CALLED");
          float menuTextY = -3.4f + (Axis.AxisList.Count/2 );
        topPos = new Vector3(0, 0, menuTextY);
        bottomPos = new Vector3(0, 0, menuTextY);

            GameObject theAxis = GameObject.FindWithTag("Axis");
        print("theAxis is " +  theAxis.name);

        if (theAxis != null)
            {
                axis = theAxis.GetComponent<Axis>();
                found = true;
            /*
            for (int i = 0; i< Axis.AxisList.Count; i++ )
            {
                menuTextY += menuTextSpacing;
                if (i == (Axis.AxisList.Count/2 )+1)
                {
                    menuTextY = menuTextY - Axis.AxisList.Count * menuTextSpacing;
                }
                GameObject newText = Instantiate(menuText);  // TODO line these up with rotIndex;
                newText.name = Axis.AxisList[i].gameObject.name;w
                TextMenuControl textScript = newText.GetComponent<TextMenuControl>();
                textScript.index = i;
                    newText.transform.parent = menuParent.transform;
                    newText.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
                    newText.transform.localPosition = new Vector3(0.315f,  2f, menuTextY);
                menuTexts.Add(newText);
                if (newText.transform.localPosition.z>topPos.z)
                {
                    topPos = newText.transform.localPosition;
                }
                if (newText.transform.localPosition.z < bottomPos.z)
                {
                    bottomPos = newText.transform.localPosition;
                }
                

            }
            Destroy(menuText);
            print("Top is " + topPos);
            print("Bottom is " + bottomPos);
            */

           // Axis.CurrentAxis = Axis.AxisList[indx];
            CreateAxis(false);
            print("CurrentAxis is " + axis.name);
        }
            else
            {
                print("AxisNotFound");
            }
        }
    }

