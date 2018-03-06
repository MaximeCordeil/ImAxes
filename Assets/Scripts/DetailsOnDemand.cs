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
    Tuple<Vector3, Vector3> tuplePCPWorld;
    Tuple<Vector3, Vector3> tuplePCPData;

    Transform parentTransform;
    //LineRenderer leaderInformation;

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

    public void setDataPCP(Tuple<Vector3, Vector3> _tuplePCPData)
    {
        tuplePCPData = _tuplePCPData;
    }

    public void setTuplePCPWorld(Tuple<Vector3, Vector3> tupleV3PCP)
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

        //leaderInformation = gameObject.AddComponent<LineRenderer>();

        //leaderInformation.material = new Material(Shader.Find("Particles/Additive"));
        //leaderInformation.widthMultiplier = 0.0015f;
        //leaderInformation.positionCount = 2;
        //leaderInformation.useWorldSpace = true;

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

    void Update()
    {
        
    }

    public void OnDetailOnDemand2D()
    {
        //Vector3 pointInVisu = transform.TransformPoint(pointerPosition);
            labelDetails.SetActive(true);
          //  GetComponent<LineRenderer>().enabled = true;
            
            Vector2 rangeX = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(xDimension)];
            Vector2 rangeY = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(yDimension)];

            if (pointerPosition != null && labelDetails != null && textMesh != null)
            {
                if (!isParallelView)
                {
                    textMesh.transform.position = (pointerPosition);
                            //labelDetails.transform.position =
                            //poitionInWorld - (poitionInWorld - Center);

                    string values = "";

                    //SceneManager.Instance.dataObject.DimensionsRange()
                    float x = localPointerPosition.x / 0.2660912f;
                    float y = localPointerPosition.y / 0.2660912f;
                    float z = localPointerPosition.z / 0.2660912f;

                    values = string.Format(@"{0}:{1} {2} {3}:{4}",
                        xDimension,
                        getValueFromDimension(x+0.5f, SceneManager.Instance.dataObject.dimensionToIndex(xDimension)),// UtilMath.normaliseValue(pointerPosition.x, -0.5f, 0.5f, rangeX.x, rangeX.y),
                        Environment.NewLine,
                        yDimension,
                        getValueFromDimension(y + 0.5f, SceneManager.Instance.dataObject.dimensionToIndex(yDimension)) 
                        );

                    textMesh.GetComponentInChildren<TextMesh>().text = values;
                    textMesh.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
           Camera.main.transform.rotation * Vector3.up);
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
                    else
                    {
                    }

                }

                labelDetails.transform.Translate(labelDetails.transform.localScale.x / 2f,
                                                      -labelDetails.transform.localScale.y / 2f, 0.005f);
            }        
    }

    public void OnDetailOnDemand3D()
    {
        string[] dimensionVisualisation = transform.name.Split('-');

        xDimension = dimensionVisualisation[0];
        yDimension = dimensionVisualisation[1];
        zDimension = dimensionVisualisation[2];
        zDimension = zDimension.Split(' ')[0];
        //Vector3 pointInVisu = transform.TransformPoint(pointerPosition);
        labelDetails.SetActive(true);
        //  GetComponent<LineRenderer>().enabled = true;

        Vector2 rangeX = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(xDimension)];
        Vector2 rangeY = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(yDimension)];
        Vector2 rangeZ = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(zDimension)];
        
        if (pointerPosition != null && labelDetails != null && textMesh != null)
        {
            if (!isParallelView)
            {
                textMesh.transform.position = (pointerPosition);
                //labelDetails.transform.position =
                //poitionInWorld - (poitionInWorld - Center);

                string values = "";

                //SceneManager.Instance.dataObject.DimensionsRange()
                float x = localPointerPosition.x / 0.2660912f;
                float y = localPointerPosition.y / 0.2660912f;
                float z = localPointerPosition.z / 0.2660912f;

                values = string.Format(@"{0}:{1} {2} {3}:{4} {5} {6}:{7}",
                    xDimension,
                    getValueFromDimension(x + 0.5f, SceneManager.Instance.dataObject.dimensionToIndex(xDimension)),// UtilMath.normaliseValue(pointerPosition.x, -0.5f, 0.5f, rangeX.x, rangeX.y),
                    Environment.NewLine,
                    yDimension,
                    getValueFromDimension(y + 0.5f, SceneManager.Instance.dataObject.dimensionToIndex(yDimension)),
                    Environment.NewLine,
                    zDimension,
                    getValueFromDimension(z + 0.5f, SceneManager.Instance.dataObject.dimensionToIndex(zDimension))
                    );

                textMesh.GetComponentInChildren<TextMesh>().text = values;
                //textMesh.gameObject.transform.LookAt(Camera.main.transform);

                textMesh.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);

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
                else
                {
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
     //   GetComponent<LineRenderer>().enabled = false;
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
