using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAxis : MonoBehaviourPun, IPunObservable
{
    private Axis axis;
    private Axis lastAxis;
    private GameObject createdAxis;

    private int currentDimensionIdx = 0;

    private int prevSliderOne;
    private int prevSliderTwo;
    private int prevRotary;

    private Transform mainCamera;

    // Filtering for smoothing
    private OneEuroFilter<Vector3> positionFilter;
    private OneEuroFilter<Quaternion> rotationFilter;
    private OneEuroFilter<Vector3> scaleFilter;

    private void Start()
    {
        mainCamera = Camera.main.transform;

        positionFilter = new OneEuroFilter<Vector3>(4);
        rotationFilter = new OneEuroFilter<Quaternion>(4);
        scaleFilter = new OneEuroFilter<Vector3>(4);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // We read values from the stream in this script, which were sent by the ServerAxis.cs script
        if (!stream.IsWriting)
        {
            // Data for axis transform
            UpdateClientTransform((Vector3)stream.ReceiveNext(),
                                  (Quaternion)stream.ReceiveNext(),
                                  (Vector3)stream.ReceiveNext()
                                  );

            // Datsa for axis booleans
            ToggleInfoboxMode((bool)stream.ReceiveNext());

            // Data for axis properties
            UpdateClientAxis((int)stream.ReceiveNext(),
                             (float)stream.ReceiveNext(),
                             (float)stream.ReceiveNext(),
                             (float)stream.ReceiveNext()
                             );
        }
    }

    public void UpdateClientTransform(Vector3 newPos, Quaternion newRot, Vector3 newScale)
    {
        if (SceneCalibrator.Instance.Root != null)
        {
            Transform tmp = transform.parent;
            transform.parent = SceneCalibrator.Instance.Root;

            // Filter the positions to smooth it a bit
            newPos = positionFilter.Filter(newPos);
            newRot = rotationFilter.Filter(newRot);
            newScale = scaleFilter.Filter(newScale);

            transform.localPosition = newPos;
            transform.localRotation = newRot;
            transform.localScale = newScale;

            transform.parent = tmp;
        }
    }

    public void UpdateClientAxis(int dimensionIdx, float minFilter, float maxFilter, float infoboxPosition)
    {
        if (axis == null)
        {
            CreateAxisObject(dimensionIdx);
            currentDimensionIdx = dimensionIdx;
        }

        // Rotary settings
        if (dimensionIdx != currentDimensionIdx)
        {
            DestroyAxisObject(axis);
            CreateAxisObject(dimensionIdx);

            currentDimensionIdx = dimensionIdx;
        }

        // Slider settings
        if (axis.MinFilter != minFilter)
        {
            axis.SetMinFilter(minFilter);
        }
        if (axis.MaxFilter != maxFilter)
        {
            axis.SetMaxFilter(maxFilter);
        }
        if (axis.InfoboxPosition != infoboxPosition)
        {
            axis.SetInfoboxPosition(infoboxPosition);
        }
    }

    public void ToggleInfoboxMode(bool toggle)
    {
        if (axis.IsInfoboxEnabled != toggle)
        {
            axis.ToggleInfobox(toggle);
        }
    }

    private void CreateAxisObject(int idx)
    {
        createdAxis = (GameObject) Instantiate(Resources.Load("Axis2"));
        axis = createdAxis.GetComponent<Axis>();
        axis.Init(SceneManager.Instance.dataObject, idx, false);
        axis.tag = "Axis";
        axis.isClone = true;
        // axis.HideHandles();

        SceneManager.Instance.AddAxis(axis);

        createdAxis.transform.SetParent(transform);
        createdAxis.transform.localPosition = new Vector3(0.024f, 0.134f, 0.04f);
        createdAxis.transform.localEulerAngles = new Vector3(0, 270, 0);
    }

    private void DestroyAxisObject(Axis axis)
    {
        SceneManager.Instance.DestroyAxis(axis);
    }
}
