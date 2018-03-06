using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class View
{
    bool UNITY_GAME_OBJECTS_MODE = false;
    List<GameObject> visualObjects = new List<GameObject>();

    public enum VIEW_DIMENSION { X, Y, Z, LINKING_FIELD };

    private Mesh myMesh;

    public Mesh MyMesh
    {
        get { return myMesh; }
        set { myMesh = value; }
    }

    private MeshTopology myMeshTopolgy;

    private List<Vector3> positions = new List<Vector3>();

    public View(MeshTopology type, string viewName)
    {
        myMeshTopolgy = type;
        myMesh = new Mesh();
        name = viewName;
    }

    private string name = "";

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    private float[] brushedIndexes;

    public float[] BrushedIndexes
    {
        get { return brushedIndexes; }
        set { brushedIndexes = value; }
    }

    RenderTexture myRenderTexture;

    public void initialiseDataView(int numberOfPoints, GameObject parent)
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            positions.Add(new Vector3());

            if (UNITY_GAME_OBJECTS_MODE)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.parent = parent.transform;
                visualObjects.Add(go);
            }
        }

        //Debug.Log("Created " + numberOfPoints +" data points");
    }

    
    public void setDataDimension(float[] dat, VIEW_DIMENSION dimension)
    {
        float minValue = dat.Min();
        float maxValue = dat.Max();

        for (int i = 0; i < dat.Length; i++)
        {
            Vector3 p = positions[i];

            switch (dimension)
            {
                case VIEW_DIMENSION.X:
                    p.x = UtilMath.normaliseValue(dat[i], minValue, maxValue, -0.5f, 0.5f);
                    break;
                case VIEW_DIMENSION.Y:
                    p.y = UtilMath.normaliseValue(dat[i], minValue, maxValue, -0.5f, 0.5f);
                    break;
                case VIEW_DIMENSION.Z:
                    p.z = UtilMath.normaliseValue(dat[i], minValue, maxValue, -0.5f, 0.5f);
                    break;
            }
            positions[i] = p;
        }

    }

    public void setDefaultColor()
    {
        Color[] colors = new Color[myMesh.vertices.Length];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        myMesh.colors = colors;
    }

    public void updateView(float[] linking)
    {
        //if (UNITY_GAME_OBJECTS_MODE)
        //    updateGameObjects(0.05f);
        //else 
        if (linking == null)
            updateMeshPositions(null);
        else //create the lines
        {
            updateMeshPositions(linking);
        }
    }

    int[] createIndicesScatterPlot(int numberOfPoints)
    {
        int[] indices = new int[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            indices[i] = i;
        }
        return indices;
    }

    int[] createIndicesLines(float[] linkingField)
    {
        List<int> indices = new List<int>();

        if (linkingField != null)
        {
            for (int i = 0; i < linkingField.Length - 1; i++)
            {
                //Debug.Log(linkingField[i] + "    - - - -        " + linkingField[i + 1]);
                if (linkingField[i] == linkingField[i + 1])
                {
                    indices.Add(i);
                    indices.Add(i + 1);
                }
            }
        }
        else
        {
            // pairwise lines
            for (int i = 0; i < positions.Count - 1; i += 2)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }
        }

        //foreach (int item in indices)
        //{
        //    Debug.Log(item);
        //}
        return indices.ToArray();
    }

    private void updateGameObjects(float scale)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            GameObject go = visualObjects[i];
            go.transform.localScale = new Vector3(scale, scale, scale);
            go.transform.localPosition = positions[i];
            go.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    private void updateMeshPositions(float[] linkingField)
    {
        switch (myMeshTopolgy)
        {
            case MeshTopology.LineStrip:
                myMesh.vertices = positions.ToArray();
                myMesh.SetIndices(createIndicesLines(linkingField), MeshTopology.LineStrip, 0);
                break;
            case MeshTopology.Lines:
                myMesh.vertices = positions.ToArray();
                myMesh.SetIndices(createIndicesLines(linkingField), MeshTopology.Lines, 0);
                break;
            case MeshTopology.Points:
                myMesh.vertices = positions.ToArray();
                myMesh.SetIndices(createIndicesScatterPlot(positions.Count), MeshTopology.Points, 0);
                //updateVertexIndices();
                break;
            case MeshTopology.Quads:
                break;
            case MeshTopology.Triangles:
                break;
            default:
                break;
        }

        updateVertexIndices(0);
        
    }

    /// <summary>
    /// use the normal to store the vertex index
    /// </summary>
    private void updateVertexIndices(int channel)
    {
        Vector3[] norms = new Vector3[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 v = Vector3.zero;
            v[channel] = (float)i;
            norms[i] = v;
        }
        myMesh.normals = norms;
    }

    private void updateSizeChannel(int channel, float[] normalisedValueDimension)
    {
        Vector3[] myMeshNormals = myMesh.normals;

        if (myMeshNormals == null) myMeshNormals = new Vector3[myMesh.vertexCount];
        for(int i=0; i<myMeshNormals.Length;i++)
        {
            Vector3 v = myMeshNormals[i];
            v[channel] = normalisedValueDimension[i];
            myMeshNormals[i] = v;
        }
        myMesh.normals= myMeshNormals;
    }

    public void debugVector3List(List<Vector3> list)
    {
        foreach (Vector3 p in list)
        {
            Debug.Log(p.ToString());
        }
    }

    public void debugVector3List(Vector3[] vector3)
    {
        foreach (Vector3 p in vector3)
        {
            Debug.Log(p.ToString());
        }
    }

    public void mapColorContinuous(float[] dat, Color fromColor, Color toColor)
    {
        List<Color> myColors = new List<Color>();
        for(int i=0;i<dat.Length;i++)
        {
            myColors.Add(Color.Lerp(fromColor, toColor, dat[i]));
        }
        //Debug.Log("vertices count: " + myMesh.vertices.Length + " colors count: " + myColors.Count);
        myMesh.colors = myColors.ToArray();
    }

    public void mapColorCategory(float[] dat, Color[] palette, bool isPCP)
    {
        Color[] colorSet = new Color[dat.Length];
        int cat =0;
        colorSet[0] = palette[cat];
        for(int i=1; i<dat.Length; i++)
        {
            if (dat[i] == dat[i - 1]) colorSet[i] = palette[cat];
            else {
                cat++;
                Debug.Log(cat);
                colorSet[i] = palette[cat];
            }
        }
        setColors(colorSet, isPCP);
    }

    public void setSizes(float[] sizes)
    {
        updateSizeChannel(1, sizes);
    }
    
    public void setColors(Color[] colors, bool isPCP)
    {
        if (colors != null)
        {
            if (/*myMeshTopolgy == MeshTopology.Lines &&*/ isPCP)
            {
                List<Color> colorsLine = new List<Color>();

                for (int i = 0; i < colors.Length; i += 1)
                {
                    colorsLine.Add(colors[i]);
                    colorsLine.Add(colors[i]);
                }
                myMesh.colors = colorsLine.ToArray();

            }
            else
            {
                myMesh.colors = colors;
            }
        }
    }

}