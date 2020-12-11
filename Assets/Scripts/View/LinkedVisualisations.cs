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

    // Update is called once per frame
    void LateUpdate()
    {
        if (v1 != null && v2 != null)
        {
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