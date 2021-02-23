using System.Collections;
using System.Collections.Generic;
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

    private bool isFlipped = false;
    private bool prevFlipped = false;

    private void Update()
    {
        if (Camera.main != null)
        {
            float dot = Vector3.Dot(Camera.main.transform.forward, ParentAxis.forward);
            isFlipped = (dot < 0);

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
