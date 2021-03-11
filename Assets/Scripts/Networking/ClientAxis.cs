using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAxis : MonoBehaviourPun, IPunObservable
{
    private Axis axis;
    private Axis lastAxis;
    private GameObject createdAxis;

    private int currentDimensionIdx = -1;

    private int prevSliderOne;
    private int prevSliderTwo;
    private int prevRotary;

    private Transform mainCamera;

    public GameObject axisPrefab;
    public SceneManager sceneManager;
    private Transform root;

    private void Start()
    {
        mainCamera = Camera.main.transform;

#if UNITY_EDITOR
        root = ViconOriginSceneCalibrator.Instance.Root;
#else
        root = QRCodeSceneCalibrator.Instance.Root;
#endif
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // We read values from the stream in this script, which were sent by the ServerAxis.cs script
        if (!stream.IsWriting)
        {
            // Data for axis transform
            UpdateClientTransform((Vector3)stream.ReceiveNext(),
                                  (Quaternion)stream.ReceiveNext()
                                  );

            // Data for axis properties
            UpdateClientAxis((int)stream.ReceiveNext(),
                             (float)stream.ReceiveNext(),
                             (float)stream.ReceiveNext(),
                             (float)stream.ReceiveNext()
                             );

            // Data for axis booleans
            ToggleInfoboxMode((bool)stream.ReceiveNext());

        }
    }

    public void UpdateClientTransform(Vector3 newPos, Quaternion newRot)
    {
        Transform tmp = transform.parent;
        transform.parent = root;

        transform.localPosition = newPos;
        transform.localRotation = newRot;

        transform.parent = tmp;
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
        // Always use the minimum value for minfilter and vice versa
        float newMinFilter = Mathf.Min(minFilter, maxFilter);
        float newMaxFilter = Mathf.Max(minFilter, maxFilter);
        if (axis.MinFilter != newMinFilter)
        {
            axis.SetMinFilter(newMinFilter);
        }
        if (axis.MaxFilter != newMaxFilter)
        {
            axis.SetMaxFilter(newMaxFilter);
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

    [PunRPC]
    public void ResetAxisObject()
    {
        DestroyAxisObject(axis);
    }

    private void CreateAxisObject(int idx)
    {
        createdAxis = Instantiate(axisPrefab) as GameObject;
        axis = createdAxis.GetComponent<Axis>();
        axis.Init(sceneManager.dataObject, idx, false);
        axis.tag = "Axis";
        // axis.HideHandles();

        sceneManager.AddAxis(axis);

        createdAxis.transform.SetParent(transform);
        createdAxis.transform.localPosition = new Vector3(0, 0, 0.03f);
        createdAxis.transform.localEulerAngles = new Vector3(0, 270, 0);
    }

    private void DestroyAxisObject(Axis axis)
    {
        if (axis != null)
        {
            sceneManager.DestroyAxis(axis);
            axis = null;
        }
    }
}
