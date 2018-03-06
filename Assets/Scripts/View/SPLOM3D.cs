using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Staxes;

public class SPLOM3D : MonoBehaviour, Grabbable
{
    Dictionary<Tuple<int, int, int>, List<GameObject>> ghostAxes = new Dictionary<Tuple<int, int, int>, List<GameObject>>();

    GameObject visualizationPrefab;

    public GameObject VisualizationPrefab
    {
        get { return visualizationPrefab; }
        set { visualizationPrefab = value; }
    }

    Visualization baseVisualization;

    public Visualization BaseVisualization
    {
        get { return baseVisualization; }
        set { baseVisualization = value; }
    }

    List<Axis> XAxes = new List<Axis>();

    public List<Axis> XAxes1
    {
        get { return XAxes; }
        set { XAxes = value; }
    }
    List<Axis> YAxes = new List<Axis>();

    public List<Axis> YAxes1
    {
        get { return YAxes; }
        set { YAxes = value; }
    }

    List<Axis> ZAxes = new List<Axis>();

    public List<Axis> ZAxes1
    {
        get { return ZAxes; }
        set { ZAxes = value; }
    }

    public List<Axis> Axes
    {
        get { return XAxes.Concat(YAxes).Concat(ZAxes).ToList(); }
    }

    List<Visualization> splomVisualizations = new List<Visualization>();

    GameObject SPLOM_HOLDER;

    Axis previousX = null;
    Axis previousY = null;
    Axis previousZ = null;

    Visualization[,,] THE3DSPLOM;

    public enum AxisName { X = 0, Y = 1, Z = 2 };


    void Update3DSPLOM(ref List<Visualization> listOfVisualisations)
    {
        for (int i = 0; i < XAxes.Count; i++)
        {
            for (int j = 0; j < YAxes.Count; j++)
            {
                if (baseVisualization.viewType == Visualization.ViewType.Scatterplot3D)
                {
                    for (int k = 0; k < ZAxes.Count; k++)
                    {
                        if (THE3DSPLOM[i, j, k] == null)
                        {
                            GameObject cloneX = XAxes[i].Clone();
                            GameObject cloneY = YAxes[j].Clone();
                            GameObject cloneZ = ZAxes[k].Clone();

                            float vectorLength = XAxes[i].transform.localScale.y;

                            cloneX.transform.position = XAxes[0].transform.position + YAxes[0].transform.up * vectorLength * j + XAxes[0].transform.up * vectorLength * i + ZAxes[0].transform.up * vectorLength * k;
                            cloneY.transform.position = YAxes[0].transform.position + YAxes[0].transform.up * vectorLength * j + XAxes[0].transform.up * vectorLength * i + ZAxes[0].transform.up * vectorLength * k;
                            cloneZ.transform.position = ZAxes[0].transform.position + YAxes[0].transform.up * vectorLength * j + XAxes[0].transform.up * vectorLength * i + ZAxes[0].transform.up * vectorLength * k;

                            cloneX.transform.rotation = XAxes[0].transform.rotation;
                            cloneY.transform.rotation = YAxes[0].transform.rotation;
                            cloneZ.transform.rotation = ZAxes[0].transform.rotation;

                            GameObject visObj = Instantiate(visualizationPrefab);
                            Visualization vis = visObj.GetComponent<Visualization>();
                            vis.axes.Clear();
                            vis.IsSPLOMElement = true;
                            vis.TheSPLOMReference = this.gameObject;

                            //enable SPLOM linking
                            listOfVisualisations.Add(vis);

                            vis.AddAxis(cloneX.GetComponent<Axis>());
                            vis.AddAxis(cloneY.GetComponent<Axis>());
                            vis.AddAxis(cloneZ.GetComponent<Axis>());

                            cloneX.GetComponent<Axis>().Ghost(XAxes[i]);
                            cloneY.GetComponent<Axis>().Ghost(YAxes[j]);
                            cloneZ.GetComponent<Axis>().Ghost(ZAxes[k]);

                            THE3DSPLOM[i, j, k] = vis;
                            splomVisualizations.Add(vis);

                            Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(i, j, k);
                            if (!ghostAxes.ContainsKey(indexInsplom))
                                ghostAxes.Add(indexInsplom, new List<GameObject>(new GameObject[] { cloneX, cloneY, cloneZ }));

                        }
                    }
                }
                else
                {
                    if (THE3DSPLOM[i, j, 0] == null)
                    {
                        GameObject cloneX = XAxes[i].Clone();
                        GameObject cloneY = YAxes[j].Clone();
                        float vectorLength = XAxes[i].transform.localScale.y;

                        cloneX.transform.position = XAxes[0].transform.position + YAxes[0].transform.up * (vectorLength) * ((float)j) + XAxes[0].transform.up * (vectorLength) * ((float)i);
                        cloneY.transform.position = YAxes[0].transform.position + YAxes[0].transform.up * (vectorLength) * ((float)j) + XAxes[0].transform.up * (vectorLength) * ((float)i);

                        cloneX.transform.rotation = XAxes[0].transform.rotation;
                        cloneY.transform.rotation = YAxes[0].transform.rotation;

                        GameObject visObj = Instantiate(visualizationPrefab);
                        Visualization vis = visObj.GetComponent<Visualization>();
                        vis.axes.Clear();
                        vis.IsSPLOMElement = true;
                        vis.TheSPLOMReference = this.gameObject;

                        //enable SPLOM linking
                        listOfVisualisations.Add(vis);

                        vis.AddAxis(cloneX.GetComponent<Axis>());
                        vis.AddAxis(cloneY.GetComponent<Axis>());

                        cloneX.GetComponent<Axis>().Ghost(XAxes[i]);
                        cloneY.GetComponent<Axis>().Ghost(YAxes[j]);

                        THE3DSPLOM[i, j, 0] = vis;
                        splomVisualizations.Add(vis);

                        Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(i, j, 0);
                        if (!ghostAxes.ContainsKey(indexInsplom))
                            ghostAxes.Add(indexInsplom, new List<GameObject>(new GameObject[] { cloneX, cloneY }));

                    }
                } // end if
            } // end for j
        } // end for i
    }

