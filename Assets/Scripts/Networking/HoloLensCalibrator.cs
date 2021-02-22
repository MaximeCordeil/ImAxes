using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloLensCalibrator : MonoBehaviourPun
{
    public bool IsCalibrated { get; private set; }
    public Transform Root { get; private set; }

    static HoloLensCalibrator _instance;
    public static HoloLensCalibrator Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<HoloLensCalibrator>()); }
    }

    public void CalibrateHoloLens1()
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        { 
            photonView.RPC("CalibrateHoloLensRPC", NetworkLauncher.Instance.Hololens1Player);
            Debug.Log("HoloLens 1 position calibrated.");
        }
        else
        {
            Debug.LogError("Could not calibrate HoloLens 1 as it is not connected to the server.");
        }   
    }

    public void CalibrateHoloLens2()
    {
        if (NetworkLauncher.Instance.Hololens2Player != null)
        {
            photonView.RPC("CalibrateHoloLensRPC", NetworkLauncher.Instance.Hololens2Player);
            Debug.Log("HoloLens 2 position calibrated.");
        }
        else
        {
            Debug.LogError("Could not calibrate HoloLens 2 as it is not connected to the server.");
        }
    }

    [PunRPC]
    private void CalibrateHoloLensRPC()
    {
        if (Root != null)
            Destroy(Root.gameObject);

        Root = new GameObject("Root").transform;
        Root.position = Camera.main.transform.position;
        Root.rotation = Camera.main.transform.rotation;
        IsCalibrated = true;
    }

    public void ShiftRootPositionX_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionXRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }
    public void ShiftRootPositionX_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionXRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootPositionXRPC(float amount)
    {
        if (Root != null)
            Root.Translate(Vector3.right * amount);
    }

    public void ShiftRootPositionY_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionYRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }

    public void ShiftRootPositionY_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionYRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootPositionYRPC(float amount)
    {
        if (Root != null)
            Root.Translate(Vector3.up * amount);
    }

    public void ShiftRootPositionZ_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionZRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }

    public void ShiftRootPositionZ_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootPositionZRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootPositionZRPC(float amount)
    {
        if (Root != null)
            Root.Translate(Vector3.forward * amount);
    }

    public void ShiftRootRotationX_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationXRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }
    public void ShiftRootRotationX_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationXRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootRotationXRPC(float amount)
    {
        if (Root != null)
            Root.Rotate(Vector3.right * amount);
    }

    public void ShiftRootRotationY_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationYRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }

    public void ShiftRootRotationY_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationYRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootRotationYRPC(float amount)
    {
        if (Root != null)
            Root.Rotate(Vector3.up * amount);
    }

    public void ShiftRootRotationZ_HL1(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationZRPC", NetworkLauncher.Instance.Hololens1Player, amount);
        }
    }

    public void ShiftRootRotationZ_HL2(float amount)
    {
        if (NetworkLauncher.Instance.Hololens1Player != null)
        {
            photonView.RPC("ShiftRootRotationZRPC", NetworkLauncher.Instance.Hololens2Player, amount);
        }
    }

    [PunRPC]
    private void ShiftRootRotationZRPC(float amount)
    {
        if (Root != null)
            Root.Rotate(Vector3.forward * amount);
    }
}
