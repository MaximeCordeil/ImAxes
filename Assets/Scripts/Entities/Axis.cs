using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using Staxes;
using UnityEngine.Events;
using System.Linq;
using System.IO;

public class Axis : MonoBehaviour, Grabbable {

    [SerializeField] TextMeshPro label;
    [SerializeField] TextMeshPro minimumValueDimensionLabel;
    [SerializeField] TextMeshPro maximumValueDimensionLabel;

    public int axisId;

    public bool isPrototype;

    //temporary hack 

    Vector3 originPosition;
    Quaternion originRotation;

    [SerializeField] Transform minFilterObject;
    [SerializeField] Transform maxFilterObject;

    [SerializeField] Transform minNormaliserObject;
    [SerializeField] Transform maxNormaliserObject;

    [SerializeField] Renderer ticksRenderer;

    [Space(10)]

    [SerializeField] UnityEvent OnEntered;
    [SerializeField] UnityEvent OnExited;

    public HashSet<Axis> ConnectedAxis = new HashSet<Axis>();

    public float MinFilter;
    public float MaxFilter;

    public float MinNormaliser;
    public float MaxNormaliser;

    public bool isDirty;

    public bool isInSplom;

    bool isTweening;
    public int SourceIndex = -1;

    public class FilterEvent : UnityEvent<float, float> { };
    public FilterEvent OnFiltered = new FilterEvent();

    public class NormalizeEvent : UnityEvent<float, float> { };
    public NormalizeEvent OnNormalized = new NormalizeEvent();

    //ticker and file path (etc) for logging activity
   
    SteamVR_TrackedObject trackedObject;
    List<Vector3> tracking = new List<Vector3>();
    
    Vector2 AttributeRange;

    float ticksScaleFactor = 1.0f;

    // ghost properties
    Axis ghostSourceAxis = null;


    public void Init(DataBinding.DataObject srcData, int idx, bool isPrototype = false)
    {
        SourceIndex = idx;
        axisId = idx;
        name = "axis " + srcData.indexToDimension(idx);


        AttributeRange = srcData.DimensionsRange[axisId];
        label.text = srcData.Identifiers[idx];
        UpdateRangeText();

        this.isPrototype = isPrototype;

        CalculateTicksScale(srcData);
        UpdateTicks();
    }

    void UpdateRangeText()
    {
        string type = SceneManager.Instance.dataObject.TypeDimensionDictionary1[SourceIndex];

        if (type == "float")
        {
            minimumValueDimensionLabel.text = Mathf.Lerp(AttributeRange.x, AttributeRange.y, MinNormaliser + 0.5f).ToString("0.000");
            maximumValueDimensionLabel.text = Mathf.Lerp(AttributeRange.x, AttributeRange.y, MaxNormaliser + 0.5f).ToString("0.000");
        }

        else if (type == "string")
        {
            float minValue = Mathf.Lerp(AttributeRange.x, AttributeRange.y, MinNormaliser + 0.5f);
            float maxValue = Mathf.Lerp(AttributeRange.x, AttributeRange.y, MaxNormaliser + 0.5f);

            float nearestMinValue = UtilMath.ClosestTo(SceneManager.Instance.dataObject.TextualDimensions.Keys.ToList(), minValue);
            float nearestMaxValue = UtilMath.ClosestTo(SceneManager.Instance.dataObject.TextualDimensions.Keys.ToList(), maxValue);

            minimumValueDimensionLabel.text = SceneManager.Instance.dataObject.TextualDimensions[nearestMinValue].ToString();
            maximumValueDimensionLabel.text = SceneManager.Instance.dataObject.TextualDimensions[nearestMaxValue].ToString();
        }
    }


    void CalculateTicksScale(DataBinding.DataObject srcData)
    {
        float range = AttributeRange.y - AttributeRange.x;
        if (srcData.Metadata[axisId].binCount > range + 2)
        {
            ticksScaleFactor = 1.0f / (srcData.Metadata[axisId].binCount / 10);
        }
        else if (range < 20)
        {
            // each tick mark represents one increment
            ticksScaleFactor = 1;
        }
        else if (range < 50)
        {
            ticksScaleFactor = 5;
        }
        else if (range < 200)
        {
            // each tick mark represents ten increment
            ticksScaleFactor = 10;
        }
        else if (range < 600)
        {
            ticksScaleFactor = 50;
        }
        else if (range < 3000)
        {
            ticksScaleFactor = 100;
        }
        else
        {
            ticksScaleFactor = 500;
        }
    }

