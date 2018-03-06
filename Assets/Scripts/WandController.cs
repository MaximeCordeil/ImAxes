using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Linq;


public interface Grabbable
{
    int GetPriority();

    // return true if this grabbable is actually grabbable and should attach to the controller
    bool OnGrab(WandController controller);
    void OnRelease(WandController controller);
    void OnDrag(WandController controller);

    void OnEnter(WandController controller);
    void OnExit(WandController controller);
}

public interface Brushable
{
    void OnBrush(WandController controller, Vector3 position, bool is3D);
    void OnBrushRelease(WandController controller);
    void OnDetailOnDemand(WandController controller, Vector3 position, Vector3 localPosition);
    void OnDetailOnDemandRelease(WandController controller);

}

public class WandController : MonoBehaviour
{
    public OVRInput.Controller OculusController;

    public bool isOculusRift = false;
    //Debug test
    GameObject brushingPoint;

    Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    bool isTouchDown;

    SteamVR_TrackedObject trackedObject;
    SteamVR_Controller.Device controller;
    
    Collider intersectingCollider;
    List<Collider> intersectingGrabbables = new List<Collider>();
    
    List<GameObject> draggingObjects = new List<GameObject>();

    Collider brushableCollider;

    List<Vector3> tracking = new List<Vector3>();

    //touch pad interaction
    float previousYValuePad = 0f;
    float incrementYValuePad = 0f;
    float yvaluePadTouchDown = 0f;

    GameObject currentBrushView = null;
    GameObject currentDetailView = null;

    public Material theBrushingMaterial;

    public Vector3 Velocity
    {
        get
        {
            return tracking[0] - tracking[tracking.Count - 1];
        }
    }

    void Start()
    {
        if (!isOculusRift) controller = SteamVR_Controller.Input((int)trackedObject.index); 
    
        brushingPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.0f);

        brushingPoint.GetComponent<SphereCollider>().enabled = false;// isTrigger = false;
        brushingPoint.GetComponent<MeshRenderer>().material = theBrushingMaterial;

    }

    void Awake()
    {

        trackedObject = GetComponent<SteamVR_TrackedObject>();
        tracking.AddRange(Enumerable.Repeat<Vector3>(Vector3.zero, 10));
    }

    public void PropergateOnGrab(GameObject g)
    {
        if (g.GetComponent<Grabbable>() != null && g.GetComponent<Grabbable>().OnGrab(this))
        {
            draggingObjects.Add(g.gameObject);
        }
    }

    void Update()
    {
        bool gripDown = isOculusRift?
            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OculusController)
            : controller.GetPressDown(gripButton);

        bool gripUp = isOculusRift ?
            OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OculusController)
            : controller.GetPressUp(gripButton);

        bool gripping = isOculusRift ?
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger, OculusController)
            : controller.GetPress(gripButton);

        //bool upButtonDown = isOculusRift?
        //    OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, OculusController) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown, OculusController)
        //    : 
        if (gripDown && intersectingGrabbables.Any(x => x!= null) && draggingObjects.Count == 0)
        {
            var potentialDrags = intersectingGrabbables.Where(x => x != null).ToList();
            potentialDrags.Sort((x, y) => y.GetComponent<Grabbable>().GetPriority() - x.GetComponent<Grabbable>().GetPriority());
            if (potentialDrags.Count() > 0)
            {
                PropergateOnGrab(potentialDrags.First().gameObject);
            }            
        }
        else if (gripUp && draggingObjects.Count > 0)
        {
            draggingObjects.Where(x => x != null).ForEach(x => x.GetComponent<Grabbable>().OnRelease(this));
            draggingObjects.Clear();
        }
        else if (gripping && draggingObjects.Count > 0)
        {
            draggingObjects.Where(x => x != null).ForEach(x => x.GetComponent<Grabbable>().OnDrag(this));            
        }
        
        if (draggingObjects.Count > 0)
        {
            if(!isOculusRift)
            controller.TriggerHapticPulse(100);
        }

        //brush actions : SteamVR_Controller.ButtonMask.Grip

        bool padPressDown = isOculusRift ? OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OculusController) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OculusController)
           : controller.GetPress(padButton);

        bool padPressUp = isOculusRift ? OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OculusController) || OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick, OculusController)
          : controller.GetPressUp(padButton);
        
        #region details on demand
        //detail on demand actions
        if (VisualisationAttributes.detailsOnDemand)
        {
            if (padPressDown)
            {
                bool detail3Dscatterplots = false;
                GameObject[] listCandidatesBrush3D = GameObject.FindGameObjectsWithTag("Scatterplot3D");
                for (int i = 0; i < listCandidatesBrush3D.Length; i++)
                {
                    {
                        if (Vector3.Distance(listCandidatesBrush3D[i].transform.position, transform.position) < 0.3f)
                        {
                            detail3Dscatterplots = true;
                            brushingPoint.gameObject.SetActive(true);

                            currentDetailView = listCandidatesBrush3D[i];
                            brushingPoint.transform.position = transform.position + transform.forward * 0.1f;
                            brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            if (currentDetailView.GetComponent<Visualization>() != null)
                            {
                                currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(this, 
                                    brushingPoint.transform.position, 
                                    currentDetailView.transform.InverseTransformPoint(brushingPoint.transform.position),
                                    true);
                            }
                            else
                            {
                                Debug.Log("the object is null/...");
                            }
                        }
                    }
                }
                if (!detail3Dscatterplots)
                {
                    RaycastHit hit;
                    Ray downRay = new Ray(transform.position, transform.forward);
                    if (Physics.Raycast(downRay, out hit))
                    {
                        if (hit.transform.gameObject.GetComponent<Brushable>() != null)
                        {
                            brushingPoint.gameObject.SetActive(true);
                            currentDetailView = hit.transform.gameObject;
                            brushingPoint.transform.position = hit.point;
                            brushingPoint.transform.rotation = currentDetailView.transform.rotation;
                            brushingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.0f);
                            
                            currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(
                                this, 
                                hit.point, 
                                currentDetailView.transform.InverseTransformPoint(hit.point),
                                false);
                        }

                    }
                }
            }
            if (padPressUp)
            {
                if (currentDetailView != null)
                {
                    currentDetailView.GetComponent<Visualization>().OnDetailOnDemand(null, Vector3.zero, Vector3.zero,false);

                    currentDetailView.GetComponent<Visualization>().OnDetailOnDemandRelease(this);
                    currentDetailView = null;
                    brushingPoint.gameObject.SetActive(false);

                }
            }
        }
