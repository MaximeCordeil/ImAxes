using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAxis : MonoBehaviourPun
{
    private Axis axis;
    private Axis lastAxis;
    private GameObject createdAxis;

    private int currentDimensionIdx = 0;

    private int prevSliderOne;
    private int prevSliderTwo;
    private int prevRotary;

    [PunRPC]
    public void UpdateClientAxis(int dimensionIdx, float minNormaliser, float maxNormaliser)
    {
        // Create the axis if it does not yet exist
        if (axis == null)
        {
            CreateAxisObject(dimensionIdx);
            currentDimensionIdx = dimensionIdx;
        }

        // Slider settings
        if (axis.MaxNormaliser != maxNormaliser)
        {
            axis.SetMaxNormalizer(maxNormaliser);
        }
        if (axis.MinNormaliser != minNormaliser)
        {
            axis.SetMinNormalizer(minNormaliser);
        }

        // Rotary settings
        if (dimensionIdx != currentDimensionIdx)
        {
            DestroyAxisObject(axis);
            CreateAxisObject(dimensionIdx);

            currentDimensionIdx = dimensionIdx;
        }
    }

    [PunRPC]
    public void UpdateClientTransform(Vector3 newPos, Quaternion newRot) // TODO: change this to interpolate values for smoothing
    {
        transform.position = newPos;
        transform.rotation = newRot;
    }

    private void CreateAxisObject(int idx)
    {
        createdAxis = Instantiate(Resources.Load("Axis Variant 1")) as GameObject;
        axis = createdAxis.GetComponent<Axis>();
        axis.Init(SceneManager.Instance.dataObject, idx, false);
        axis.tag = "Axis";
        axis.isClone = true;
        axis.HideHandles();

        axis.transform.SetParent(transform);
        axis.transform.localPosition = Vector3.zero;
        axis.transform.localRotation = Quaternion.identity;

        SceneManager.Instance.AddAxis(axis);
    }

    private void DestroyAxisObject(Axis axis)
    {
        SceneManager.Instance.DestroyAxis(axis);
    }

}
