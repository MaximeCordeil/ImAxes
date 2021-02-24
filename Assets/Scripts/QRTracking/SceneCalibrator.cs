using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCalibrator : MonoBehaviour
{
    protected static SceneCalibrator _instance;
    public static SceneCalibrator Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Transform Root;
}
