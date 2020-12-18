using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class SceneManager : MonoBehaviour
{

    public List<Axis> sceneAxes { get; internal set; }

    public DataBinding.DataObject dataObject;

    public class OnAxisAddedEvent : UnityEvent<Axis> { }
    public OnAxisAddedEvent OnAxisAdded = new OnAxisAddedEvent();

    [SerializeField]
    GameObject axisPrefab;

    [Header("Data Source")]

    [SerializeField]
    TextAsset sourceData;

    [SerializeField]
    DataObjectMetadata metadata;

    [SerializeField]
    bool createAxisShelf = true;

    static SceneManager _instance;
    public static SceneManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<SceneManager>()); }
    }


    void Start()
    {
        sceneAxes = new List<Axis>();
        dataObject = new DataBinding.DataObject(sourceData.text, metadata);

        // setup default visual settings

        VisualisationAttributes.Instance.sizes = Enumerable.Range(0, SceneManager.Instance.dataObject.DataPoints).Select(_ => 1f).ToArray();

        List<float> categories = SceneManager.Instance.dataObject.getNumberOfCategories(VisualisationAttributes.Instance.ColoredAttribute);
        int nbCategories = categories.Count;
        Color[] palette = Colors.generateColorPalette(nbCategories);

        Dictionary<float, Color> indexCategoryToColor = new Dictionary<float, Color>();
        for (int i = 0; i < categories.Count; i++)
        {
            indexCategoryToColor.Add(categories[i], palette[i]);
        }

        VisualisationAttributes.Instance.colors = Colors.mapColorPalette(SceneManager.Instance.dataObject.getDimension(VisualisationAttributes.Instance.ColoredAttribute), indexCategoryToColor);

        // create the axis
        if (createAxisShelf)
        {
            for (int i = 0; i < dataObject.Identifiers.Length; ++i)
            {
                Vector3 v = new Vector3(1.352134f - (i % 7) * 0.35f, 1.506231f - (i / 7) / 2f, 0f);// -0.4875801f);
                GameObject obj = (GameObject)Instantiate(axisPrefab);
                obj.transform.position = v;
                Axis axis = obj.GetComponent<Axis>();
                axis.Init(dataObject, i, true);
                axis.InitOrigin(v, obj.transform.rotation);
                axis.tag = "Axis";

                AddAxis(axis);
            }
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateSPLOMS();
        }
    }

    public void AddAxis(Axis axis)
    {
        sceneAxes.Add(axis);
        OnAxisAdded.Invoke(axis);
    }

    public void DestroyAxis(Axis axis)
    {
        sceneAxes.Remove(axis);
        Destroy(axis.gameObject);

        foreach (Visualization vis in axis.correspondingVisualizations())
        {
            Destroy(vis.gameObject);

            foreach (SPLOM3D splom3d in GameObject.FindObjectsOfType<SPLOM3D>())
            {
                if (splom3d.BaseVisualization == vis)
                    Destroy(splom3d.gameObject);
            }
        }
    }

    //
    // Debug functions
    //

    void CreateSPLOMS()
    {
        print("creating the splom");
        Axis[] axes = (Axis[])GameObject.FindObjectsOfType(typeof(Axis));

        GameObject a = axes[0].gameObject;// GameObject.Find("axis horesepower");
        a.transform.position = new Vector3(0f, 1.383f, 0.388f);

        Quaternion qt = new Quaternion();
        qt.eulerAngles = new Vector3(-90f, 180f, 0f);
        a.transform.rotation = qt;

        GameObject b = axes[1].gameObject;
        b.transform.position = new Vector3(0f, 1.506231f, 0.2461f);

        GameObject d = axes[2].gameObject;
        d.transform.position = new Vector3(0.1485f, 1.4145f, 0.2747f);
        qt.eulerAngles = new Vector3(0f, 180f, 90f);
        d.transform.rotation = qt;
    }

    void CreateLSPLOM()
    {
        Quaternion qt = new Quaternion();
        qt.eulerAngles = new Vector3(0f, 180f, 90f);

        GameObject a = GameObject.Find("axis horesepower");
        a.transform.position = new Vector3(0.1018f, 1.369f, -1.3629f);
        a.transform.rotation = qt;

        GameObject b = GameObject.Find("axis weight");
        b.transform.position = new Vector3(-0.04786599f, 1.506231f, -1.356f);

        GameObject c = GameObject.Find("axis mpg");
        c.transform.position = new Vector3(-0.045f, 1.768f, -1.357f);

        GameObject d = GameObject.Find("axis name");
        d.transform.position = new Vector3(-0.047f, 2.03f, -1.354f);

        qt.eulerAngles = new Vector3(0f, 180f, 90f);

        GameObject e = GameObject.Find("axis displacement");
        e.transform.position = new Vector3(0.37f, 1.378f, -1.37f);
        e.transform.rotation = qt;

    }

    void CreateSPLOMCenter()
    {
        Quaternion qt = new Quaternion();
        qt.eulerAngles = new Vector3(0f, 180f, -90f);

        GameObject c = GameObject.Find("axis mpg");
        c.transform.position = new Vector3(1.3173f, 1.7632f, -0.941f);

        GameObject d = GameObject.Find("axis weight");

        d.transform.position = new Vector3(1.173f, 1.6389f, -0.9362f);
        d.transform.rotation = qt;

        //        Quaternion qt = new Quaternion();

        GameObject e = GameObject.Find("axis displacement");

        e.transform.position = new Vector3(0.8942f, 1.64f, -0.938f);
        e.transform.rotation = qt;
    }

    void CreateSPLOMSWithU()
    {
        Axis[] axes = (Axis[])GameObject.FindObjectsOfType(typeof(Axis));
        for (int i = 2; i < 8; ++i)
        {
            axes[i].transform.Translate(i % 2 * (axes[i].transform.localScale.x * 10f), i % 2 * (-axes[i].transform.localScale.x * 6.5f), 1f);
            axes[i].transform.Rotate(0f, 0f, i % 2 * (90f));
        }

        GameObject a = GameObject.Find("axis mpg");
        a.transform.position = new Vector3(0.236f, 1.506231f, -1.486f);
    }
}
