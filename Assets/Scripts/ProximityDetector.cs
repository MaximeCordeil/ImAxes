using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProximityDetector : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnProximityEntered;

    [SerializeField]
    UnityEvent OnProximityExited;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            OnProximityEntered.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            OnProximityExited.Invoke();
        }
    }

    public void ForceExit()
    {
        OnProximityExited.Invoke();
    }
}

