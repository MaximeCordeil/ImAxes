using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationButton : MonoBehaviour
{

    public Canvas TargetCanvas;

    void Start()
    {
        if (TargetCanvas == null)
        {
            Debug.Log("Warning: Navigation script has no target menu.");
        }
        else
        {
            this.gameObject.GetComponent<Button>().onClick.AddListener(GoToTarget);
        }
    }

    void GoToTarget()
    {
        // Find the first parent viveMenu.
        var viveMenu = GetComponentInParent<ViveMenu>();
        viveMenu.SetCurrentMenu(TargetCanvas);
    }
}
