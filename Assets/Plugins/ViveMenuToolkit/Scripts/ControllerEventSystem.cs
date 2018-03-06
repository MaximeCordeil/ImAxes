using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ViveMenuControllerEvents
{
    public bool Active { get; set; }

    public UnityEvent TriggerClicked = new UnityEvent();
    public UnityEvent TriggerUnclicked = new UnityEvent();
    public UnityEvent PadClicked = new UnityEvent();
    public UnityEvent PadUnclicked = new UnityEvent();
    public UnityEvent MenuButtonClicked = new UnityEvent();
    public UnityEvent MenuButtonUnclicked = new UnityEvent();
    public UnityEvent PadTouched = new UnityEvent();
    public UnityEvent PadUntouched = new UnityEvent();
    public UnityEvent SwipeUp = new UnityEvent();
    public UnityEvent SwipeDown = new UnityEvent();
    public UnityEvent SwipeLeft = new UnityEvent();
    public UnityEvent SwipeRight = new UnityEvent();


    public UnityEvent PadClickUp = new UnityEvent();
    public UnityEvent PadClickDown = new UnityEvent();
    public UnityEvent PadClickLeft = new UnityEvent();
    public UnityEvent PadClickRight = new UnityEvent();


    public UnityEvent PadTouchDown = new UnityEvent();
    public UnityEvent PadTouchUp = new UnityEvent();
    public UnityEvent PadTouchLeft = new UnityEvent();
    public UnityEvent PadTouchRight = new UnityEvent();


}

public class ControllerEventSystem : MonoBehaviour
{
    private List<ViveMenuControllerEvents> MenuEvents = new List<ViveMenuControllerEvents>();

    private SteamVR_TrackedController trackedController;
    SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device device;

    // For swiping.
    private readonly Vector2 mXAxis = new Vector2(1, 0);
    private readonly Vector2 mYAxis = new Vector2(0, 1);
    private bool trackingSwipe = false;
    private bool checkSwipe = false;

    // The angle range for detecting swipe
    private const float mAngleRange = 30;

    // To recognize as swipe user should at lease swipe for this many pixels
    private const float mMinSwipeDist = 0.2f;

    // To recognize as a swipe the velocity of the swipe
    // should be at least mMinVelocity
    // Reduce or increase to control the swipe speed
    private const float mMinVelocity = 4.0f;
    private Vector2 mStartPosition;
    private Vector2 endPosition;
    private float mSwipeStartTime;

    Vector2 touchPad;

    void Start()
    {
        this.controller = GetComponent<SteamVR_TrackedObject>();
        this.trackedController = GetComponent<SteamVR_TrackedController>();

        // Trigger Click.
        trackedController.TriggerClicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.TriggerClicked.Invoke();
            });
        };

        trackedController.TriggerUnclicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.TriggerUnclicked.Invoke();
            });
        };

        // Pad click.
        trackedController.PadClicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.PadClicked.Invoke();
            });
        };

        // Pad click.
        trackedController.PadUnclicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.PadUnclicked.Invoke();
            });
        };

        // Menu Button.
        trackedController.MenuButtonClicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.MenuButtonClicked.Invoke();
            });
        };

        trackedController.MenuButtonUnclicked += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.MenuButtonUnclicked.Invoke();
            });
        };

        trackedController.PadTouched += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.PadTouched.Invoke();
            });
        };

        trackedController.PadUntouched += (o, a) =>
        {
            MenuEvents.ForEach(x =>
            {
                if (x.Active) x.PadUntouched.Invoke();
            });
        };
    }

    public void checkPadClicked()
    {

        //Get pressdown for click and GetTouch for touch
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Read the touchpad values
            touchPad = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
            if (-0.5f < touchPad.x && touchPad.x < 0.5f && touchPad.y > 0.5f)
            {
                MenuEvents.ForEach(x =>
                {
                    if (x.Active) x.PadClickUp.Invoke();
                });
            }


            if (-0.5f < touchPad.x && touchPad.x < 0.5f && touchPad.y < -0.5f)
            {
                Debug.Log("Down clicked");
                MenuEvents.ForEach(x =>
                {
                    if (x.Active) x.PadClickDown.Invoke();
                });

            }

            if (-0.5f < touchPad.y && touchPad.y < 0.5f && touchPad.x < -0.5f)
            {

                MenuEvents.ForEach(x =>
                {
                    if (x.Active) x.PadClickLeft.Invoke();
                });
            }

            if (-0.5f < touchPad.y && touchPad.y < 0.5f && touchPad.x > 0.5f)
            {
                MenuEvents.ForEach(x =>
                {
                    if (x.Active) x.PadClickRight.Invoke();
                });
            }
        }

    }

    public void AddViveMenuEvents(ViveMenuControllerEvents e)
    {
        this.MenuEvents.Add(e);
    }

    void Update()
    {
        if (controller == null || trackedController == null) { return; }

        device = SteamVR_Controller.Input((int)controller.index);
        // Touch down, possible chance for a swipe
        if ((int)controller.index != -1 && device.GetTouchDown(Valve.VR.EVRButtonId.k_EButton_Axis0))
        {
            trackingSwipe = true;
            // Record start time and position
            mStartPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);
            mSwipeStartTime = Time.time;
        }
        // Touch up , possible chance for a swipe
        else if (device.GetTouchUp(Valve.VR.EVRButtonId.k_EButton_Axis0))
        {
            trackingSwipe = false;
            trackingSwipe = true;
            checkSwipe = true;
        }
        else if (trackingSwipe)
        {
            endPosition = new Vector2(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x,
                                      device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y);

        }

        if (checkSwipe)
        {
            checkSwipe = false;

            float deltaTime = Time.time - mSwipeStartTime;
            Vector2 swipeVector = endPosition - mStartPosition;

            float velocity = swipeVector.magnitude / deltaTime;
            if (velocity > mMinVelocity &&
                swipeVector.magnitude > mMinSwipeDist)
            {
                // if the swipe has enough velocity and enough distance
                swipeVector.Normalize();

                float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                // Detect left and right swipe
                if (angleOfSwipe < mAngleRange)
                {
                    MenuEvents.ForEach(x =>
                    {
                        if (x.Active) x.SwipeRight.Invoke();
                    });
                }
                else if ((180.0f - angleOfSwipe) < mAngleRange)
                {
                    MenuEvents.ForEach(x =>
                    {
                        if (x.Active) x.SwipeLeft.Invoke();
                    });
                }
                else
                {
                    // Detect top and bottom swipe
                    angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                    if (angleOfSwipe < mAngleRange)
                    {
                        MenuEvents.ForEach(x =>
                        {
                            if (x.Active) x.SwipeUp.Invoke();
                        });
                    }
                    else if ((180.0f - angleOfSwipe) < mAngleRange)
                    {
                        MenuEvents.ForEach(x =>
                        {
                            if (x.Active) x.SwipeDown.Invoke();
                        });
                    }
                }
            }
        }
        checkPadClicked();
    }
}