    public void ClearSplom(ref List<Visualization> listOfVisualisations)
    {
        foreach (var vis in listOfVisualisations.Where(v => v.axes.Count == 1))
        {
            foreach (var x in XAxes1)
            {
                if (x == vis.axes[0]) vis.ShowHistogram(true);
            }

            foreach (var x in YAxes1)
            {
                if (x == vis.axes[0]) vis.ShowHistogram(true);
            }

            foreach (var x in ZAxes1)
            {
                if (x == vis.axes[0]) vis.ShowHistogram(true);
            }
        }

        deleteS3DFromXAxes(0, ref listOfVisualisations);
        deleteS3DFromYAxes(0, ref listOfVisualisations);
        deleteS3DFromZAxes(0, ref listOfVisualisations);

        foreach (var axis in ghostAxes)
        {
            foreach (var item in axis.Value)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }
    }

    public void initialiseBaseScatterplot(Visualization visualizationBase, Axis x, Axis y, Axis z)
    {
        THE3DSPLOM = new Visualization[SceneManager.Instance.dataObject.NbDimensions, SceneManager.Instance.dataObject.NbDimensions, SceneManager.Instance.dataObject.NbDimensions];

        baseVisualization = visualizationBase;
        visualizationBase.TheSPLOMReference = this.gameObject;

        if (x != null) XAxes.Add(x);
        if (y != null) YAxes.Add(y);
        if (z != null) ZAxes.Add(z);

        THE3DSPLOM[0, 0, 0] = visualizationBase;
    }

    public void updateXAxes(Axis x, ref List<Visualization> SP_ImAxes)
    {
        XAxes.Add(x);
        previousX = x;
        Update3DSPLOM(ref SP_ImAxes);
        //hide the histogram on the Axis                
        Visualization v = SP_ImAxes.Single(vis => vis.axes.Count == 1 && vis.axes[0] == x);
        v.ShowHistogram(false);
    }

    public void updateYAxes(Axis y, ref List<Visualization> SP_ImAxes)
    {
        YAxes.Add(y);
        previousY = y;
        Update3DSPLOM(ref SP_ImAxes);
        //hide the histogram on the Axis                
        Visualization v = SP_ImAxes.Single(vis => vis.axes.Count == 1 && vis.axes[0] == y);
        v.ShowHistogram(false);
    }

    public void updateZAxes(Axis z, ref List<Visualization> SP_ImAxes)
    {
        print("ADDED >> Z");
        ZAxes.Add(z);
        previousZ = z;
        Update3DSPLOM(ref SP_ImAxes);
        //hide the histogram on the Axis                
        Visualization v = SP_ImAxes.Single(vis => vis.axes.Count == 1 && vis.axes[0] == z);
        v.ShowHistogram(false);
    }

    // this function will align a major axis of the SPLOM correctly to the axis
    public void AlignAxisToSplom(Axis axis)
    {
        int yidx = YAxes.IndexOf(axis);
        int xidx = XAxes.IndexOf(axis);
        int zidx = ZAxes.IndexOf(axis);

        if (yidx > 0)
        {
            Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(0, yidx, 0);            
            var result = ghostAxes[indexInsplom];
            axis.AnimateTo(result[1].transform.position, result[1].transform.rotation);            
        }
        else if (xidx > 0)
        {
            Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(xidx, 0, 0);
            var result = ghostAxes[indexInsplom];
            axis.AnimateTo(result[0].transform.position, result[0].transform.rotation);
        }
        else if (zidx > 0)
        {
            Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(0, 0, zidx);
            var result = ghostAxes[indexInsplom];
            axis.AnimateTo(result[2].transform.position, result[2].transform.rotation);
        }
    }

