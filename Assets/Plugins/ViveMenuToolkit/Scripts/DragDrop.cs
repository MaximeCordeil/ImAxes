using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Wacki;
using System.Linq;
public class DragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _offset;
    private ViveMenu viveMenu; // The parent viveMenu of what this is attached to.
    private ViveUILaserPointer pointer; // Stores a vive laser pointer on button down press.

    public void Start()
    {
        var viveMenus = gameObject.GetComponentsInParent<ViveMenu>();
        if (viveMenus.Count() > 1) { Debug.Log("Warning: There should not be more than one ViveMenu as a parent of a canvas"); }
        this.viveMenu = viveMenus.Single();
    }

    bool _pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        // Offset is the difference between the laser hitPoint and the centre of the menu.
        var laserEventData = eventData as LaserPointerEventData;
        var laserPointer = laserEventData.controller as ViveUILaserPointer;
        this.pointer = laserPointer;
        _offset = this.pointer.GetHitPoint().position - viveMenu.transform.position;

        // Lock the laser distance when the move button is clicked.
        this.pointer.Lock(true);
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Unlock on pointer released.
        pointer.Lock(false);
        _pressed = false;
    }

    void Update()
    {
        if (!_pressed)
            return;

        var hitPointPosition = pointer.GetHitPoint().position;

        // Moves the ViveMenu canvas, so that all the submenus follow.
        viveMenu.transform.position = new Vector3(hitPointPosition.x - _offset.x, hitPointPosition.y - _offset.y, hitPointPosition.z - _offset.z);
    }
}
