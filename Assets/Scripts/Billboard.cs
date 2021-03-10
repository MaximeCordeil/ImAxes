using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool LockX = false;
    public bool LockY = false;
    public bool LockZ = false;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
              Camera.main.transform.rotation * Vector3.up);
        Vector3 eulerAngles = transform.localEulerAngles;
        if (LockX)
            eulerAngles.x = 0;
        if (LockY)
            eulerAngles.y = 0;
        if (LockZ)
            eulerAngles.z = 0;
        transform.localEulerAngles = eulerAngles;
    }
}
