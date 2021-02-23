using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Staxes;
using System.Linq;

public class LinkedVisualisations : MonoBehaviour
{
    Visualization v1 = null;

    public Visualization V1
    {
        get { return v1; }
        set { v1 = value; }
    }
    Visualization v2 = null;

    public Visualization V2
    {
        get { return v2; }
        set { v2 = value; }
    }

    Mesh mymesh;

    //Color[] colorBuffer;

    Vector3?[] c1;
    Vector3?[] c2;

    List<Vector3> pointBuffer = new List<Vector3>();
    List<Vector3> normalsBuffer = new List<Vector3>();
    List<int> indicesBuffer = new List<int>();
    List<Color> colors = new List<Color>();

    Renderer linkRenderer;

    // Linked filtering variables
    public List<Visualization> ConnectedVisualisations;
    public List<LinkedVisualisations> ConnectedLinkedVisualisations;
    public float[] SharedFilteredPoints;
    public bool SharedFilteredPointsChanged;
    private bool wasV1Truncated = false;
    private bool wasV2Truncated = false;

    // create a mesk for the linked view
    void CreateMesh()
    {
        mymesh.Clear();

        if (v1.viewType == Visualization.ViewType.Histogram && v2.viewType != Visualization.ViewType.Histogram)
        {
            //1 get list of axes in v2 and test parallelism
            List<Axis> listOfAxes = v2.axes;
            List<float> distances = new List<float>();

            // the first value is the distance between the histogram and the centre of the visualsiation
            distances.Add(Vector3.Distance(v1.axes[0].transform.position, v2.transform.position));

            //the rest are the distances between the histogram and each axes
            foreach (var item in listOfAxes)
            {
                distances.Add(Vector3.Distance(item.transform.position, v1.axes[0].transform.position));
            }

            // the closest item
            int indexClosestVisu = distances.IndexOf(distances.ToArray().Min());

            // get the corresponding histogram
            if (indexClosestVisu > 0)
            {
                c2 = v2.get1DAxisCoordinates(indexClosestVisu - 1);
            }
            else c2 = v2.GetPoints();

            c1 = v1.GetPoints();
        }
        else if (v2.viewType == Visualization.ViewType.Histogram && v1.viewType != Visualization.ViewType.Histogram)
        {
            //1 get list of axes in v2 and test parallelism
            List<Axis> listOfAxes = v1.axes;
            List<float> distances = new List<float>();

            // the first value is the distance between the histogram and the centre of the visualsiation
            distances.Add(Vector3.Distance(v1.transform.position, v2.axes[0].transform.position));

            //the rest are the distances between the histogram and each axes
            foreach (var item in listOfAxes)
            {
                distances.Add(Vector3.Distance(item.transform.position, v2.axes[0].transform.position));
            }

            // the closest item
            int indexClosestVisu = distances.IndexOf(distances.ToArray().Min());

            // get the corresponding histogram
            if (indexClosestVisu > 0)
            {
                c1 = v1.get1DAxisCoordinates(indexClosestVisu - 1);
            }
            else c1 = v1.GetPoints();

            c2 = v2.GetPoints();
        }
        else
        {
            c1 = v1.GetPoints();
            c2 = v2.GetPoints();
        }

        pointBuffer.Clear();
        indicesBuffer.Clear();
        colors.Clear();
        normalsBuffer.Clear();

        //1 create mesh buffer and indice buffer
        if (c1 != null && c2 != null)
        {
            for (int i = 0; i < c1.Length; i++)
            {
                if (c1[i] != null && c2[i] != null)
                {
                    pointBuffer.Add(c1[i].Value);
                    pointBuffer.Add(c2[i].Value);

                    Color c = VisualisationAttributes.Instance.colors[i];
                    c.a = VisualisationAttributes.Instance.LinkTransparency;
                    colors.Add(c);
                    colors.Add(c);

                    normalsBuffer.Add(new Vector3(0, 0, 0));
                    normalsBuffer.Add(new Vector3(0, 0, 1));
                }
            }

            for (int i = 0; i < pointBuffer.Count; i += 2)
            {
                indicesBuffer.Add(i);
                indicesBuffer.Add(i + 1);
            }

            mymesh.vertices = pointBuffer.ToArray();
            mymesh.normals = normalsBuffer.ToArray();
            mymesh.SetIndices(indicesBuffer.ToArray(), MeshTopology.Lines, 0);
            mymesh.colors = colors.ToArray();
        }
    }

