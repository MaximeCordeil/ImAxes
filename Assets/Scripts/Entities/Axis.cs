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

public class Axis : MonoBehaviour {

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
    public static Quaternion AxisRot = new Quaternion();
    [Range(-0.505f, 0.505f)] public float MinFilter;
    [Range(-0.505f, 0.505f)] public float MaxFilter;
    [Range(-0.505f, 0.505f)] public float MinNormaliser;
    [Range(-0.505f, 0.505f)] public float MaxNormaliser;


    public bool isDirty;

    public bool isInSplom;

    bool isTweening;
    public int SourceIndex = -1;

    public class FilterEvent : UnityEvent<float, float> { };
    public FilterEvent OnFiltered = new FilterEvent();

    public class NormalizeEvent : UnityEvent<float, float> { };
    public NormalizeEvent OnNormalized = new NormalizeEvent();

    //ticker and file path (etc) for logging activity

    Vector2 AttributeRange;

    float ticksScaleFactor = 1.0f;

    // ghost properties
    public Axis ghostSourceAxis = null;

    // Axis infobox details
    public bool IsInfoboxEnabled = false;
    [Range(-0.505f, 0.505f)] public float InfoboxPosition;
    public TextMeshPro InfoboxText;
    private Renderer minFilterObjectRenderer;
    private Renderer maxFilterObjectRenderer;
    private Color originalMinFilterObjectColour;
    private Color originalMaxFilterObjectColour;
    public List<int> HighlightedIndices = new List<int>();

    public void HideHandles()
    {
        minFilterObject.localScale = Vector3.zero;
        maxFilterObject.localScale = Vector3.zero;
        // minNormaliserObject.localScale = Vector3.zero;
        // maxNormaliserObject.localScale = Vector3.zero;
    }

    public void Init(DataBinding.DataObject srcData, int idx, bool isPrototype = false)
    {
        SourceIndex = idx;
        axisId = idx;
        name = srcData.indexToDimension(idx);// changed name here js, removed axis
        tag = "axis";

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
              //  print (Mathf.Lerp(AttributeRange.x, AttributeRange.y, MaxNormaliser + 0.5f).ToString("0.000"));
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
        // all colliders from this object should ignore raycast
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var item in colliders)
        {
            item.gameObject.layer = 2;
        }

        minFilterObjectRenderer = minFilterObject.GetComponent<Renderer>();
        maxFilterObjectRenderer = maxFilterObject.GetComponent<Renderer>();
        originalMinFilterObjectColour = minFilterObjectRenderer.material.color;
        originalMaxFilterObjectColour = maxFilterObjectRenderer.material.color;
    }

#if UNITY_EDITOR    // Debug code for interacting with inspector
    private float prevMinFilter;
    private float prevMaxFilter;
    private float prevMinNormaliser;
    private float prevMaxNormaliser;
    private bool prevInfoboxToggle;
    private float prevInfoboxPosition;
    void Update()
    {
        if (prevMinFilter != MinFilter)
        {
            SetMinFilter(MinFilter);
            prevMinFilter = MinFilter;
        }
        if (prevMaxFilter != MaxFilter)
        {
            SetMaxFilter(MaxFilter);
            prevMaxFilter = MaxFilter;
        }
        if (prevMinNormaliser != MinNormaliser)
        {
            SetMinNormalizer(MinNormaliser);
            prevMinNormaliser = MinNormaliser;
        }
        if (prevMaxNormaliser != MaxNormaliser)
        {
            SetMaxNormalizer(MaxNormaliser);
            prevMaxNormaliser = MaxNormaliser;
        }
        if (prevInfoboxToggle != IsInfoboxEnabled)
        {
            ToggleInfobox(IsInfoboxEnabled);
            prevInfoboxToggle = IsInfoboxEnabled;
        }
        if (prevInfoboxPosition != InfoboxPosition)
        {
            SetInfoboxPosition(InfoboxPosition);
            prevInfoboxPosition = InfoboxPosition;
        }
    }