    void UpdateTicks()
    {

        float range = Mathf.Lerp(AttributeRange.x, AttributeRange.y, MaxNormaliser + 0.5f) - Mathf.Lerp(AttributeRange.x, AttributeRange.y, MinNormaliser + 0.5f);
        float scale = range / ticksScaleFactor;
        ticksRenderer.material.mainTextureScale = new Vector3(1, scale);
    }

    public void setDebug(string dbg)
    {
        DataBinding.DataObject srcData = SceneManager.Instance.dataObject;
        label.text = srcData.Identifiers[axisId] + "(" + dbg + ")";
    }

    public void InitOrigin(Vector3 originPosition, Quaternion originRotation)
    {
        this.originPosition = originPosition;
        this.originRotation = originRotation;
    }

    void Start()
    {

        //all colliders from this object should ignore raycast
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var item in colliders)
        {
            item.gameObject.layer = 2;
        }       

    }

    void OnDestroy()
    {
        if (ghostSourceAxis != null)
        {
            ghostSourceAxis.OnFiltered.RemoveListener(Ghost_OnFiltered);
            ghostSourceAxis.OnNormalized.RemoveListener(Ghost_OnNormalized);
        }
    }

    public void Update()
    {
        if (isPrototype)
        {
            if (Vector3.Distance(originPosition, transform.position) > 0.25f)
            {
                isPrototype = false;
                GameObject clone = Clone();
                clone.GetComponent<Axis>().OnExited.Invoke();
                clone.GetComponent<Axis>().ReturnToOrigin();

                SceneManager.Instance.AddAxis(clone.GetComponent<Axis>());
                
                foreach (var obj in GameObject.FindObjectsOfType<WandController>())
                {
                    if (obj.IsDragging())
                        obj.Shake();
                }
            }
        }
        
    }

    public void LateUpdate()
    {
        isDirty = false;
    }

    public void SetMinFilter(float val)
    {
        MinFilter = val;
        OnFiltered.Invoke(MinFilter, MaxFilter);
    }

    public void SetMaxFilter(float val)
    {
        MaxFilter = val;
        OnFiltered.Invoke(MinFilter, MaxFilter);
    }

    public void SetMinNormalizer(float val)
    {
        MinNormaliser = Mathf.Clamp(val, -0.505f, 0.505f);
        UpdateRangeText();
        OnNormalized.Invoke(MinNormaliser, MaxNormaliser);
        UpdateTicks();
    }

    public void SetMaxNormalizer(float val)
    {
        MaxNormaliser = Mathf.Clamp(val, -0.505f, 0.505f);
        UpdateRangeText();
        OnNormalized.Invoke(MinNormaliser, MaxNormaliser);
        UpdateTicks();
    }

    public GameObject Clone()
    {
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation, null);
        Axis axis = clone.GetComponent<Axis>();
        axis.InitOrigin(originPosition, originRotation);
        axis.ticksRenderer.material = Instantiate(ticksRenderer.material) as Material;

        return clone;
    }

    public GameObject Dup(GameObject go, Vector3 tp, Quaternion tr)
    {
        GameObject clone = Instantiate(go, tp, tr, null);
        Axis axis = clone.GetComponent<Axis>();
        axis.InitOrigin(originPosition, originRotation);
        axis.ticksRenderer.material = Instantiate(ticksRenderer.material) as Material;

        return clone;
    }



    #region euclidan functions

    // calculates the project of the transform tr (assumed to be the user's hand) onto the axis
    // as a float between 0...1
    public float CalculateLinearMapping(Transform tr)
    {
        Vector3 direction = MaxPosition - MinPosition;
        float length = direction.magnitude;
        direction.Normalize();

        Vector3 displacement = tr.position - MinPosition;

        return Vector3.Dot(displacement, direction) / length;
    }

    public bool IsHorizontal
    {
        get
        {
            float dp = Vector3.Dot(this.Up, Vector3.up);
            return dp > -0.25f && dp < 0.25f;
        }
    }

    public bool IsVertical
    {
        get
        {
            float dp = Vector3.Dot(this.Up, Vector3.up);
            return dp > 0.9f || dp < -0.9f;

        }
    }
    public bool isPerependicular(Axis axis)
    {
        return Vector3.Dot(Up, axis.Up) > -0.2f && Vector3.Dot(Up, axis.Up) < 0.2f;
    }

    public bool IsParallel(Axis axis) {
        return Vector3.Dot(Up, axis.Up) > 0.5f;
    }

    public bool IsColinear(Axis axis)
    {
        if (axis.IsHorizontal)
        {
            return Vector3.Dot(Up, axis.Up) > 0.95f;// 0.1f && Vector3.Dot(Up, axis.Up) > -0.1f;
        }
        else { return Vector3.Dot(Up, axis.Up) > 0.95f; }
    }

    public Vector3 Up
    {
        get { return transform.TransformDirection(Vector3.up); }
    }

    Vector3 _maxPos;
    public Vector3 MaxPosition
    {
        get { return _maxPos; }
    }

    Vector3 _minPos;
    public Vector3 MinPosition
    {
        get { return _minPos; }
    }

    public void UpdateCoords()
    {
        _minPos = transform.TransformPoint(Vector3.down * 0.51f);
        _maxPos = transform.TransformPoint(Vector3.up * 0.51f);
    }

    public float Distance(Axis axes)
    {
        Vector3 pos_a = transform.position;
        Vector3 pos_b = axes.transform.position;
        return Vector3.Distance(pos_a, pos_b);
    }

    // returns the top and bottom points of this axis in world coordinates
    public List<Vector3> Points()
    {
        return new List<Vector3> {
            transform.TransformPoint(Vector3.up * 0.5f),
            transform.TransformPoint(Vector3.down * 0.5f)
            };
    }

    #endregion

    int Grabbable.GetPriority()
    {
        return 5;
    }

    public bool OnGrab(WandController controller)
    {
        if (!isTweening)
        {
            transform.parent = controller.transform;
            transform.DOKill();
        }
        GetComponent<Rigidbody>().isKinematic = true;
        isDirty = true;
        return true;
    }

    public void OnRelease(WandController controller)
    {
        transform.parent = null;

        if (!isPrototype)
        {
            // destroy the axis
            if (controller.Velocity.magnitude > 0.2f)
            {
                Rigidbody body = GetComponent<Rigidbody>();
                body.isKinematic = false;
                body.useGravity = true;
                body.AddForce(controller.Velocity * -1000);
                gameObject.layer = LayerMask.NameToLayer("TransparentFX");

                transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack);

                return;
            }
        }
        else
        {
            // return the axis to its position
            ReturnToOrigin();
        }

        List<Visualization> lv = correspondingVisualizations();

        foreach (var visu in lv)
        {
            if (visu.viewType == Visualization.ViewType.Scatterplot2D)
            {
                var haxis = visu.ReferenceAxis1.horizontal;
                var vaxis = visu.ReferenceAxis1.vertical;

                var vu = vaxis.Up;
                var hu = haxis.Up;

                Vector3.OrthoNormalize(ref vu, ref hu);

                var q1 = Quaternion.LookRotation(-Vector3.Cross(vu, hu), vu);
                var q2 = Quaternion.LookRotation(Vector3.Cross(vu, hu), hu);

                // find out which direction the horizontal is facing
                var urvec = Vector3.Cross(-Vector3.Cross(vu, hu), vu);
                float d = Vector3.Dot(urvec, (haxis.transform.position - vaxis.transform.position));

                // determine the position of the horizontal axis
                Vector3 hpos = vaxis.transform.position +
                    -vu * vaxis.transform.localScale.y * 0.5f +
                    -Mathf.Sign(d) * hu * haxis.transform.localScale.y * 0.5f;

                vaxis.AnimateTo(vaxis.transform.position, q1);
                haxis.AnimateTo(hpos, q2);
            }
            else if (visu.viewType == Visualization.ViewType.Scatterplot3D)
            {
                if (visu != null && visu.ReferenceAxis1.vertical != null && visu.ReferenceAxis1.horizontal != null && visu.ReferenceAxis1.depth != null)
                {
                    var haxis = visu.ReferenceAxis1.horizontal;
                    var vaxis = visu.ReferenceAxis1.vertical;
                    var daxis = visu.ReferenceAxis1.depth;

                    var vu = vaxis.Up;
                    var hu = haxis.Up;
                    var du = daxis.Up;

                    Vector3.OrthoNormalize(ref vu, ref hu, ref du);

                    var q1 = Quaternion.LookRotation(-du, vu);
                    var q2 = Quaternion.LookRotation(du, hu);
                    var q3 = Quaternion.LookRotation(-hu, du);

                    Vector3 hpos = vaxis.transform.position +
                        -vu * vaxis.transform.localScale.y * 0.5f +
                        hu * haxis.transform.localScale.y * 0.5f;

                    Vector3 dpos = vaxis.transform.position +
                        -vu * vaxis.transform.localScale.y * 0.5f +
                        du * daxis.transform.localScale.y * 0.5f;

                    vaxis.AnimateTo(vaxis.transform.position, q1);
                    haxis.AnimateTo(hpos, q2);
                    daxis.AnimateTo(dpos, q3);                    
                }
            }
        } // end for each 

        // align this axis correctly to the SPLOM
        foreach (SPLOM3D splom in CorrespondingSPLOMS())
        {
            splom.AlignAxisToSplom(this);
        }

        GetComponent<Rigidbody>().isKinematic = false;
        isDirty = false;
    }

    public void OnDrag(WandController controller)
    {
        isDirty = true;
    }

    public void OnEnter(WandController controller)
    {
        OnEntered.Invoke();
    }

    public void OnExit(WandController controller)
    {
        OnExited.Invoke();
    }

    void ReturnToOrigin()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(originRotation.eulerAngles, 0.7f, RotateMode.Fast).SetEase(Ease.OutSine));
        seq.Join(transform.DOMove(originPosition, 0.7f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() => GetComponent<Axis>().isPrototype = true);

        foreach (var c in GetComponentsInChildren<ProximityDetector>())
        {
            c.ForceExit();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name + "  " + collision.contacts[0].ToString());
    }

    void OnCollisionExit(Collision collision)
    {

    }

    public List<Visualization> correspondingVisualizations()
    {
        return GameObject.FindObjectsOfType<Visualization>().Where(x => x.axes.Contains(this)).ToList();
    }

    public List<SPLOM3D> CorrespondingSPLOMS()
    {
        return FindObjectsOfType<SPLOM3D>().Where(x => x.Axes.Contains(this)).ToList();
    }

    public void OnApplicationQuit()
    {
        
    }

    public void Ghost(Axis sourceAxis)
    {
        sourceAxis.OnFiltered.AddListener(Ghost_OnFiltered);
        sourceAxis.OnNormalized.AddListener(Ghost_OnNormalized);

        foreach (Renderer r in transform.GetComponentsInChildren<Renderer>(true))
        {
            r.enabled = false;
        }
        foreach (Collider c in transform.GetComponentsInChildren<Collider>(true))
        {
            c.enabled = false;
        }
    }

    void Ghost_OnFiltered(float minFilter, float maxFilter)
    {
        MinFilter = minFilter;
        MaxFilter = maxFilter;
        OnFiltered.Invoke(MinFilter, MaxFilter);
    }

    void Ghost_OnNormalized(float minNorm, float maxNorm)
    {
        MinNormaliser = minNorm;
        MaxNormaliser = maxNorm;
        OnNormalized.Invoke(MinNormaliser, MaxNormaliser);
    }

    public void AnimateTo(Vector3 pos, Quaternion rot)
    {
        transform.DORotateQuaternion(rot, 0.4f).SetEase(Ease.OutBack);
        transform.DOMove(pos, 0.4f).SetEase(Ease.OutBack);        
    }

}