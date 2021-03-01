using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine;

public class FaceTextToCamera : MonoBehaviour
{
    public Transform ParentAxis;
    public bool IsTextTickLabel = false;
    public TextMeshPro Text;
    public bool FlipAxisX = false;
    public bool FlipAxisY = false;
    public bool FlipAxisZ = false;
    public bool ReverseFlip = false;

    private bool isFlipped = false;
    private bool prevFlipped = false;

    private void Update()
    {
        if (CameraCache.Main != null)
        {
            float dot = Vector3.Dot(CameraCache.Main.transform.forward, ParentAxis.forward);
            isFlipped = (dot < 0);
            if (ReverseFlip) isFlipped = !isFlipped;

            if (isFlipped != prevFlipped)
            {
                if (isFlipped)
                {
                    var rotation = transform.localEulerAngles;
                    if (FlipAxisX) rotation.x += 180;
                    if (FlipAxisY) rotation.y += 180;
                    if (FlipAxisZ) rotation.z += 180;
                    transform.localEulerAngles = rotation;

                    if (IsTextTickLabel) Text.alignment = TextAlignmentOptions.MidlineLeft;
                }
                else
                {
                    var rotation = transform.localEulerAngles;
                    if (FlipAxisX) rotation.x -= 180;
                    if (FlipAxisY) rotation.y -= 180;
                    if (FlipAxisZ) rotation.z -= 180;
                    transform.localEulerAngles = rotation;

                    if (IsTextTickLabel) Text.alignment = TextAlignmentOptions.MidlineRight;
                }

                prevFlipped = isFlipped;
            }
        }
    }
}
