using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ImAxesRecognizer : MonoBehaviour
{
    [SerializeField]
    GameObject visualizationPrefab;

    AdjacencyMatrix<Visualization> adjacency;

    //holds the names of the created linked visualisations

    public float ADJACENCY_DISTANCE = 0.25f;

    int stopTest = 0;

    //New Staxes Scene Parsing

    List<Axis> A = new List<Axis>();

    public List<Visualization> SP = new List<Visualization>();
    List<Visualization> PCP = new List<Visualization>();

    public List<Axis> usedAxisIn3DSP = new List<Axis>();
    public List<Axis> usedAxisIn2DSP = new List<Axis>();

    public float SP_DISTANCE = 0.05f;
    public float SP_DISTANCE_SQR = 0.05f * 0.05f;
    public float PCP_DISTANCE = 0.00001f;
    public float SP_MIDPOINT_DISTANCE = 0.25f;

    static ImAxesRecognizer _instance;
    public static ImAxesRecognizer Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ImAxesRecognizer>()); }
    }


    void Start()
    {
        adjacency = new AdjacencyMatrix<Visualization>(SceneManager.Instance.sceneAxes.Count);
        SceneManager.Instance.OnAxisAdded.AddListener(OnAxisAdded);
        SceneManager.Instance.OnAxisDestroyed.AddListener(OnAxisAdded);
    }

    void OnAxisAdded(Axis axis)
    {
        adjacency.Resize(SceneManager.Instance.sceneAxes.Count);
    }

    void OnAxisDestroyed(Axis axis)
    {
        adjacency.Resize(SceneManager.Instance.sceneAxes.Count);
    }

    //RULES SP =====================

    //RSP1:
    bool RSP1(Axis a, Axis b)
    {
        return (a.IsPerpendicular(b) &&
                ((b.MinPosition - a.MaxPosition).sqrMagnitude < SP_DISTANCE_SQR
                    || (b.MinPosition - a.MinPosition).sqrMagnitude < SP_DISTANCE_SQR
                    || (b.MaxPosition - a.MaxPosition).sqrMagnitude < SP_DISTANCE_SQR));
    }

    bool RSP1(Axis a, Axis b, Axis c)
    {
        return

           a.IsPerpendicular(b)
           && a.IsPerpendicular(c)
           && b.IsPerpendicular(c)
           &&
            ((Vector3.Distance(b.MinPosition, a.MinPosition) < SP_DISTANCE && (Vector3.Distance(b.MinPosition, c.MinPosition) < SP_DISTANCE))
            || ((Vector3.Distance(b.MinPosition, a.MinPosition) < SP_DISTANCE && (Vector3.Distance(b.MinPosition, c.MaxPosition) < SP_DISTANCE)))
            || ((Vector3.Distance(b.MaxPosition, a.MinPosition) < SP_DISTANCE && (Vector3.Distance(b.MaxPosition, c.MaxPosition) < SP_DISTANCE)))
            || ((Vector3.Distance(b.MaxPosition, a.MinPosition) < SP_DISTANCE && (Vector3.Distance(b.MaxPosition, c.MinPosition) < SP_DISTANCE)))
            || ((Vector3.Distance(b.MinPosition, a.MaxPosition) < SP_DISTANCE && (Vector3.Distance(b.MinPosition, c.MinPosition) < SP_DISTANCE)))
            || ((Vector3.Distance(b.MinPosition, a.MaxPosition) < SP_DISTANCE && (Vector3.Distance(b.MinPosition, c.MaxPosition) < SP_DISTANCE)))
            || ((Vector3.Distance(b.MaxPosition, a.MaxPosition) < SP_DISTANCE && (Vector3.Distance(b.MaxPosition, c.MaxPosition) < SP_DISTANCE)))
            )
           ;
    }

    bool RSP1_Distance(Axis a, Axis b, Axis c)
    {
        return a.Distance(b) > SP_MIDPOINT_DISTANCE || a.Distance(c) > SP_MIDPOINT_DISTANCE
            || !a.IsPerpendicular(b) || a.IsParallel(b)
            || !a.IsPerpendicular(c) || a.IsParallel(c)
            || !b.IsPerpendicular(c) || b.IsParallel(c)
             ;
    }

    bool RSP1_Distance(Axis a, Axis b)
    {

        return a.Distance(b) > SP_MIDPOINT_DISTANCE || !a.IsPerpendicular(b) || a.IsParallel(b);
    }

    //bool RSP2(Axis a, Axis b)
    //{
    //    return (Vector3.Distance(a.MinPosition, b.MaxPosition) < SP_DISTANCE && !a.IsParallel(b));
    //}

    bool RPCP1(Visualization a, Visualization b)
    {
        return (Vector3.Distance(a.transform.position, b.transform.position) < PCP_DISTANCE);
    }

    bool RPCP1_Distance(Visualization a, Visualization b)
    {
        if (a != null && b != null)
            return (Vector3.Distance(a.transform.position, b.transform.position) > PCP_DISTANCE);
        else return false;
    }

    List<Axis> findSplomArrangement(List<Axis> axes)
    {
        List<Axis> list = new List<Axis>();

        //Step 1: find a

        return list;
    }

    void OnGUII()
    {
        string SPs = "SPs: " + "\n";
        string usedin3DSP = "Used3DSP:" + "\n";
        string linkedVisualisations = "linked visus:\n";
        foreach (var item in SP)
        {
            SPs += "-- " + item.ToString() + " " + item.viewType + "\n";
        }
        foreach (var item in usedAxisIn3DSP)
        {
            usedin3DSP += "-- " + item.ToString() + "\n";
        }

        GUI.Label(new Rect(10, 10, 400, 2000), SPs);
        GUI.Label(new Rect(410, 10, 200, 2000), usedin3DSP);
        GUI.Label(new Rect(610, 10, 200, 2000), "nb of SPs:" + SP.Count.ToString());
        GUI.Label(new Rect(810, 10, 200, 2000), "nb of SPs:" + linkedVisualisations);
    }

    void ParseScene_V2()
    {
        // get all the current axes
        A = SceneManager.Instance.sceneAxes;
        // ==============================================================================
        // ======================== PASS0: PARSING SPs ==================================
        // ==============================================================================

        foreach (var axis in A)
        {
            if(axis!=null)
            axis.UpdateCoords();
        }

        // Pass 0: Producing SPs
        // Stage 1: produce SPs of degree 3
        for (int i = 0; i < A.Count; i++)
        {
            for (int j = 0; j < A.Count; j++)
            {
                for (int k = 0; k < A.Count; k++)
                {
                    if (k == j || k == i || i == j)
                        continue;

                    if (A[i].isPrototype && A[j].isPrototype && A[k].isPrototype)
                        continue;

                    if (RSP1(A[i], A[j], A[k]) && adjacency[i, j, k] == null)
                    {
                        //create a 3D SPLOM
                        // create a visualization object
                        GameObject visObj = (GameObject)Instantiate(visualizationPrefab);
                        Visualization vis = visObj.GetComponent<Visualization>();

                        vis.AddAxis(A[i]);
                        vis.AddAxis(A[j]);
                        vis.AddAxis(A[k]);

                        adjacency[i, j, k] = vis;

                        if (SP.Any(x => x.axes.Count == 1 && x.axes.Contains(A[i])))
                        {
                            var toRemove = SP.Single(x => x.axes.Count == 1 && x.axes.Contains(A[i]));
                            Destroy(SP.Find(x => x.axes.Count == 1 && x.axes.Contains(A[i])).gameObject);
                            SP.Remove(toRemove);
                        }
                        if (SP.Any(x => x.axes.Count == 1 && x.axes.Contains(A[j])))
                        {
                            var toRemove = SP.Single(x => x.axes.Count == 1 && x.axes.Contains(A[j]));
                            Destroy(SP.Find(x => x.axes.Count == 1 && x.axes.Contains(A[j])).gameObject);
                            SP.Remove(toRemove);

                        }
                        if (SP.Any(x => x.axes.Count == 1 && x.axes.Contains(A[k])))
                        {
                            var toRemove = SP.Single(x => x.axes.Count == 1 && x.axes.Contains(A[k]));
                            Destroy(SP.Find(x => x.axes.Count == 1 && x.axes.Contains(A[k])).gameObject);
                            SP.Remove(toRemove);
                        }

                        //add it to 3D SPLOM LIST
                        SP.Add(vis);

                        //add the 3 axes to the used axis in 3DSPLOMS
                        if (!usedAxisIn3DSP.Contains(A[i]))
                            usedAxisIn3DSP.Add(A[i]);
                        if (!usedAxisIn3DSP.Contains(A[j]))
                            usedAxisIn3DSP.Add(A[j]);
                        if (!usedAxisIn3DSP.Contains(A[k]))
                            usedAxisIn3DSP.Add(A[k]);

                    }
                    //destroy the visualisation if not satisfying RSP1 anymore
                    else if (RSP1_Distance(A[i], A[j], A[k]) && adjacency[i, j, k] != null)
                    {
                        Visualization v = adjacency[i, j, k];

                        //destroy the visualisation
                        adjacency[i, j, k] = null;
                        if (v != null)
                        {
                            // >>>>>>>>>>> HERE WHEN A 2D SP BECOMES A 3D SP <<<<<<<<<<<<
                            //Clean the memory lists
                            SPLOM3D sp = this.SPLOMS3D.Find(x => x.BaseVisualization == v);
                            if (sp != null)
                            {
                                this.SPLOMS3D.Remove(sp);
                                //sp.showAllHistogramsOnClear();
                                sp.ClearSplom(ref SP);
                                Destroy(sp.gameObject);
                            }

                            //Clean the memory lists
                            SP.Remove(v);
                            Destroy(v.gameObject);
                        }

                        //Only remove from the list if no other 3DSP is using it
                        if (!SP.Any(x => x.axes.Count == 3 && x.axes.Contains(A[i])))
                            usedAxisIn3DSP.Remove(A[i]);
                        if (!SP.Any(x => x.axes.Count == 3 && x.axes.Contains(A[j])))
                            usedAxisIn3DSP.Remove(A[j]);
                        if (!SP.Any(x => x.axes.Count == 3 && x.axes.Contains(A[k])))
                            usedAxisIn3DSP.Remove(A[k]);
                    }
                }
            }
        }

        // Pass 0:
        // Stage 2: produce SPs of degree 2
        // RULE: Degree 3 consumes lower degrees
        for (int i = 0; i < A.Count; i++)
        {
            for (int j = 0; j < A.Count; j++)
            {
                if (i == j)
                    continue;

                if (A[i].isPrototype && A[j].isPrototype)
                    continue;

                if ((RSP1(A[i], A[j])) &&
                    adjacency[i, j, i] == null &&
                    !usedAxisIn3DSP.Contains(A[i]) &&
                    !usedAxisIn3DSP.Contains(A[j]))
                {
                    // create a visualization object
                    GameObject visObj = (GameObject)Instantiate(visualizationPrefab);
                    Visualization vis = visObj.GetComponent<Visualization>();
                    vis.AddAxis(A[i]);
                    vis.AddAxis(A[j]);

                    if (SP.Any(x => x.axes.Count == 1 && x.axes.Contains(A[i])))
                    {
                        var toRemove = SP.Single(x => x.axes.Count == 1 && x.axes.Contains(A[i]));
                        Destroy(SP.Find(x => x.axes.Count == 1 && x.axes.Contains(A[i])).gameObject);
                        SP.Remove(toRemove);
                    }
                    if (SP.Any(x => x.axes.Count == 1 && x.axes.Contains(A[j])))
                    {
                        var toRemove = SP.Single(x => x.axes.Count == 1 && x.axes.Contains(A[j]));
                        Destroy(SP.Find(x => x.axes.Count == 1 && x.axes.Contains(A[j])).gameObject);
                        SP.Remove(toRemove);
                    }

                    adjacency[i, j, i] = vis;
                    adjacency[i, j, j] = vis;

                    if (!usedAxisIn2DSP.Contains(A[i]))
                        usedAxisIn2DSP.Add(A[i]);
                    if (!usedAxisIn2DSP.Contains(A[j]))
                        usedAxisIn2DSP.Add(A[j]);

                    SP.Add(vis);
                    //create a 2D SPLOM if only A[i] and A[j] do not belong to 3D SPLOM LIST
                    //add it to 3D SPLOM LIST

                }
                else if (usedAxisIn3DSP.Contains(A[i]) &&
                    usedAxisIn3DSP.Contains(A[j]) && adjacency[i, j, i] != null)
                {

                    //destroy the visualisation
                    Visualization v = adjacency[i, j, i];
                    adjacency[i, j, i] = null;

                    if (v != null)
                    {
                       // >>>>>>>>>>> HERE WHEN A 2D SP BECOMES A 3D SP <<<<<<<<<<<<
                        //Clean the memory lists
                        SPLOM3D sp = SPLOMS3D.Find(x => x.BaseVisualization == v);
                        if (sp != null)
                        {
                            SPLOMS3D.Remove(sp);
                            //sp.showAllHistogramsOnClear();
                            sp.ClearSplom(ref SP);
                            Destroy(sp.gameObject);
                        }


                        SP.Remove(v);
                        Destroy(v.gameObject);
                    }

                    //Only remove from the list if no other 2DSP is using it
                    if (!SP.Any(x => x.axes.Count == 2 && x.axes.Contains(A[i])))
                        usedAxisIn2DSP.Remove(A[i]);
                    if (!SP.Any(x => x.axes.Count == 2 && x.axes.Contains(A[j])))
                        usedAxisIn2DSP.Remove(A[j]);
                }

                else if (RSP1_Distance(A[i], A[j]) && adjacency[i, j, i] != null)
                {
                    //destroy the visualisation
                    Visualization v = adjacency[i, j, i];

                    SPLOM3D sp = SPLOMS3D.Find(x => x.BaseVisualization == v);
                    if (sp != null)
                    {
                        SPLOMS3D.Remove(sp);
                        //sp.showAllHistogramsOnClear();
                        sp.ClearSplom(ref SP);
                        Destroy(sp.gameObject);
                    }

                    adjacency[i, j, i] = null;

                    if (v != null)
                    {
                        //Clean the memory lists
                        SP.Remove(v);
                        Destroy(v.gameObject);

                    }
                    //Only remove from the list if no other 2DSP is using it
                    if (!SP.Any(x => x.axes.Count == 2 && x.axes.Contains(A[i])))
                        usedAxisIn2DSP.Remove(A[i]);
                    if (!SP.Any(x => x.axes.Count == 2 && x.axes.Contains(A[j])))
                        usedAxisIn2DSP.Remove(A[j]);
                }
            }
        }

        // Pass 0:
        // Stage 2: produce 1D Sps
        for (int i = 0; i < A.Count; i++)
        {
            // if A[i] does not belong to any higher order visualisation,
            // enable histogram and
            if (!SP.Any(x => x.axes.Count == 1 && x.axes[0] == A[i])
                && !usedAxisIn2DSP.Contains(A[i])
                && !usedAxisIn3DSP.Contains(A[i]))
            {
                GameObject visObj = (GameObject)Instantiate(visualizationPrefab);

                Visualization vis = visObj.GetComponent<Visualization>();
                vis.AddAxis(A[i]);
                SP.Add(vis);
            }
        }

        // Pass 0:
        // Stage 3: produce Scatterplot Matrices
        for (int i = 0; i < SP.Count; i++)
        {
            // Pass 0:
            // Stage 4: produce Scatterplot 3D Matrices
            if ((SP[i].viewType == Visualization.ViewType.Scatterplot3D// SPLOM3Ds build on scatterplots 3D
                || SP[i].viewType == Visualization.ViewType.Scatterplot2D)                                                                    // && SP[i].IsBaseSPLOM                               //
                && !SPLOMS3D.Any(x => x.BaseVisualization == SP[i]) && !SP[i].IsSPLOMElement) //

            {

                //the SP is now a base for a SPLOM
                List<Axis> visuAxes = SP[i].axes;
                Axis x3D = SP[i].ReferenceAxis1.horizontal;// visuAxes[0].IsHorizontal ? visuAxes[0] : visuAxes[1].IsHorizontal ? visuAxes[1] : visuAxes[2];
                Axis y3D = SP[i].ReferenceAxis1.vertical; //visuAxes.First(x => x != x3D && x.IsVertical);// visuAxes[0] == x3D ? visuAxes[1] : visuAxes[2] == x3D? visuAxes[0] : visuAxes[1];
                Axis z3D = SP[i].ReferenceAxis1.depth; //visuAxes.First(x => x != x3D && x != y3D);

                GameObject splomHolder = new GameObject("ScatterplotMatrix " + SP[i].name);
                //create a base SPLOM
                SPLOM3D newSplom = splomHolder.AddComponent<SPLOM3D>();// new SPLOM();
                newSplom.initialiseBaseScatterplot(SP[i], x3D, y3D, z3D);
                newSplom.VisualizationPrefab = visualizationPrefab;
                SPLOMS3D.Add(newSplom);

            }

        }

        //update Scatterplot matrices 3D
        for (int sp3d = 0; sp3d < SPLOMS3D.Count; sp3d++)// (var splom3D in SPLOMS3D)
        {
            SPLOM3D splom3D = SPLOMS3D[sp3d];

            Axis x3D = null;
            if (splom3D.XAxes1.Count>0)
            x3D = splom3D.XAxes1.Last(); // get the last X axis
            Axis y3D = null;
            if(splom3D.YAxes1.Count>0)
            y3D = splom3D.YAxes1.Last(); // get the last Y axis

            Axis z3D = null;
            if (splom3D.ZAxes1.Count>0)
            z3D = splom3D.ZAxes1.Last(); // get the last Z axis

            //look for a new axis
            Axis newXAxis = null;
            Axis newYAxis = null;
            Axis newZAxis = null;

            float SPLOM_DISTANCE = SP_DISTANCE / 2f;
            float SPLOM_TIP_OFFSET = 0.025f;

            foreach (var axis in A)
            {
                if (x3D!=null && Vector3.Distance(x3D.MaxPosition + x3D.Up * SPLOM_TIP_OFFSET, axis.MinPosition) < SPLOM_DISTANCE && x3D.IsColinear(axis)
                    && !splom3D.XAxes1.Contains(axis))
                    newXAxis = axis;
                else if (y3D != null && Vector3.Distance(y3D.MaxPosition + y3D.Up * SPLOM_TIP_OFFSET, axis.MinPosition) < SPLOM_DISTANCE && y3D.IsColinear(axis)
                    && !splom3D.YAxes1.Contains(axis))
                    newYAxis = axis;
                else if (z3D!=null && Vector3.Distance(z3D.MaxPosition + z3D.Up * SPLOM_TIP_OFFSET, axis.MinPosition) < SPLOM_DISTANCE && z3D.IsColinear(axis)
                    && !splom3D.ZAxes1.Contains(axis))
                    newZAxis = axis;
            }

            int indexX = -1;
            int indexY = -1;
            int indexZ = -1;

            if (newXAxis != null) // => new x axis found on splom 2D
                splom3D.updateXAxes(newXAxis, ref SP);
            else // look for removed axis on the X axis of the SPLOM
            {
                for (int i = 0; i < splom3D.XAxes1.Count - 1; i++)
                {
                    //look for the first broken axis
                    if (Vector3.Distance(splom3D.XAxes1[i].MaxPosition + splom3D.XAxes1[i].Up * SPLOM_TIP_OFFSET, splom3D.XAxes1[i + 1].MinPosition) > SPLOM_DISTANCE)
                    //broken axis
                    {
                        indexX = i + 1;
                        break;
                    }
                }
            }
            if (newYAxis != null) // => new y axis found on splom 2D
                splom3D.updateYAxes(newYAxis, ref SP);
            else
            {
                for (int i = 0; i < splom3D.YAxes1.Count - 1; i++)
                {
                    //look for the first broken axis
                    if (Vector3.Distance(splom3D.YAxes1[i].MaxPosition + splom3D.YAxes1[i].Up * SPLOM_TIP_OFFSET, splom3D.YAxes1[i + 1].MinPosition) > SPLOM_DISTANCE)
                    //broken axis
                    {
                        indexY = i + 1;
                        break;
                    }
                }
            }

            if (newZAxis != null) // => new y axis found on splom 2D
                splom3D.updateZAxes(newZAxis, ref SP);
            else
            {
                for (int i = 0; i < splom3D.ZAxes1.Count - 1; i++)
                {
                    //look for the first broken axis
                    if (Vector3.Distance(splom3D.ZAxes1[i].MaxPosition + splom3D.ZAxes1[i].Up * SPLOM_TIP_OFFSET, splom3D.ZAxes1[i + 1].MinPosition) > SPLOM_DISTANCE)
                    //broken axis
                    {
                        indexZ = i + 1;
                        break;
                    }
                }
            }


            if (indexX >= 0)
            {
                //show histograms on disconnected axes
                for (int i = indexX; i < splom3D.XAxes1.Count; i++)
                {
                    foreach (var sp1 in SP.Where(x=>x.axes.Count==1))
                    {
                        if (sp1.axes[0] == splom3D.XAxes1[i])
                            sp1.ShowHistogram(true);
                    }
                }
                splom3D.deleteS3DFromXAxes(indexX, ref SP);
            }
            if (indexY >= 0)
            {
                for (int i = indexY; i < splom3D.YAxes1.Count; i++)
                {
                    foreach (var sp1 in SP.Where(x => x.axes.Count == 1))
                    {
                        if (sp1.axes[0] == splom3D.YAxes1[i])
                            sp1.ShowHistogram(true);
                    }
                }
                splom3D.deleteS3DFromYAxes(indexY, ref SP);
            }
            if (indexZ >= 0)
            {
                for (int i = indexZ; i < splom3D.ZAxes1.Count; i++)
                {
                    foreach (var sp1 in SP.Where(x => x.axes.Count == 1))
                    {
                        if (sp1.axes[0] == splom3D.ZAxes1[i])
                            sp1.ShowHistogram(true);
                    }
                }
                splom3D.deleteS3DFromZAxes(indexZ, ref SP);
            }
        }

        // ==============================================================================
        // ======================== PASS1: PARSING PCPs =================================
        // ==============================================================================
        //Pass1: enable close linked visualisations

        //HACK: THIS IS A COMPLETE BANDAID TO FIX THE FLOATING VISUALIZATION BUG,
        //TODO: We should be trying to work out why the visualization bug is occuring in the first place.

        for(var i = SP.Count - 1; i >= 0; i--)
        {
            if (SP[i] == null)
                SP.RemoveAt(i);
        }

        //TODO: END OF HACK


        for (int i = 0; i < SP.Count; i++)
            for (int j = 0; j < SP.Count; j++)
            {
                if (i != j)
                {
                    if (SP[i].viewType != Visualization.ViewType.Histogram)
                    {
                        if (SP[i].axes.Select(x => x.ghostSourceAxis).Where(x => x != null).Intersect(SP[j].axes).Count() > 0)
                            continue;
                    }

                    if (SP[j].viewType != Visualization.ViewType.Histogram)
                    {
                        if (SP[j].axes.Select(x => x.ghostSourceAxis).Where(x => x != null).Intersect(SP[i].axes).Count() > 0)
                            continue;
                    }

                    string _name = SP[i] + "-" + SP[j];
                    string _nameReverse = SP[j] + "-" + SP[i];

                    //test the distance between 2 axes if linking 2 histograms
                    if (SP[i].viewType == Visualization.ViewType.Histogram && SP[j].viewType == Visualization.ViewType.Histogram)
                    {
                        if (SP[i].transform.position != SP[j].transform.position
                            && Vector3.Distance(SP[i].axes[0].transform.position, SP[j].axes[0].transform.position) < PCP_DISTANCE
                            && !linkedVisualisationDictionary.ContainsKey(_name) && !linkedVisualisationDictionary.ContainsKey(_nameReverse))
                        {
                            SP[i].ShowHistogram(false);
                            SP[j].ShowHistogram(false);

                            LinkVisualisations(_name, SP[i], SP[j]);
                        }
                    }
                    else if (SP[i].viewType == Visualization.ViewType.Histogram && SP[j].viewType != Visualization.ViewType.Histogram)
                    {

                        if (SP[i].axes[0].transform.position != SP[j].transform.position
                            && Vector3.Distance(SP[i].axes[0].transform.position, SP[j].transform.position) < PCP_DISTANCE
                            && !linkedVisualisationDictionary.ContainsKey(_name) && !linkedVisualisationDictionary.ContainsKey(_nameReverse))
                        //     && !SP[j].IsSPLOMElement)
                        {
                            if (SP[i].viewType == Visualization.ViewType.Histogram) SP[i].ShowHistogram(false);
                            if (SP[j].viewType == Visualization.ViewType.Histogram) SP[j].ShowHistogram(false);

                            LinkVisualisations(_name, SP[i], SP[j]);
                        }
                    }
                    else
                    {
                        if (SP[i].transform.position != SP[j].transform.position
                            && Vector3.Distance(SP[i].transform.position, SP[j].transform.position) < PCP_DISTANCE
                            && !linkedVisualisationDictionary.ContainsKey(_name) && !linkedVisualisationDictionary.ContainsKey(_nameReverse))
                        {
                            if (SP[i].viewType == Visualization.ViewType.Histogram) SP[i].ShowHistogram(false);
                            if (SP[j].viewType == Visualization.ViewType.Histogram) SP[j].ShowHistogram(false);

                            LinkVisualisations(_name, SP[i], SP[j]);
                        }
                    }
                }
            }

        // Dirty... this pass makes sure that all histograms stay hidden
        // if they belong to a SPLOM2D or 3D
        foreach (var item in SP)
        {
            if (item.viewType == Visualization.ViewType.Histogram && AxisInScatterplotMaxtrix(item.axes[0]))
                item.ShowHistogram(false);
        }

        //cleaning the visualisations that needs to be destroyed
        toDestroy.Clear();

        foreach (var item in linkedVisualisationDictionary.Values)
        {
            Visualization v1 = item.GetComponent<LinkedVisualisations>().V1;
            Visualization v2 = item.GetComponent<LinkedVisualisations>().V2;

            if (v1 == null || v2 == null || Vector3.Distance(v1.transform.position, v2.transform.position) > PCP_DISTANCE + 0.05f)
            {
                toDestroy.Add(item.name);
            }

        }

        //cleaning linked visualsiations
        foreach (var item in toDestroy)
        {
            GameObject lvv = linkedVisualisationDictionary[item];
            linkedVisualisationDictionary.Remove(lvv.name);

            Visualization v1 = lvv.GetComponent<LinkedVisualisations>().V1;
            Visualization v2 = lvv.GetComponent<LinkedVisualisations>().V2;

            foreach (var ax1 in v1.axes)
            {
                ax1.isDirty = true;
            }

            foreach (var ax2 in v2.axes)
            {
                ax2.isDirty = true;
            }

            if (v1 != null && v1.viewType == Visualization.ViewType.Histogram)
            {
                //only show if not used in other linked visualizations
                if (linkedVisualisationDictionary.Values.None(x => x.GetComponent<LinkedVisualisations>().Contains(v1.axes[0])))
                    v1.ShowHistogram(true);
            }

            if (v2.viewType == Visualization.ViewType.Histogram && v2 != null)
            {
                if (linkedVisualisationDictionary.Values.None(x => x.GetComponent<LinkedVisualisations>().Contains(v2.axes[0])))
                    v2.ShowHistogram(true);
            }

            Destroy(lvv);
        }

        //display all sp symbols on the debug panel
        string grammarSymbols = "SP1: " + SP.Count(x => x.viewType == Visualization.ViewType.Histogram) + "\n"
                                + "SP2: " + SP.Count(x => x.viewType == Visualization.ViewType.Scatterplot2D) + "\n"
                                + "SP3: " + SP.Count(x => x.viewType == Visualization.ViewType.Scatterplot3D) + "\n"
//                                + "SP2_SPLOM: " + SPLOMS2D.Count + "\n"
                                + "SP3_SPLOM: " + SPLOMS3D.Count + "\n";
    }

    void LinkVisualisations(string _name, Visualization v1, Visualization v2)
    {
        GameObject lvGO = new GameObject();
        lvGO.name = _name;
        LinkedVisualisations lv = lvGO.AddComponent<LinkedVisualisations>();

        var sc = lvGO.AddComponent<SphereCollider>();
        sc.radius = 0.06f;
        sc.isTrigger = true;

        lv.V1 = v1;
        lv.V2 = v2;
        linkedVisualisationDictionary.Add(_name, lvGO);
    }

    public List<Visualization> WalkLinkedVisualisations(LinkedVisualisations src)
    {
        var toProcess = new Stack<LinkedVisualisations>();
        toProcess.Push(src);

        var connectedVis = new List<Visualization>();

        var processed = new List<LinkedVisualisations>();

        while (toProcess.Count > 0)
        {
            var link = toProcess.Pop();
            processed.Add(link);

            connectedVis.Add(link.V1);
            connectedVis.Add(link.V2);

            foreach (var srcvis in connectedVis)
            {
                var connected = linkedVisualisationDictionary.Values.Select(x => x.GetComponent<LinkedVisualisations>())
                                                                    .Where(x => !processed.Contains(x))
                                                                    .Where(x => x.V1 == srcvis || x.V2 == srcvis);

                foreach (var v in connected)
                {
                    toProcess.Push(v);
                }
            }
        }
        return connectedVis.Distinct().ToList();
    }

    public System.Tuple<List<Visualization>, List<LinkedVisualisations>> GetChainedVisualisationsAndLinkedVisualisations(Visualization src)
    {
        var toProcess = new Stack<LinkedVisualisations>();

        var srcLinkedVisualisations = linkedVisualisationDictionary.Values.Select(x => x.GetComponent<LinkedVisualisations>())
                                                                    .Where(x => x.V1 == src || x.V2 == src);

        if (srcLinkedVisualisations.Count() > 0)
            toProcess.Push(srcLinkedVisualisations.First());

        var connectedVis = new List<Visualization>();
        connectedVis.Add(src);
        var connectedLinkedVis = new List<LinkedVisualisations>();
        var processed = new List<LinkedVisualisations>();

        while (toProcess.Count > 0)
        {
            var link = toProcess.Pop();
            processed.Add(link);

            if (!connectedVis.Contains(link.V1)) connectedVis.Add(link.V1);
            if (!connectedVis.Contains(link.V2)) connectedVis.Add(link.V2);

            foreach (var srcvis in connectedVis)
            {
                var connected = linkedVisualisationDictionary.Values.Select(x => x.GetComponent<LinkedVisualisations>())
                                                                    .Where(x => !processed.Contains(x))
                                                                    .Where(x => x.V1 == srcvis || x.V2 == srcvis);

                foreach (var v in connected)
                {
                    toProcess.Push(v);

                    if (!connectedLinkedVis.Contains(v))
                    {
                        connectedLinkedVis.Add(v);
                    }
                }
            }
        }
        return new System.Tuple<List<Visualization>, List<LinkedVisualisations>>(connectedVis, connectedLinkedVis);
    }

    public System.Tuple<List<Visualization>, List<LinkedVisualisations>> GetChainedVisualisationsAndLinkedVisualisations(LinkedVisualisations src)
    {
        var toProcess = new Stack<LinkedVisualisations>();
        toProcess.Push(src);

        var connectedVis = new List<Visualization>();
        var connectedLinkedVis = new List<LinkedVisualisations>();
        var processed = new List<LinkedVisualisations>();

        while (toProcess.Count > 0)
        {
            var link = toProcess.Pop();
            processed.Add(link);

            if (!connectedVis.Contains(link.V1)) connectedVis.Add(link.V1);
            if (!connectedVis.Contains(link.V2)) connectedVis.Add(link.V2);

            foreach (var srcvis in connectedVis)
            {
                var connected = linkedVisualisationDictionary.Values.Select(x => x.GetComponent<LinkedVisualisations>())
                                                                    .Where(x => !processed.Contains(x))
                                                                    .Where(x => x.V1 == srcvis || x.V2 == srcvis);

                foreach (var v in connected)
                {
                    toProcess.Push(v);

                    if (!connectedLinkedVis.Contains(v))
                    {
                        connectedLinkedVis.Add(v);
                    }
                }
            }
        }
        return new System.Tuple<List<Visualization>, List<LinkedVisualisations>>(connectedVis, connectedLinkedVis);
    }

    List<string> toDestroy = new List<string>();

    public Dictionary<string, GameObject> linkedVisualisationDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> SPLOMDictionnaty = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> SPLOMDictionnaty2 = new Dictionary<string, GameObject>();

    //List<SPLOM2D> SPLOMS2D = new List<SPLOM2D>();
    List<SPLOM3D> SPLOMS3D = new List<SPLOM3D>();

    bool AxisInScatterplotMaxtrix(Axis axis)
    {
        bool isInAScatterplotMatrix = false;

        foreach (var item in SPLOMS3D)
        {
            isInAScatterplotMatrix |= item.XAxes1.Contains(axis) || item.YAxes1.Contains(axis) || item.ZAxes1.Contains(axis);
        }

        return isInAScatterplotMatrix;
    }

    void Update()
    {
        ParseScene_V2();
    }

    public bool scatterplot2DContainsAxis(GameObject[] scatterplots2D, Axis axis)
    {
        for (int i = 0; i < scatterplots2D.Length; i++)
        {
            if (scatterplots2D[i].GetComponent<Visualization>().axes.Contains(axis))
                return true;
        }
        return false;
    }

    void connectedAxis(Axis a, List<Axis> A, ref List<Axis> theConnectedAxes)
    {
        //List<Axis> theConnectedAxes = new List<Axis>();

        foreach (var b in A)
        {
            if (Vector3.Distance(a.MaxPosition, b.MinPosition) < SP_DISTANCE / 4f && a.IsColinear(b))
            {
                theConnectedAxes.Add(b);
                connectedAxis(b, A, ref theConnectedAxes);
            }
        }
    }

}