    internal void deleteS3DFromXAxes(int indexX, ref List<Visualization> SP_ImAxes)
    {

        for (int i = indexX; i < XAxes.Count; i++) // delete from the index of the broken axis to the end
        {
            for (int j = 0; j < YAxes.Count; j++) // for all lines
            {
                for (int k = 0; k <= ZAxes.Count; k++) // for all lines
                {
                    //delete the corresponding columns
                    if (THE3DSPLOM[i, j, k] != null)
                    {
                        SP_ImAxes.Remove(THE3DSPLOM[i, j, k]);

                        Destroy(THE3DSPLOM[i, j, k].gameObject);
                        THE3DSPLOM[i, j, k] = null;

                        Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(i, j, k);
                        if (ghostAxes.ContainsKey(indexInsplom))
                        {
                            //if (ghostAxes[indexInsplom]!=null)
                            foreach (var item in ghostAxes[indexInsplom])
                            {
                                if (item != null)
                                    Destroy(item);
                            }

                            ghostAxes.Remove(indexInsplom);
                        }
                    }

                }
            }
        }

        copyXAxesListToIndex(indexX);
    }

    internal void deleteS3DFromYAxes(int indexY, ref List<Visualization> SP_ImAxes)
    {
        for (int i = 0; i < XAxes.Count; i++) // for all columns
        {
            for (int j = indexY; j < YAxes.Count; j++) // delete from the index of the broken axis to the end
            {
                for (int k = 0; k <= ZAxes.Count; k++) // for all lines
                {

                    //delete the corresponding columns
                    if (THE3DSPLOM[i, j, k] != null)
                    {
                        SP_ImAxes.Remove(THE3DSPLOM[i, j, k]);

                        Destroy(THE3DSPLOM[i, j, k].gameObject);
                        THE3DSPLOM[i, j, k] = null;

                        Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(i, j, k);
                        if (ghostAxes.ContainsKey(indexInsplom))
                        {
                            //if (ghostAxes[indexInsplom]!=null)
                            foreach (var item in ghostAxes[indexInsplom])
                            {
                                if (item != null)
                                    Destroy(item);
                            }
                            ghostAxes.Remove(indexInsplom);

                        }
                    }
                }
            }
        }
        copyYAxesListToIndex(indexY);

    }

    internal void deleteS3DFromZAxes(int indexZ, ref List<Visualization> SP_ImAxes)
    {
        for (int i = 0; i < XAxes.Count; i++) // for all columns
        {
            for (int j = 0; j < YAxes.Count; j++) // delete from the index of the broken axis to the end
            {
                for (int k = indexZ; k <= ZAxes.Count; k++) // for all lines
                {

                    //delete the corresponding columns
                    if (THE3DSPLOM[i, j, k] != null)
                    {
                        SP_ImAxes.Remove(THE3DSPLOM[i, j, k]);

                        Destroy(THE3DSPLOM[i, j, k].gameObject);
                        THE3DSPLOM[i, j, k] = null;

                        Tuple<int, int, int> indexInsplom = new Tuple<int, int, int>(i, j, k);
                        if (ghostAxes.ContainsKey(indexInsplom))
                        {
                            foreach (var item in ghostAxes[indexInsplom])
                            {
                                if (item != null)
                                    Destroy(item);
                            }
                            ghostAxes.Remove(indexInsplom);

                        }
                    }
                }
            }
        }

        copyZAxesListToIndex(indexZ);

    }

    void copyXAxesListToIndex(int index)
    {
        List<Axis> returnList = new List<Axis>();

        for (int i = 0; i < index; i++)
            returnList.Add(XAxes[i]);
        XAxes = returnList;
    }

    void copyYAxesListToIndex(int index)
    {
        List<Axis> returnList = new List<Axis>();

        for (int i = 0; i < index; i++)
            returnList.Add(YAxes[i]);

        YAxes1 = returnList;
    }

    void copyZAxesListToIndex(int index)
    {
        List<Axis> returnList = new List<Axis>();

        for (int i = 0; i < index; i++)
            returnList.Add(ZAxes[i]);

        ZAxes1 = returnList;
    }

    public int GetPriority()
    {
        return 0;
    }

    public bool OnGrab(WandController controller)
    {
        foreach (var item in XAxes)
        {
            controller.PropergateOnGrab(item.gameObject);
        }
        foreach (var item in YAxes)
        {
            controller.PropergateOnGrab(item.gameObject);
        }
        foreach (var item in ZAxes)
        {
            controller.PropergateOnGrab(item.gameObject);
        }
        foreach (var item in ghostAxes)
        {
            item.Value.ForEach(x => { if (x != null) controller.PropergateOnGrab(x.gameObject); });
        }
        return false;
    }

    public void OnRelease(WandController controller)
    { }

    public void OnDrag(WandController controller)
    { }

    public void OnEnter(WandController controller)
    { }

    public void OnExit(WandController controller)
    { }
}
