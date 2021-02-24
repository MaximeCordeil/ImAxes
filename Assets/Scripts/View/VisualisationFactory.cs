using UnityEngine;
using DataBinding;
using System.Collections.Generic;
using System.Linq;
using Staxes;

public class VisualisationFactory : MonoBehaviour
{
    public Material histogramMaterial;
    public Material pointCloudMaterial;
    public Material linesGraphMaterial;
    public Material linkedViewsMaterial;
    public Material pointCloudMaterialBrush;
    public Material connectedPointLineMaterial;

    DataObject dataObject;
    GameObject MenuDimensions;

    //Singleton pattern
    protected static VisualisationFactory _instance;
    public static VisualisationFactory Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<VisualisationFactory>()); }
    }

    // Use this for initialization
    void Start()
    {

    }
    /// <summary>
    /// sets the position of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="pos"></param>
    void SetViewPosition(ref GameObject view, Vector3 pos)
    {
        view.transform.position = pos;
    }

    /// <summary>
    /// sets the scale of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="scale"></param>
    void SetViewScale(ref GameObject view, Vector3 scale)
    {
        view.transform.localScale = scale;
    }

    /// <summary>
    /// sets the orientation of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="quat"></param>
    void SetViewRotation(ref GameObject view, Quaternion quat)
    {
        view.transform.rotation = quat;
    }

    /// <summary>
    /// Creates a textual menu with data dimensions 
    /// </summary>
    /// <param name="dobjs"></param>
    /// <returns></returns>
    GameObject createTextMenu(DisplayMenu dm, DataObject dobjs)
    {
        GameObject menu = new GameObject();
        dm.createTextMenu(menu, Color.black, Color.red);
        menu.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return menu;
    }

    /// <summary>
    /// Creates a line view of the distribution
    /// </summary>
    /// <param name="dobjs"></param>
    /// <param name="Dimension"></param>
    /// <param name="binSize"></param>
    /// <param name="smooth"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public Tuple<GameObject, Vector3[]> CreateLineHistogramView(DataObject dobjs, int Dimension, int binSize, bool smooth, float scale, Material mat, Transform holder)
    {
        GameObject Snax = new GameObject();
        List<Vector3> l = new List<Vector3>();

        //get the array of dimension
        float[] values = dobjs.GetCol(dobjs.DataArray, Dimension);
        float[] bins = new float[values.Length + 1];// binSize+1];
        int _binSize = values.Length;
        //bin the values
        for (int i = 0; i < values.Length; i++)
        {
            int indexBin = Mathf.RoundToInt(values[i] * _binSize);
            bins[indexBin] += 1f;// 0.025f;
        }

        float minBin = bins.Min();
        float maxBin = bins.Max();

        //create the data points height (~ histo)
        for (int i = 0; i < bins.Length; i++)
        {
            // normalize positions to range in -0.5...0.5
            float y = UtilMath.normaliseValue(i, 0, bins.Length - 1, 0.5f, -0.5f);
            float x = UtilMath.normaliseValue(bins[i], minBin, maxBin, -0.5f, 0.5f);
            Vector3 v = new Vector3(y, x, 0f);
            l.Add(v);
        }

        Vector3[] pointCurved;
        if (smooth) pointCurved = Curver.MakeSmoothCurve(l.ToArray(), _binSize);
        else pointCurved = l.ToArray();

        LineRenderer lineRenderer = Snax.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = mat;

        lineRenderer.SetWidth(0.0025f, 0.0025f);
        lineRenderer.positionCount = pointCurved.Length;
        lineRenderer.SetPositions(pointCurved);
        lineRenderer.useWorldSpace = false;
        return new Tuple<GameObject, Vector3[]>(Snax, pointCurved);
    }


    public Tuple<GameObject, Vector3[]> CreateBarHistogramView(DataObject dobjs, int Dimension, int binSize, bool smooth, float scale, Material mat, Transform holder, float minFilter, float maxFilter, float minNormalizer, float maxNormalizer)
    {
        GameObject Snax = new GameObject();

        //get the array of dimension
        DiscreteBinner binner = new DiscreteBinner();

        float[] values = dobjs.GetCol(dobjs.DataArray, Dimension);
        var filtered = values.Where(x => x >= minFilter + 0.5f && x <= maxFilter + 0.5f);
        if (filtered.Count() > 0)
            values = filtered.ToArray();
            
        float minVal = dobjs.DimensionsRange[Dimension].x;
        float maxVal = dobjs.DimensionsRange[Dimension].y;

        binner.MakeIntervals(values, dobjs.Metadata[Dimension].binCount);
        foreach (float value in values)
        {
            binner.Bin(value);
        }

        float[] bins = binner.bins;
        int _binSize = values.Length;

        float minBin = Mathf.Min(bins.Min(), 0);
        float maxBin = bins.Max();

        //create the data points height (~ histo)
        List<Vector3> l = new List<Vector3>();
        for (int i = 0; i < bins.Length; i++)
        {
            // normalize positions to range in -0.5...0.5
            float y = UtilMath.normaliseValue(i, 0, bins.Length - 1, 0.5f, -0.5f);
            if (float.IsNaN(y))
                y = 0;
            float x = UtilMath.normaliseValue(bins[i], minBin, maxBin, minNormalizer, maxNormalizer); // -0.5f, 0.5f);
            Vector3 v = new Vector3(x, y, 0f);
            l.Add(v);
        }

        // generate the mesh
        MeshFilter filter = Snax.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filter.mesh = mesh;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        float step = 0.5f / bins.Length;
        for (int i = 0; i < bins.Length; ++i)
        {
            Vector3 v = l[i];
            verts.Add(new Vector3(v.y - step, -0.5f, 0.0001f));
            verts.Add(new Vector3(v.y - step, v.x, 0.0001f));
            verts.Add(new Vector3(v.y + step, v.x, 0.0001f));
            verts.Add(new Vector3(v.y + step, -0.5f, 0.0001f));

            verts.Add(new Vector3(v.y - step, -0.5f, -0.0001f));
            verts.Add(new Vector3(v.y - step, v.x, -0.0001f));
            verts.Add(new Vector3(v.y + step, v.x, -0.0001f));
            verts.Add(new Vector3(v.y + step, -0.5f, -0.0001f));

            // front
            tris.Add(i * 8 + 0 + 0);
            tris.Add(i * 8 + 0 + 1);
            tris.Add(i * 8 + 0 + 3);
            tris.Add(i * 8 + 0 + 3);
            tris.Add(i * 8 + 0 + 1);
            tris.Add(i * 8 + 0 + 2);

            // back
            tris.Add(i * 8 + 4 + 3);
            tris.Add(i * 8 + 4 + 1);
            tris.Add(i * 8 + 4 + 0);
            tris.Add(i * 8 + 4 + 2);
            tris.Add(i * 8 + 4 + 1);
            tris.Add(i * 8 + 4 + 3);

        }
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = Snax.AddComponent<MeshRenderer>();
        meshRenderer.material = histogramMaterial;
        return new Tuple<GameObject, Vector3[]>(Snax, l.ToArray());
    }

    public static void UpdatetHistogramMesh(DataObject dobjs, int Dimension, int binSize, bool smooth, float scale, Material mat, Transform holder, float minFilter, float maxFilter, float minNormalizer, float maxNormalizer, ref Mesh mesh)
    {
        mesh.Clear();
        //get the array of dimension
        DiscreteBinner binner = new DiscreteBinner();

        float[] values = dobjs.GetCol(dobjs.DataArray, Dimension);
        //values = values.Where(x => x >= minNormalizer + 0.5f && x <= maxNormalizer + 0.5f).ToArray();

        float minVal = dobjs.DimensionsRange[Dimension].x;
        float maxVal = dobjs.DimensionsRange[Dimension].y;

        binner.MakeIntervals(values, dobjs.Metadata[Dimension].binCount);
        foreach (float value in values)
        {
            binner.Bin(value);
        }

        float[] bins = binner.bins;
        int _binSize = values.Length;

        float minBin = bins.Min();
        float maxBin = bins.Max();

        float minNormInternal = 0f;
        float maxNormInternal = 0f;

        //create the data points height (~ histo)
        List<Vector3> l = new List<Vector3>();
        for (int i = 0; i < bins.Length; i++)
        {
            float y1 = UtilMath.normaliseValue(i, 0, bins.Length - 1, -0.5f, 0.5f);
            if ((y1 >= (minFilter) && y1 <= (maxFilter)))
            {
                // normalize positions to range in -0.5...0.5
                float y = UtilMath.normaliseValue(i, 0, bins.Length - 1, 0.5f, -0.5f);

                float ynorm = UtilMath.normaliseValue(-y, minNormalizer, maxNormalizer, 0.5f, -0.5f);
                if (ynorm >= -0.5f &&
                         ynorm <= 0.5f)
                {
                    float x = UtilMath.normaliseValue(bins[i], minBin, maxBin, -0.5f, 0.5f);

                    Vector3 v = new Vector3(x, ynorm, 0f);
                    l.Add(v);
                }
            }
        }

        // generate the mesh
        //Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        float step = 0.5f / bins.Length;

        for (int i = 0; i < l.Count; ++i)
        {
            Vector3 v = l[i];
            verts.Add(new Vector3(v.y - step, -0.5f, 0.0001f));
            verts.Add(new Vector3(v.y - step, v.x, 0.0001f));
            verts.Add(new Vector3(v.y + step, v.x, 0.0001f));
            verts.Add(new Vector3(v.y + step, -0.5f, 0.0001f));

            verts.Add(new Vector3(v.y - step, -0.5f, -0.0001f));
            verts.Add(new Vector3(v.y - step, v.x, -0.0001f));
            verts.Add(new Vector3(v.y + step, v.x, -0.0001f));
            verts.Add(new Vector3(v.y + step, -0.5f, -0.0001f));

            // front
            tris.Add(i * 8 + 0 + 0);
            tris.Add(i * 8 + 0 + 1);
            tris.Add(i * 8 + 0 + 3);
            tris.Add(i * 8 + 0 + 3);
            tris.Add(i * 8 + 0 + 1);
            tris.Add(i * 8 + 0 + 2);

            // back
            tris.Add(i * 8 + 4 + 3);
            tris.Add(i * 8 + 4 + 1);
            tris.Add(i * 8 + 4 + 0);
            tris.Add(i * 8 + 4 + 2);
            tris.Add(i * 8 + 4 + 1);
            tris.Add(i * 8 + 4 + 3);

        }
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }

    public GameObject CreateDiscHistogramView(DataObject dobjs, int Dimension, int binSize, bool smooth, float scale)
    {
        GameObject Snax = new GameObject();
        List<Vector3> l = new List<Vector3>();

        //get the array of dimension
        float[] values = dobjs.GetCol(dobjs.DataArray, Dimension);
        float[] bins = new float[binSize + 1];

        //bin the values
        for (int i = 0; i < values.Length; i++)
        {
            int indexBin = Mathf.RoundToInt(values[i] * binSize);
            bins[indexBin] += 0.05f;
        }

        float minBin = bins.Min();
        float maxBin = bins.Max();

        //create the data points height (~ histo)
        for (int i = 0; i < bins.Length; i++)
        {
            // normalize positions to range in -0.5...0.5
            float y = UtilMath.normaliseValue(i, 0, bins.Length - 1, 0.5f, -0.5f);
            float x = UtilMath.normaliseValue(bins[i], minBin, maxBin, -0.5f, 0.5f);
            Vector3 v = new Vector3(y, x);
            l.Add(v);
        }

        Vector3[] pointCurved;
        if (smooth) pointCurved = Curver.MakeSmoothCurve(l.ToArray(), binSize);
        else pointCurved = l.ToArray();

        LineRenderer lineRenderer = Snax.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = Color.red;
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.0025f, 0.0025f);
        lineRenderer.SetVertexCount(pointCurved.Length);
        lineRenderer.SetPositions(pointCurved);
        lineRenderer.useWorldSpace = false;

        return Snax;
    }

    /// <summary>
    /// Creates a single view with dimensions x, y, z
    /// </summary>
    /// <param name="dobjs"></param>
    /// <param name="DimensionX"></param>
    /// <param name="DimensionY"></param>
    /// <param name="DimensionZ"></param>
    /// <param name="topology"></param>
    /// <param name="LinkIndex"> the linking field to create a graph; pass a negative value to ignore</param>
    /// <returns></returns>
    public Tuple<GameObject, View> CreateSingle2DView(DataObject dobjs, int DimensionX, int DimensionY, int DimensionZ, int LinkIndex, MeshTopology topology, Material m, bool parallel = false)
    {
        // @todo remove the parallel bool for a better implementation
        string viewName = "";

        if (DimensionX >= 0) viewName += dobjs.indexToDimension(DimensionX) + "-";
        if (DimensionY >= 0) viewName += dobjs.indexToDimension(DimensionY) + "-";
        if (DimensionZ >= 0) viewName += dobjs.indexToDimension(DimensionZ);

        MeshTopology mtp;

        if (LinkIndex > 0)
        {
            mtp = MeshTopology.Lines;
            //double the 

        }
        else mtp = MeshTopology.Points;

        View v = new View(mtp, viewName);
        GameObject view = new GameObject(viewName);
        //view.transform.parent = transform;

        if (!parallel)
        {
            v.initialiseDataView(dobjs.DataPoints, view);

            if (DimensionX >= 0)
            {
                float[] xpts = dobjs.getDimension(DimensionX);
                v.setDataDimension(xpts, View.VIEW_DIMENSION.X);
            }
            if (DimensionY >= 0)
            {
                v.setDataDimension(dobjs.getDimension(DimensionY), View.VIEW_DIMENSION.Y);
            }
            if (DimensionZ >= 0)
            {
                v.setDataDimension(dobjs.getDimension(DimensionZ), View.VIEW_DIMENSION.Z);
            }
        }
        else
        {
            if (DimensionX >= 0 && DimensionY >= 0)
            {
                v.initialiseDataView(dobjs.DataPoints * 2, view);

                List<float> data = new List<float>();
                List<float> range = new List<float>();

                float[] dimx = dobjs.getDimension(DimensionX);
                float[] dimy = dobjs.getDimension(DimensionY);

                for (int i = 0; i < dobjs.DataPoints; i++)
                {
                    data.Add(dimx[i]);
                    data.Add(dimy[i]);
                    range.Add(-1);
                    range.Add(1);
                }

                v.setDataDimension(data.ToArray(), View.VIEW_DIMENSION.Y);
                v.setDataDimension(range.ToArray(), View.VIEW_DIMENSION.X);
            }
        }
        if (LinkIndex < 0)
            v.updateView(null);
        else
            v.updateView(dobjs.getDimension(LinkIndex));

        view.AddComponent<MeshFilter>();
        view.AddComponent<MeshRenderer>();

        view.GetComponent<MeshFilter>().mesh = v.MyMesh;
        view.GetComponent<Renderer>().material = m;

        return new Tuple<GameObject, View>(view, v);
    }

    public Tuple<GameObject, View> CreateLinkedPCPto2DScatterplotsViews(DataObject dobjs, int DimensionX1, int DimensionY1, int DimensionX2, Material m)
    {
        string viewName = "";
        if (DimensionX1 >= 0) viewName += dobjs.indexToDimension(DimensionX1) + "-";
        if (DimensionY1 >= 0) viewName += dobjs.indexToDimension(DimensionY1) + "-";
        if (DimensionX2 >= 0) viewName += dobjs.indexToDimension(DimensionX2) + "-";

        View v = new View(MeshTopology.Lines, viewName);
        GameObject view = new GameObject(viewName);
        v.initialiseDataView(dobjs.DataPoints * 2, view);

        List<float> XData = new List<float>();
        List<float> YData = new List<float>();
        List<float> ZData = new List<float>();

        float[] dimx1 = dobjs.getDimension(DimensionX1);
        float[] dimy1 = dobjs.getDimension(DimensionY1);
        float[] dimx2 = dobjs.getDimension(DimensionX2);

        for (int i = 0; i < dobjs.DataPoints; i++)
        {
            XData.Add(dimx1[i]);
            YData.Add(dimy1[i]);
            ZData.Add(-0.5f);

            XData.Add(dimx1[i]);
            YData.Add(dimx2[i]);
            ZData.Add(0.5f);
        }

        v.setDataDimension(XData.ToArray(), View.VIEW_DIMENSION.X);
        v.setDataDimension(YData.ToArray(), View.VIEW_DIMENSION.Y);
        v.setDataDimension(ZData.ToArray(), View.VIEW_DIMENSION.Z);
        v.updateView(null);

        view.AddComponent<MeshFilter>();
        view.AddComponent<MeshRenderer>();

        view.GetComponent<MeshFilter>().mesh = v.MyMesh;
        view.GetComponent<Renderer>().material = m;

        return new Tuple<GameObject, View>(view, v);
    }


    public Tuple<GameObject, View> CreateLinked2DScatterplotsViews(DataObject dobjs, int DimensionX1, int DimensionY1, int DimensionX2, int DimensionY2, Material m)
    {
        float[] x1 = dobjs.getDimension(DimensionX1);
        float[] y1 = dobjs.getDimension(DimensionY1);
        float[] x2 = dobjs.getDimension(DimensionX2);
        float[] y2 = dobjs.getDimension(DimensionY2);

        string viewName = "";
        if (DimensionX1 >= 0) viewName += dobjs.indexToDimension(DimensionX1) + "-";
        if (DimensionY1 >= 0) viewName += dobjs.indexToDimension(DimensionY1) + "-";
        if (DimensionX2 >= 0) viewName += dobjs.indexToDimension(DimensionX2) + "-";
        if (DimensionY2 >= 0) viewName += dobjs.indexToDimension(DimensionY2) + "-";

        View v = new View(MeshTopology.Lines, viewName);
        GameObject view = new GameObject(viewName);
        v.initialiseDataView(dobjs.DataPoints * 2, view);

        List<float> XData = new List<float>();
        List<float> YData = new List<float>();
        List<float> ZData = new List<float>();

        float[] dimx1 = dobjs.getDimension(DimensionX1);
        float[] dimy1 = dobjs.getDimension(DimensionY1);
        float[] dimx2 = dobjs.getDimension(DimensionX2);
        float[] dimy2 = dobjs.getDimension(DimensionY2);

        for (int i = 0; i < dobjs.DataPoints; i++)
        {
            XData.Add(dimx1[i]);
            YData.Add(dimy1[i]);
            ZData.Add(-0.5f);

            XData.Add(dimx2[i]);//dimx2[i]);
            YData.Add(0f);
            ZData.Add(0.5f);
        }

        v.setDataDimension(XData.ToArray(), View.VIEW_DIMENSION.X);
        v.setDataDimension(YData.ToArray(), View.VIEW_DIMENSION.Y);
        v.setDataDimension(ZData.ToArray(), View.VIEW_DIMENSION.Z);
        v.updateView(null);

        view.AddComponent<MeshFilter>();
        view.AddComponent<MeshRenderer>();

        view.GetComponent<MeshFilter>().mesh = v.MyMesh;
        view.GetComponent<Renderer>().material = m;

        return new Tuple<GameObject, View>(view, v);
    }

    public Tuple<GameObject, View> CreateLinked3DScatterplotsViews(DataObject dobjs, int DimensionX1, int DimensionY1, int DimensionZ1, int DimensionX2, int DimensionY2, int DimensionZ2, Material m)
    {
        float[] x1 = dobjs.getDimension(DimensionX1);
        float[] y1 = dobjs.getDimension(DimensionY1);
        float[] z1 = dobjs.getDimension(DimensionZ1);

        float[] x2 = dobjs.getDimension(DimensionX2);
        float[] y2 = dobjs.getDimension(DimensionY2);
        float[] z2 = dobjs.getDimension(DimensionZ1);

        string viewName = "";
        if (DimensionX1 >= 0) viewName += dobjs.indexToDimension(DimensionX1) + "-";
        if (DimensionY1 >= 0) viewName += dobjs.indexToDimension(DimensionY1) + "-";
        if (DimensionZ1 >= 0) viewName += dobjs.indexToDimension(DimensionZ1) + "-";
        if (DimensionX2 >= 0) viewName += dobjs.indexToDimension(DimensionX2) + "-";
        if (DimensionY2 >= 0) viewName += dobjs.indexToDimension(DimensionY2) + "-";
        if (DimensionZ2 >= 0) viewName += dobjs.indexToDimension(DimensionZ2) + "-";

        View v = new View(MeshTopology.Lines, viewName);
        GameObject view = new GameObject(viewName);
        v.initialiseDataView(dobjs.DataPoints * 2, view);

        List<float> XData = new List<float>();
        List<float> YData = new List<float>();
        List<float> ZData = new List<float>();

        float[] dimx1 = dobjs.getDimension(DimensionX1);
        float[] dimy1 = dobjs.getDimension(DimensionY1);
        float[] dimz1 = dobjs.getDimension(DimensionZ1);
        float[] dimx2 = dobjs.getDimension(DimensionX2);
        float[] dimy2 = dobjs.getDimension(DimensionY2);
        float[] dimz2 = dobjs.getDimension(DimensionZ2);

        for (int i = 0; i < dobjs.DataPoints; i++)
        {
            XData.Add(dimx1[i]);
            YData.Add(dimy1[i]);
            ZData.Add(dimz1[i]);

            XData.Add(dimx2[i]);
            YData.Add(dimy2[i]);
            ZData.Add(dimz2[i]);
        }

        v.setDataDimension(XData.ToArray(), View.VIEW_DIMENSION.X);
        v.setDataDimension(YData.ToArray(), View.VIEW_DIMENSION.Y);
        v.setDataDimension(ZData.ToArray(), View.VIEW_DIMENSION.Z);
        v.updateView(null);

        view.AddComponent<MeshFilter>();
        view.AddComponent<MeshRenderer>();

        view.GetComponent<MeshFilter>().mesh = v.MyMesh;
        view.GetComponent<Renderer>().material = m;

        return new Tuple<GameObject, View>(view, v);
    }

    /// <summary>
    /// Creates a SPLOM from data
    /// </summary>
    /// <param name="dobjs">the DataObject</param>
    /// <returns>A 2D array of game objects views</returns>
    GameObject[,] CreateSPLOM2D(DataObject dobjs, int linkingField, MeshTopology topology, Material material, float spacing)
    {
        GameObject[,] SPLOM = new GameObject[dobjs.Identifiers.Length, dobjs.Identifiers.Length];
        string[] descriptors = dobjs.Identifiers;
        for (int i = 0; i < descriptors.Length; i++)
        {
            for (int j = 0; j < descriptors.Length; j++)
            {
                {
                    GameObject view = CreateSingle2DView(dobjs, i, j, -1, linkingField, topology, material).Item1;
                    view.transform.position = new Vector3((float)i * spacing, -(float)j * spacing, 0f);
                    SPLOM[i, j] = view;
                }
            }
        }
        return SPLOM;
    }

    GameObject[,] CreateSPLOM3D(DataObject dobjs, int linkingField, MeshTopology topology, Material material, float spacing)
    {
        GameObject[,] SPLOM = new GameObject[dobjs.Identifiers.Length, dobjs.Identifiers.Length];
        string[] descriptors = dobjs.Identifiers;
        for (int i = 0; i < descriptors.Length; i++)
        {
            for (int j = 0; j < descriptors.Length; j++)
            {
                for (int k = 0; k < descriptors.Length; k++)
                {
                    if (i != j)
                    {
                        GameObject view = CreateSingle2DView(dobjs, i, j, k, linkingField, topology, material).Item1;
                        view.transform.position = new Vector3((float)i * spacing, -(float)j * spacing, (float)k * spacing);
                        SPLOM[i, j] = view;
                    }
                }
            }
        }
        return SPLOM;
    }

    GameObject CreateLabel(string label, GameObject parent, Vector3 position)
    {
        GameObject TextObject = new GameObject(label);
        TextObject.AddComponent<TextMesh>();
        TextMesh tm = TextObject.GetComponent<TextMesh>();
        tm.text = label;
        TextObject.transform.localPosition = position;
        TextObject.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        tm.fontSize = 108;
        tm.color = Color.black;
        TextObject.AddComponent<BoxCollider>();

        TextObject.transform.parent = parent.transform;

        return TextObject;
    }
}