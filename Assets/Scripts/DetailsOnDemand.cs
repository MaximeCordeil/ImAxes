using Staxes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetailsOnDemand : MonoBehaviour
{

    Vector3 Center = Vector3.zero;
    Vector3 pointerPosition;
    Vector3 localPointerPosition;

    int maxDetails = 1;
    GameObject labelDetails;
    GameObject textMesh;

    Vector3 tl = Vector3.zero;
    Vector3 tr = Vector3.zero;
    Vector3 bl = Vector3.zero;
    Vector3 br = Vector3.zero;

    string xDimension = "";
    string yDimension = "";
    string zDimension = "";

    Vector3 poitionInWorld = Vector3.one;
    System.Tuple<Vector3, Vector3> tuplePCPWorld;
    System.Tuple<Vector3, Vector3> tuplePCPData;

    Transform parentTransform;
    public LineRenderer leaderInformation;

    Visualization visualizationReference = null;

    public Visualization VisualizationReference
    {
        get { return visualizationReference; }
        set { visualizationReference = value; }
    }

    bool isParallelView = false;

    public bool IsParallelView
    {
        get
        {
            return isParallelView;
        }

        set
        {
            isParallelView = value;
        }
    }

    public void setCorners(Vector3 _tl, Vector3 _tr, Vector3 _bl, Vector3 _br)
    {
        tl = _tl;
        tr = _tr;
        bl = _bl;
        br = _br;
    }

    public void setDataPCP(System.Tuple<Vector3, Vector3> _tuplePCPData)
    {
        tuplePCPData = _tuplePCPData;
    }

    public void setTuplePCPWorld(System.Tuple<Vector3, Vector3> tupleV3PCP)
    {
        tuplePCPWorld = tupleV3PCP;
    }

    public void setPositionInWorldScatterplot(Vector3 p)
    {
        poitionInWorld = p;
    }

    public void setCenter(Vector3 center)
    {
        Center = center;

    }

    public void setPointerPosition(Vector3 _pointerPosition)
    {
        pointerPosition = _pointerPosition;
    }

    public void setLocalPointerPosition(Vector3 _localPointerPosition)
    {
        localPointerPosition = _localPointerPosition;
    }

    private void Awake()
    {
        tempTransformObject = new GameObject("Brush Transform");
        tempTransformObject.transform.parent = transform;
        tempTransformObject.transform.localPosition = Vector3.zero;
        tempTransformObject.transform.localScale = new Vector3(0.2660912f, 0.2660912f, 0.2660912f) / 2;
    }

    void Start()
    {
        labelDetails = GameObject.CreatePrimitive(PrimitiveType.Quad);
        labelDetails.transform.localScale = new Vector3(0.04f, 0.01f, 1f);
        labelDetails.transform.parent = transform;

        Material m = new Material(Shader.Find("Standard"));
        m.SetFloat("_Mode", 2);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        m.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        m.name = "MyCustomMaterial";

        labelDetails.GetComponent<MeshRenderer>().material = m;

        textMesh = new GameObject();
        textMesh.transform.parent = transform;
        textMesh.transform.localScale = Vector3.one / 100f;
        textMesh.AddComponent<TextMesh>();
        textMesh.GetComponent<TextMesh>().color = Color.black;
        textMesh.GetComponent<TextMesh>().fontSize = 25;

        string[] dimensionVisualisation = transform.name.Split('-');

        xDimension = dimensionVisualisation[0];
        yDimension = dimensionVisualisation[1];

        leaderInformation = gameObject.AddComponent<LineRenderer>();

        leaderInformation.material = new Material(Shader.Find("Particles/Additive"));
        //leaderInformation.widthMultiplier = 0.0015f;
        leaderInformation.positionCount = 2;
        leaderInformation.useWorldSpace = true;
        leaderInformation.widthCurve = AnimationCurve.Linear(0, 0.0015f, 1, 0.0015f);

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );

        //leaderInformation.colorGradient = gradient;
        //labelDetails[0].transform.position = 2f * (Vector3.one);
    }
    
    float precisionSearch = 10E-6f;

    // takes a dimension name and an index of a datapoint, and returns a string value for that dimension 
    string StringValFromDataObj(DataBinding.DataObject dataObj, string dimensionName, int index)
    {
        float xval = dataObj.getOriginalDimension(dimensionName)[index];
        string xvalstr = xval.ToString();
        if (dataObj.TypeDimensionDictionary1[dataObj.dimensionToIndex(dimensionName)] == "string")
        {
            xvalstr = dataObj.TextualDimensions[xval];
        };
        return xvalstr;
    }

    public void OnDetailOnDemand2D()
    {
        textMesh.SetActive(true);
        labelDetails.SetActive(true);

        Vector2 rangeX = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(xDimension)];
        Vector2 rangeY = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(yDimension)];
        string values = "";

        if (pointerPosition != null && labelDetails != null && textMesh != null)
        {
            if (!isParallelView)
            {
                textMesh.transform.position = (pointerPosition);


                float x = localPointerPosition.x / 0.2660912f;
                float y = localPointerPosition.y / 0.2660912f;
                float z = localPointerPosition.z / 0.2660912f;

                //find the closest point in the list
                Vector2 pointerPosition2D = new Vector2(x + 0.5f, y + 0.5f);
                List<float> distances = new List<float>();

                for (int i = 0; i < SceneManager.Instance.dataObject.getDimension(0).Length; i++)
                {
                    distances.Add(Vector2.Distance(pointerPosition2D, new Vector2(SceneManager.Instance.dataObject.getDimension(xDimension)[i], SceneManager.Instance.dataObject.getDimension(yDimension)[i])));
                }
                int index = distances.FindIndex(d => d < distances.Min() + precisionSearch && d > distances.Min() - precisionSearch);

                var dataObj = SceneManager.Instance.dataObject;

                string xvalstr = StringValFromDataObj(dataObj, xDimension, index);
                string yvalstr = StringValFromDataObj(dataObj, yDimension, index);

                values = string.Format(@"{0}:{1} {2} {3}:{4}",
                    xDimension,
                    xvalstr,
                    Environment.NewLine,
                    yDimension,
                    yvalstr);

                leaderInformation.SetPosition(0, pointerPosition);
                    leaderInformation.SetPosition(1,
                      (transform.TransformPoint((SceneManager.Instance.dataObject.getDimension(xDimension)[index] - 0.5f) * 0.2660912f,
                       (SceneManager.Instance.dataObject.getDimension(yDimension)[index] - 0.5f) * 0.2660912f, 0f)));
                    leaderInformation.widthCurve = AnimationCurve.Linear(0, 0.0015f, 1, 0.0015f);

            }
            else
            {
                textMesh.transform.position = (pointerPosition);
                //get the axis from the PCP
                values = "PCP";
                // details on demand for PCP
                if (tuplePCPData.Item1.x < 0f)
                {
                    values = "PCP";// UtilMath.normaliseValue(tuplePCPWorld.Item1.x, -0.5f, 0.5f, rangeX.x, rangeX.y).ToString();
                }
            }

            textMesh.GetComponentInChildren<TextMesh>().text = values;
            textMesh.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);

            labelDetails.transform.Translate(labelDetails.transform.localScale.x / 2f,
                                                  -labelDetails.transform.localScale.y / 2f, 0.005f);
        }
    }
    
    GameObject tempTransformObject = null;

    public void OnDetailOnDemand3D()
    {
        textMesh.SetActive(true);
        string[] dimensionVisualisation = transform.name.Split('-');

        xDimension = dimensionVisualisation[0];
        yDimension = dimensionVisualisation[1];
        zDimension = dimensionVisualisation[2];
        zDimension = zDimension.Split(' ')[0];
        
        labelDetails.SetActive(true);
        
        Vector2 rangeX = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(xDimension)];
        Vector2 rangeY = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(yDimension)];
        Vector2 rangeZ = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(zDimension)];

        if (pointerPosition != null && labelDetails != null && textMesh != null)
        {
            if (!isParallelView)
            {
                textMesh.transform.position = (pointerPosition);

                string values = "";

                // create a transform for the visualisation space
                var vup = visualizationReference.fbl - visualizationReference.ftl;
                var right = visualizationReference.fbr - visualizationReference.fbl;

                right.Normalize();
                vup.Normalize();
                vup = -vup;

                var cp = Vector3.Cross(right, vup);

                var forward = visualizationReference.fbl - visualizationReference.bbl;

                bool isFlipped = false;

                if (Vector3.Dot(cp, forward) > 0)
                {
                    isFlipped = true;
                    forward = forward.normalized;
                }
                else
                {
                    forward = -forward.normalized;
                }

                Transform vt = tempTransformObject.transform;
                vt.rotation = Quaternion.LookRotation(forward, vup);

                Vector3 positionInLocal3DSP = vt.InverseTransformPoint(pointerPosition);

                float x = (positionInLocal3DSP.x + 1) / 2;
                float y = (positionInLocal3DSP.y + 1) / 2;
                float z = (positionInLocal3DSP.z + 1) / 2;

                if (isFlipped)
                {
                    z = 1 - z;
                }

                //find the closest point in the list
                Vector3 pointerPosition3D = new Vector3(x, y, z);
                List<float> distances = new List<float>();

                float minDistance = float.MaxValue;
                int minIndex = -1;
                for (int i = 0; i < SceneManager.Instance.dataObject.getDimension(0).Length; i++)
                {
                    var m = Vector3.SqrMagnitude(pointerPosition3D -
                        new Vector3(
                        SceneManager.Instance.dataObject.getDimension(xDimension)[i],
                        SceneManager.Instance.dataObject.getDimension(yDimension)[i],
                        SceneManager.Instance.dataObject.getDimension(zDimension)[i]));

                    if (m < minDistance)
                    {
                        minDistance = m;
                        minIndex = i;
                    }
                }

                int index = minIndex;

                var dataObj = SceneManager.Instance.dataObject;

                string xvalstr = StringValFromDataObj(dataObj, xDimension, index);
                string yvalstr = StringValFromDataObj(dataObj, yDimension, index);
                string zvalstr = StringValFromDataObj(dataObj, zDimension, index);

                values = string.Format(@"{0}:{1} {2} {3}:{4} {5} {6}:{7}",
                    xDimension, xvalstr,
                    Environment.NewLine,
                    yDimension, yvalstr,
                    Environment.NewLine,
                    zDimension, zvalstr);

                float xd = dataObj.getDimension(xDimension)[index];
                float yd = dataObj.getDimension(yDimension)[index];
                float zd = dataObj.getDimension(zDimension)[index];

                if (isFlipped)
                {
                    zd = 1 - zd;
                }

                leaderInformation.SetPosition(0, pointerPosition);
                leaderInformation.SetPosition(1,
               (vt.TransformPoint(
                (xd - 0.5f) / vt.localScale.x * 0.2660914f,
                (yd - 0.5f) / vt.localScale.y * 0.2660914f,
                (zd - 0.5f) / vt.localScale.z * 0.2660914f
                )));
                leaderInformation.widthCurve = AnimationCurve.Linear(0, 0.0015f, 1, 0.0015f);

                textMesh.GetComponentInChildren<TextMesh>().text = values;
                textMesh.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Vector3.up);// Camera.main.transform.rotation * Vector3.up);

            }
            else
            {
                textMesh.transform.position =
                        labelDetails.transform.position =
                        tuplePCPWorld.Item1 - (tuplePCPWorld.Item1 - Center);

                // details on demand for PCP
                if (tuplePCPData.Item1.x < 0f)
                {
                    float valueLeft = UtilMath.normaliseValue(tuplePCPWorld.Item1.x, -0.5f, 0.5f, rangeX.x, rangeX.y);
                }
            }

            labelDetails.transform.Translate(labelDetails.transform.localScale.x / 2f,
                                                  -labelDetails.transform.localScale.y / 2f, 0.005f);
        }
    }

    public object getValueFromDimension(float value, int dimension)
    {
        if (SceneManager.Instance.dataObject.TypeDimensionDictionary1[dimension] == "string")
        {
            Vector2 range = SceneManager.Instance.dataObject.DimensionsRange[dimension];
            float lerpedValue = Mathf.Lerp(range.x, range.y, value);
            float closest = UtilMath.ClosestTo(SceneManager.Instance.dataObject.TextualDimensions.Keys.ToList(), lerpedValue);
            return SceneManager.Instance.dataObject.TextualDimensions[closest].ToString();
        }
        else
            return SceneManager.Instance.dataObject.getOriginalValue(value, dimension);
    }

    internal void OnDetailOnDemandEnd()
    {
        textMesh.SetActive(false);
        labelDetails.SetActive(false);

        leaderInformation.SetPosition(0, Vector3.zero);
        leaderInformation.SetPosition(1, Vector3.zero);
    }

    Vector3 transformPointToVisualisation(Vector3 point)
    {
        return new Vector3(UtilMath.normaliseValue(point.x, -0.5f, 0.5f, br.x, bl.x),
                           UtilMath.normaliseValue(point.y, -0.5f, 0.5f, bl.y, tl.y),
                           UtilMath.normaliseValue(point.z, -0.5f, 0.5f, bl.z, br.z));
    }

    internal void setTransformParent(Transform transform)
    {
        parentTransform = transform;
    }


}