#endif

    public void LateUpdate()
    {
        isDirty = false;
    }

    void OnDestroy()
    {
        if (ghostSourceAxis != null)
        {
            ghostSourceAxis.OnFiltered.RemoveListener(Ghost_OnFiltered);
            ghostSourceAxis.OnNormalized.RemoveListener(Ghost_OnNormalized);
        }
    }

    public void SetMinFilter(float val)
    {
        MinFilter = Mathf.Clamp(val, -0.505f, 0.505f);
        OnFiltered.Invoke(MinFilter, MaxFilter);

        SetLocalYPosition(minFilterObject.transform, UtilMath.normaliseValue(MinFilter, -0.505f, 0.505f, -0.5f, 0.5f));
    }

    public void SetMaxFilter(float val)
    {
        MaxFilter = Mathf.Clamp(val, -0.505f, 0.505f);
        OnFiltered.Invoke(MinFilter, MaxFilter);

        SetLocalYPosition(maxFilterObject.transform, UtilMath.normaliseValue(MaxFilter, -0.505f, 0.505f, -0.5f, 0.5f));
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

    public void ToggleInfobox(bool toggle)
    {
        IsInfoboxEnabled = toggle;
        InfoboxText.enabled = toggle;
        InfoboxText.verticalAlignment = VerticalAlignmentOptions.Top;
        // Toggle visibility of all other labels
        minimumValueDimensionLabel.enabled = !toggle;
        maximumValueDimensionLabel.enabled = !toggle;
        maximumValueDimensionLabel.enabled = !toggle;

        if (toggle)
        {
            // Use the filter objects to mark the infobox location
            SetInfoboxPosition(InfoboxPosition);
            minFilterObjectRenderer.material.color = new Color(0, 0.4f, 0);
            maxFilterObjectRenderer.material.color = new Color(0, 0.4f, 0);
        }
        else
        {
            // Reset filter object positions
            minFilterObjectRenderer.material.color = originalMinFilterObjectColour;
            maxFilterObjectRenderer.material.color = originalMaxFilterObjectColour;
            SetMinFilter(MinFilter);
            SetMaxFilter(MaxFilter);

            HighlightedIndices.Clear();
            List<Visualization> visualizations = correspondingVisualizations();
            if (visualizations.Count > 0)
            {
                visualizations[0].FilteredPointsChanged = true;
            }

        }
    }

    public void SetInfoboxPosition(float val)
    {
        if (!IsInfoboxEnabled)
            return;

        InfoboxPosition = Mathf.Clamp(val, -0.505f, 0.505f);
        float normalised = UtilMath.normaliseValue(InfoboxPosition, -0.505f, 0.505f, 0, 1f);
        string dimension = SceneManager.Instance.dataObject.indexToDimension(SourceIndex);

        // Set infobox text
        // Get the array of filtered points from any corresponding visualisations
        List<Visualization> visualizations = correspondingVisualizations();
        if (visualizations.Count == 0)
            return;
        float[] filteredPoints = visualizations[0].SharedFilteredPoints;
        // Get the array of float values for this dimension
        float[] values = SceneManager.Instance.dataObject.getDimension(dimension);
        // Keep only the indices which are not marked as 1 in the filteredPoints array
        List<float> filteredValues = new List<float>();
        for (int i = 0; i < filteredPoints.Length; i++)
        {
            if (filteredPoints[i] == 0)
                filteredValues.Add(values[i]);
        }

        if (filteredValues.Count == 0)
        {
            InfoboxText.text = string.Format("<size=16><b>{0}</b></size><br>{1}", dimension, "No points to view!");
            visualizations[0].FilteredPointsChanged = true;
        }
        else
        {
            float nearestValue = UtilMath.ClosestTo(filteredValues, normalised);
            string outputValue = "";

            string type = SceneManager.Instance.dataObject.TypeDimensionDictionary1[SourceIndex];
            if (type == "float")
            {
                outputValue = Mathf.Lerp(AttributeRange.x, AttributeRange.y, nearestValue).ToString();
            }
            else if (type == "string")
            {
                float value = Mathf.Lerp(AttributeRange.x, AttributeRange.y, nearestValue);
                float textualDimensionIdx = UtilMath.ClosestTo(SceneManager.Instance.dataObject.TextualDimensions.Keys.ToList(), value);
                outputValue = SceneManager.Instance.dataObject.TextualDimensions[textualDimensionIdx].ToString();
            }


            HighlightedIndices.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == nearestValue)
                    HighlightedIndices.Add(i);
            }
            string overlappingText = (HighlightedIndices.Count > 1) ? HighlightedIndices.Count + " overlapping points" : "";

            InfoboxText.text = string.Format("<size=10><b>{0}</b></size><br><size=16>{1}</size><br><size=8><i>{2}</i></size>", dimension, outputValue, overlappingText);

            // Tell a connected axis to change the highlighted points
            visualizations[0].FilteredPointsChanged = true;
        }

        // Set infobox position, as well as the filter handles
        float y = UtilMath.normaliseValue(InfoboxPosition, -0.505f, 0.505f, -0.5f, 0.5f);
        SetLocalYPosition(InfoboxText.transform, y);
        SetLocalYPosition(minFilterObject, y);
        SetLocalYPosition(maxFilterObject, y);
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

    public bool IsPerpendicular(Axis axis)
    {
        // return Vector3.Dot(Up, axis.Up) > -0.2f && Vector3.Dot(Up, axis.Up) < 0.2f;
        return Vector3.Dot(Up, axis.Up) > -0.4f && Vector3.Dot(Up, axis.Up) < 0.4f;
    }

    public bool IsParallel(Axis axis)
    {
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
        ghostSourceAxis = sourceAxis;

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

    private void SetLocalYPosition(Transform t, float y)
    {
        var pos = t.localPosition;
        pos.y = y;
        t.localPosition = pos;
    }
}