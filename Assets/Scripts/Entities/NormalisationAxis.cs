using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalisationAxis : MonoBehaviour {

    public GameObject end;
    public GameObject sliderOne;
    public GameObject sliderTwo;
    public Color cylActive;
    public Color cylInactive;
    public Color activeEmission;
    public Color inactiveEmission;
    MeshRenderer cylRend;
    bool set;
    Axis myAxis;
    public Transform cylinder;
    float multiplier = -10f;
	// Use this for initialization
	void Start () {
        myAxis = GetComponentInParent<Axis>();
        cylRend = cylinder.transform.gameObject.GetComponentInChildren<MeshRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
        var lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, new Vector3(0, 0, -myAxis.MaxNormaliser));
        lr.SetPosition(1, new Vector3(0, 0, -myAxis.MinNormaliser));
       
        if (myAxis ==Axis.CurrentAxis && !set)
        {
            set = true;
            sliderOne.SetActive(true);
            sliderTwo.SetActive(true);
            cylRend.material.SetColor("_Color", cylActive);
            cylRend.material.EnableKeyword("_EMISSION");
            cylRend.material.SetColor("_EmissionColor", activeEmission);
        }
        if (myAxis == Axis.CurrentAxis)
        {
            cylinder.localPosition = new Vector3(0, (myAxis.MinNormaliser), 0);
            sliderOne.transform.localPosition = new Vector3(0, (myAxis.MinNormaliser), 0);
            sliderTwo.transform.localPosition = new Vector3(0, (myAxis.MaxNormaliser), 0);
            float scaleCalc = (myAxis.MinNormaliser - myAxis.MaxNormaliser) * multiplier;
            cylinder.localScale = new Vector3(cylinder.localScale.x, cylinder.localScale.y, scaleCalc);
        }
        if (myAxis != Axis.CurrentAxis && set)
        {
            set = false;
            sliderOne.SetActive(false);
            sliderTwo.SetActive(false);
            cylRend.material.SetColor("_Color", cylInactive);
            cylRend.material.DisableKeyword("_EMISSION");
        }


    }
}