    // Use this for initialization
    void Start()
    {
        var placeHolder = new GameObject();
        mymesh = new Mesh();

        placeHolder.transform.parent = transform;

        MeshFilter filter = placeHolder.AddComponent<MeshFilter>();
        linkRenderer = placeHolder.AddComponent<MeshRenderer>();

        filter.mesh = mymesh;
        linkRenderer.material = VisualisationFactory.Instance.linkedViewsMaterial;

        placeHolder.tag = "LinkedVisualisation";

        CreateMesh();

        EventManager.StartListening(ApplicationConfiguration.OnColoredAttributeChanged, OnColoredAttributeChanged);
        EventManager.StartListening(ApplicationConfiguration.OnLinkedTransparencyChanged, OnColoredAttributeChanged);
    }

    void OnDestroy()
    {
        EventManager.StopListening(ApplicationConfiguration.OnColoredAttributeChanged, OnColoredAttributeChanged);
        EventManager.StopListening(ApplicationConfiguration.OnLinkedTransparencyChanged, OnColoredAttributeChanged);
    }

    private void Update()
    {
        ConnectedVisualisations = null;
        ConnectedLinkedVisualisations = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (v1 != null && v2 != null)
        {
            // Update the connected visualisations list for this LinkedVisualisation only if it has not yet already
            // been set by another Visualisation ir LinkedVisualisation. This essentially means that the first Visualisation
            // or LinkedVisualisation in the chain which executes is the one which sets it for all other ones in the chain
            if (ConnectedVisualisations == null)
            {
                var result = ImAxesRecognizer.Instance.GetChainedVisualisationsAndLinkedVisualisations(this);
                ConnectedVisualisations = result.Item1;
                ConnectedLinkedVisualisations = result.Item2;

                // Combine the FilteredPoints of all ConnectedVisualisations if at least one of them has changed
                bool updateFiltered = false;
                foreach (var vis in ConnectedVisualisations)
                {
                    if (vis.FilteredPointsChanged)
                    {
                        updateFiltered = true;
                        break;
                    }
                }

                // Create a shared filtered array of all ConnectedVisualisations and set it to all Visualisations and LinkedVisualisations
                if (updateFiltered)
                {
                    int dataCount = SceneManager.Instance.dataObject.DataPoints;
                    SharedFilteredPoints = new float[dataCount];
                    foreach (var vis in ConnectedVisualisations)
                    {
                        for (int i = 0; i < dataCount; i++)
                        {
                            if (vis.FilteredPoints[i] == 1)
                                SharedFilteredPoints[i] = 1;
                        }

                        vis.FilteredPointsChanged = false;
                    }

                    foreach (var vis in ConnectedVisualisations)
                    {
                        vis.SharedFilteredPoints = SharedFilteredPoints;
                        vis.SharedFilteredPointsChanged = true;
                    }
                    foreach (var linkedVis in ConnectedLinkedVisualisations)
                    {
                        linkedVis.SharedFilteredPoints = SharedFilteredPoints;
                        linkedVis.SharedFilteredPointsChanged = true;
                    }

                    SharedFilteredPointsChanged = true;
                }
            }

            // Set normals array
            if (SharedFilteredPointsChanged)
            {
                List<Vector3> normals = new List<Vector3>();
                mymesh.GetNormals(normals);

                // Check if we should truncate the lines
                bool isV1Truncated = false;
                bool isV2Truncated = false;
                float truncateV1 = 0;
                float truncateV2 = 0;
                switch (v1.viewType)
                {
                    // Never truncate histograms
                    case Visualization.ViewType.Histogram:
                        break;
                    // Only truncate 2D scatterplots if it is horizontal
                    case Visualization.ViewType.Scatterplot2D:
                        float v1Angle = Vector3.Angle(V1.transform.forward, Vector3.up);
                        isV1Truncated = !(45 < v1Angle && v1Angle < 135);
                        truncateV1 = isV1Truncated ? 1 : 0;
                        break;
                    // Always truncate 3D scatterplots
                    case Visualization.ViewType.Scatterplot3D:
                        isV1Truncated = true;
                        truncateV1 = 1;
                        break;
                }
                switch (v2.viewType)
                {
                    // Never truncate histograms
                    case Visualization.ViewType.Histogram:
                        break;
                    // Only truncate 2D scatterplots if it is horizontal
                    case Visualization.ViewType.Scatterplot2D:
                        float v2Angle = Vector3.Angle(V2.transform.forward, Vector3.up);
                        isV2Truncated = !(45 < v2Angle && v2Angle < 135);
                        truncateV2 = isV2Truncated ? 1 : 0;
                        break;
                    // Always truncate 3D scatterplots
                    case Visualization.ViewType.Scatterplot3D:
                        isV2Truncated = true;
                        truncateV2 = 1;
                        break;
                }

                // Linked visualisations are double length
                int normalIndex = 0;
                for (int i = 0; i < SceneManager.Instance.dataObject.DataPoints; i++)
                {
                    Vector3 norm1 = normals[normalIndex];
                    Vector3 norm2 = normals[normalIndex + 1];

                    norm1.x = SharedFilteredPoints[i];
                    norm2.x = SharedFilteredPoints[i];

                    norm1.y = truncateV1;
                    norm2.y = truncateV2;

                    normals[normalIndex] = norm1;
                    normals[normalIndex + 1] = norm2;

                    normalIndex += 2;
                }
                mymesh.SetNormals(normals);
                SharedFilteredPointsChanged = false;

                wasV1Truncated = isV1Truncated;
                wasV2Truncated = isV2Truncated;
            }
            else
            {
                // Check if we should truncate the lines
                bool isV1Truncated = false;
                bool isV2Truncated = false;
                float truncateV1 = 0;
                float truncateV2 = 0;
                switch (v1.viewType)
                {
                    // Never truncate histograms
                    case Visualization.ViewType.Histogram:
                        break;
                    // Only truncate 2D scatterplots if it is horizontal
                    case Visualization.ViewType.Scatterplot2D:
                        float v1Angle = Vector3.Angle(V1.transform.forward, Vector3.up);
                        isV1Truncated = !(45 < v1Angle && v1Angle < 135);
                        truncateV1 = isV1Truncated ? 1 : 0;
                        break;
                    // Always truncate 3D scatterplots
                    case Visualization.ViewType.Scatterplot3D:
                        isV1Truncated = true;
                        truncateV1 = 1;
                        break;
                }
                switch (v2.viewType)
                {
                    // Never truncate histograms
                    case Visualization.ViewType.Histogram:
                        break;
                    // Only truncate 2D scatterplots if it is horizontal
                    case Visualization.ViewType.Scatterplot2D:
                        float v2Angle = Vector3.Angle(V2.transform.forward, Vector3.up);
                        isV2Truncated = !(45 < v2Angle && v2Angle < 135);
                        truncateV2 = isV2Truncated ? 1 : 0;
                        break;
                    // Always truncate 3D scatterplots
                    case Visualization.ViewType.Scatterplot3D:
                        isV2Truncated = true;
                        truncateV2 = 1;
                        break;
                }

                // If either visualisation has swapped between is truncating and is not truncating, update the normals
                if (isV1Truncated != wasV1Truncated || isV2Truncated != wasV2Truncated)
                {
                    List<Vector3> normals = new List<Vector3>();
                    mymesh.GetNormals(normals);
                    int normalIndex = 0;
                    for (int i = 0; i < SceneManager.Instance.dataObject.DataPoints; i++)
                    {
                        Vector3 norm1 = normals[normalIndex];
                        Vector3 norm2 = normals[normalIndex + 1];
                        norm1.y = truncateV1;
                        norm2.y = truncateV2;
                        normals[normalIndex] = norm1;
                        normals[normalIndex + 1] = norm2;
                        normalIndex += 2;
                    }
                    mymesh.SetNormals(normals);

                    wasV1Truncated = isV1Truncated;
                    wasV2Truncated = isV2Truncated;
                }
            }

            // position the linked visualization between axis
            Vector3 p = new Vector3();
            try
            {
                p = (v1.axes[0].transform.position + v2.axes[0].transform.position) / 2;
                transform.position = p;

                linkRenderer.material.SetVector("_ftl1", transform.InverseTransformPoint(v1.ftl));
                linkRenderer.material.SetVector("_fbl1", transform.InverseTransformPoint(v1.fbl));
                linkRenderer.material.SetVector("_ftr1", transform.InverseTransformPoint(v1.ftr));
                linkRenderer.material.SetVector("_fbr1", transform.InverseTransformPoint(v1.fbr));

                linkRenderer.material.SetVector("_btl1", transform.InverseTransformPoint(v1.btl));
                linkRenderer.material.SetVector("_bbl1", transform.InverseTransformPoint(v1.bbl));
                linkRenderer.material.SetVector("_btr1", transform.InverseTransformPoint(v1.btr));
                linkRenderer.material.SetVector("_bbr1", transform.InverseTransformPoint(v1.bbr));

                linkRenderer.material.SetVector("_ftr2", transform.InverseTransformPoint(v2.ftr));
                linkRenderer.material.SetVector("_fbr2", transform.InverseTransformPoint(v2.fbr));
                linkRenderer.material.SetVector("_ftl2", transform.InverseTransformPoint(v2.ftl));
                linkRenderer.material.SetVector("_fbl2", transform.InverseTransformPoint(v2.fbl));

                linkRenderer.material.SetVector("_btr2", transform.InverseTransformPoint(v2.btr));
                linkRenderer.material.SetVector("_bbr2", transform.InverseTransformPoint(v2.bbr));
                linkRenderer.material.SetVector("_btl2", transform.InverseTransformPoint(v2.btl));
                linkRenderer.material.SetVector("_bbl2", transform.InverseTransformPoint(v2.bbl));
                
                linkRenderer.material.SetFloat("_MinXFilter1", v1.minXFilter);
                linkRenderer.material.SetFloat("_MaxXFilter1", v1.maxXFilter);
                linkRenderer.material.SetFloat("_MinYFilter1", v1.minYFilter);
                linkRenderer.material.SetFloat("_MaxYFilter1", v1.maxYFilter);
                linkRenderer.material.SetFloat("_MinZFilter1", v1.minZFilter);
                linkRenderer.material.SetFloat("_MaxZFilter1", v1.maxZFilter);

                linkRenderer.material.SetFloat("_MinXFilter2", v2.minXFilter);
                linkRenderer.material.SetFloat("_MaxXFilter2", v2.maxXFilter);
                linkRenderer.material.SetFloat("_MinYFilter2", v2.minYFilter);
                linkRenderer.material.SetFloat("_MaxYFilter2", v2.maxYFilter);
                linkRenderer.material.SetFloat("_MinZFilter2", v2.minZFilter);
                linkRenderer.material.SetFloat("_MaxZFilter2", v2.maxZFilter);

                linkRenderer.material.SetFloat("_MinNormX1", v1.minXNormalizer);
                linkRenderer.material.SetFloat("_MaxNormX1", v1.maxXNormalizer);
                linkRenderer.material.SetFloat("_MinNormY1", v1.minYNormalizer);
                linkRenderer.material.SetFloat("_MaxNormY1", v1.maxYNormalizer);
                linkRenderer.material.SetFloat("_MinNormZ1", v1.minZNormalizer);
                linkRenderer.material.SetFloat("_MaxNormZ1", v1.maxZNormalizer);

                linkRenderer.material.SetFloat("_MinNormX2", v2.minXNormalizer);
                linkRenderer.material.SetFloat("_MaxNormX2", v2.maxXNormalizer);
                linkRenderer.material.SetFloat("_MinNormY2", v2.minYNormalizer);
                linkRenderer.material.SetFloat("_MaxNormY2", v2.maxYNormalizer);
                linkRenderer.material.SetFloat("_MinNormZ2", v2.minZNormalizer);
                linkRenderer.material.SetFloat("_MaxNormZ2", v2.maxZNormalizer);
            }
            catch
            {
                try
                {
                    Destroy(v1.gameObject);
                    Destroy(v2.gameObject);
                }
                catch { }
            }

        }
        else
        {
            try
            {
                Destroy(v1.gameObject);
                Destroy(v2.gameObject);
            } catch { }
        }

    }

    public int GetPriority()
    {
        return 50;
    }
    public bool Contains(Axis a)
    {
        return (V1 != null && V2 != null && (V1.axes.Contains(a) || V2.axes.Contains(a))) ;
    }

    void OnColoredAttributeChanged(float idx)
    {
        CreateMesh();
    }

}