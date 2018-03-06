using System.Collections.Generic;
using UnityEngine;


public class VisualisationAttributes : MonoBehaviour {

    public Color[] colors = null;

    public float[] sizes = null;

    public float ScatterplotDefaultPointSize = 0.4f;

    public float MinScatterplotPointSize = 0.1f;

    public float MaxScatterplotPointSize = 0.9f;
    
    public static bool detailsOnDemand = true;

    public int ColoredAttribute = 0;

    public int LinkedAttribute = -1;

    public float LinkTransparency = 0.4f;

    public int SizeAttribute = -1;
    public bool IsGradientColor = false; // is the colour mapping a gradient or categorical

    public Color MinGradientColor = Color.red;

    public Color MaxGradientColor = Color.blue;


    //Singleton pattern
    protected static VisualisationAttributes _instance;
    public static VisualisationAttributes Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<VisualisationAttributes>()); }
    }

    /// <summary>
    /// maps a color gradient to a continuous variable 
    /// </summary>
    /// <param name="minColor"></param>
    /// <param name="maxColor"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Color[] getContinuousColors(Color minColor, Color maxColor, float[] data)
    {
        Color[] colors = new Color[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            colors[i] = Color.Lerp(minColor, maxColor, data[i]);
        }

        return colors;
    }

    public static Color[] getCategoricalColors(float[] data, Dictionary<float, Color> scheme)
    {
        Color[] thecolors = new Color[data.Length];

        for(int i=0;i<data.Length;i++)
        {
            thecolors[i] = scheme[data[i]];
        }

        return thecolors;
    }

    #region old code

    //public void populatePaletteReference(string attributeName, List<Color> cat)
    //{
    //    List<float> sortedKeys = mappingDiscreteData.Keys.ToList();
    //    sortedKeys.Sort();

    //    GameObject textMesh1 = new GameObject();
    //    textMesh1.transform.parent = transform;
    //    textMesh1.transform.Rotate(0f, 180f, 0f);

    //    textMesh1.transform.position = new Vector3(0f, transform.parent.position.y - 0.09f, 0f);
    //    textMesh1.transform.localScale = Vector3.one / 100f;
    //    textMesh1.AddComponent<TextMesh>();
    //    textMesh1.GetComponent<TextMesh>().color = Color.white;
    //    textMesh1.GetComponent<TextMesh>().fontSize = 25;
    //    textMesh1.GetComponent<TextMesh>().text = attributeName;

    //    for (int i = 0; i < sortedKeys.Count; i++)
    //    {
    //        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //        quad.GetComponent<Renderer>().material.color = mappingDiscreteData[sortedKeys[i]];// cat[i];
    //        quad.transform.parent = transform;
    //        quad.transform.Rotate(0f, 180f, 0f);
    //        quad.transform.position = new Vector3(i*0.1f, transform.parent.position.y-0.1f , 0f);
    //        quad.transform.localScale = Vector3.one * 0.1f;

    //        GameObject textMesh = new GameObject();
    //        textMesh.transform.parent = quad.transform;
    //        textMesh.transform.Rotate(0f, 180f, 0f);

    //        textMesh.transform.localPosition= new Vector3(quad.transform.localScale.x / 2f, quad.transform.localScale.x / 2f, 0.1f);
    //        textMesh.transform.localScale = Vector3.one / 10f;
    //        textMesh.AddComponent<TextMesh>();
    //        textMesh.GetComponent<TextMesh>().color = Color.white;
    //        textMesh.GetComponent<TextMesh>().fontSize = 25;
    //        //            textMesh.GetComponent<TextMesh>().text = mappingDiscreteData.try
    //        //float refValue = -1f;
    //        //TryGetKey(mappingDiscreteData, cat[i], out refValue);
    //        Vector2 rangeX = SceneManager.Instance.dataObject.DimensionsRange[SceneManager.Instance.dataObject.dimensionToIndex(attributeName)];
    //        textMesh.GetComponent<TextMesh>().text = UtilMath.normaliseValue(sortedKeys[i], 0f, 1f, rangeX.x, rangeX.y).ToString();
    //    }
    //}


    //public static List<Color> getPalette(int N_STEPS)
    //{
    //    List<Color> colors = new List<Color>();

    //    for (int i = 0; i < N_STEPS; i++)
    //    {
    //        Color c = Random.ColorHSV(0.6f, 1f);
    //        c.a = 1f;
    //        colors.Add(c);
    //    }
    //    return colors;
    //}

    //static Dictionary<float, Color> mappingDiscreteData = new Dictionary<float, Color>();

    //public static Color[] mapBinaryColor(Color c1, Color c2, float[] value)
    //{
    //    Color[] colors = new Color[value.Length];

    //    for (int i = 0; i < value.Length; i++)
    //    {
    //        if (value[i] > 0f) colors[i] = c2;
    //        else colors[i] = c1;

    //    }

    //        return colors;
    //}

    //public static Color[] mapDiscreteColor(float[] values)
    //{
    //    Color[] colors = new Color[values.Length];
        
    //    for (int i = 0; i < values.Length; i++)
    //    {
    //        if (!mappingDiscreteData.ContainsKey((values[i])))
    //        {
    //            Color c = Random.ColorHSV();
    //            mappingDiscreteData.Add(values[i], c);
    //            colors[i] = c;
    //        }
    //        else
    //        {
    //            colors[i] = mappingDiscreteData[values[i]];
    //        }
    //    }

    //    return colors;
    //}

    //public static List<Color> getNumberofCategories(Color[] values)
    //{
    //    List<Color> types = new List<Color>();

    //    for (int i = 0; i < values.Length; i++)
    //    {
    //        if (!types.Contains(values[i]))
    //            types.Add(values[i]);
    //    }

    //    return types;
    //}

    //public static bool TryGetKey<K, V>(IDictionary<K, V> instance, V value, out K key)
    //{
    //    foreach (var entry in instance)
    //    {
    //        if (!entry.Value.Equals(value))
    //        {
    //            continue;
    //        }
    //        key = entry.Key;
    //        return true;
    //    }
    //    key = default(K);
    //    return false;    
    //}

    #endregion 
}