#endregion
        
        tracking.RemoveAt(0);
        tracking.Add(transform.TransformPoint(new Vector3(0, -0.04f, 0)));

    }

    void OnTriggerEnter(Collider col)
    {
        if (draggingObjects.Count > 0)
            return;

        var grabble = col.GetComponent<Grabbable>();
        if (grabble != null && !intersectingGrabbables.Contains(col))
        {
            Collider activeGrabbable = intersectingGrabbables.FirstOrDefault();
            intersectingGrabbables.Add(col);
            intersectingGrabbables.RemoveAll(x => x == null);
            intersectingGrabbables.Sort((x, y) => y.GetComponent<Grabbable>().GetPriority() - x.GetComponent<Grabbable>().GetPriority());
            if (intersectingGrabbables[0] == col){
                if (activeGrabbable != null && activeGrabbable != intersectingGrabbables[0])
                {
                    activeGrabbable.GetComponent<Grabbable>().OnExit(this);
                }
                grabble.OnEnter(this);
            }
        }
        if (col.GetComponent<Brushable>() != null)
        {
            brushableCollider = col;
        }
    }

    void OnTriggerExit(Collider col)
    {
        intersectingGrabbables.RemoveAll(x => x == null);

        var grabbable = col.GetComponent<Grabbable>();
        if (grabbable != null && intersectingGrabbables.Contains(col))
        {
            if (col == intersectingGrabbables[0]){
                grabbable.OnExit(this);
                intersectingGrabbables.RemoveAt(0);
                if (intersectingGrabbables.Count > 0){
                    intersectingGrabbables[0].GetComponent<Grabbable>().OnEnter(this);
                }
            } else {
                intersectingGrabbables.Remove(col);
            }
            brushableCollider = null;
        }
    }

    void SetIntersectingCollider(Collider col)
    {
        if (col != null){
            if (intersectingCollider != null && col.GetComponent<Grabbable>().GetPriority() >= intersectingCollider.GetComponent<Grabbable>().GetPriority())
            {
                intersectingCollider.GetComponent<Grabbable>().OnExit(this);
            }
            intersectingCollider = col;
            intersectingCollider.GetComponent<Grabbable>().OnEnter(this);
            
        } else {
            intersectingCollider.GetComponent<Grabbable>().OnExit(this);
            intersectingCollider = null;
        }        
    }

    public bool IsDragging(Grabbable grab)
    {
        return draggingObjects.Any(x => x.GetComponent<Grabbable>() == grab);
    }

    public bool IsDragging()
    {
        return draggingObjects.Count > 0;
    }

    IEnumerator ShakeCoroutine()
    {
        for (int i = 0; i < 15; ++i)
        {
            controller.TriggerHapticPulse((ushort)(3900 * (15 - i) / 15.0f));
            yield return new WaitForEndOfFrame();
        }
    }

    //length is how long the vibration should go for
    //strength is vibration strength from 0-1
    IEnumerator TriggerHaptics(float length, float strength) {
        if (!isOculusRift)
        {
            for (float i = 0; i < length; i += Time.deltaTime)
            {
                controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void Shake()
    {
        if (!isOculusRift)
        StartCoroutine(ShakeCoroutine());
    }

    public void OnApplicationQuit()
    {
       
    }

}
