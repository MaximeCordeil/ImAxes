using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class AxisRangeWidget : MonoBehaviour {

    [SerializeField]
    float axisOffset = 2.0f;

    [SerializeField]
    UnityEvent OnEntered;

    [SerializeField]
    UnityEvent OnExited;

    Axis parentAxis;

    public Vector3 initialScale;
    Vector3 rescaled = Vector3.one;

    [Serializable]
    public class OnValueChanged : UnityEvent<float> { };
    public OnValueChanged onValueChanged;


    // Use this for initialization
    void Start () {
        parentAxis = GetComponentInParent<Axis>();
        //initialScale = transform.localScale;
        rescaled = initialScale;
        rescaled.x *= 2f;
        rescaled.z *= 2f;
    }

    public int GetPriority()
    {
        return 10;
    }

    public void ProximityEnter()
    {
        transform.DOKill(true);
        transform.DOLocalMoveX(-axisOffset, 0.35f).SetEase(Ease.OutBack);
      //  transform.DOScale(rescaled, 0.35f).SetEase(Ease.OutBack);  killed scale for wireless axisuse
    }

    public void ProximityExit()
    {
        transform.DOKill(true);
        transform.DOLocalMoveX(0, 0.25f);
       // transform.DOScale(initialScale, 0.25f);
    }
}
