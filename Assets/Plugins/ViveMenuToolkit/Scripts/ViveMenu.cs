using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wacki;

public class ViveMenu : MonoBehaviour
{

    bool dropdownselected = false;
    public enum ViveMenuControlMode
    {
        Laser,
        Controller
    }

    // Public editor fields.
    [Header("Default Menu (Optional)")]
    [Tooltip("Drag and drop the menu you want to show on initialise.")]
    public Canvas DefaultMenu;
    [Tooltip("Determines whether the UI is controlled by a laser or by controller input.")]
    public ViveMenuControlMode ControlMode;
    public GameObject Camera;
    public bool FollowCamera; // TODO this only applies while drag/dropping the menu.
    public bool IgnoreLaser;
    public bool HideOnStart;
    [Tooltip("Prevents the laser showing with the controller is pointed at the menu. Only applies when the controller is set to !LaserAlwaysOn.")]
    // TODO only show in controller mode.
    public SteamVR_TrackedController Controller;

    // Private fields
    private List<Canvas> Menus;
    private Canvas currentMenu;
    private ViveMenuControllerEvents ViveMenuEvents = new ViveMenuControllerEvents();

    private Dropdown _ddCurrent = null;

    // Initialise.
    private void Start()
    {
        // Hide all menus except the default one. 
        this.Menus = this.gameObject.GetComponentsInChildren<Canvas>(includeInactive: true).ToList();
        currentMenu = this.Menus.SingleOrDefault(x => x == this.DefaultMenu);

        if (currentMenu == null) // Just use the first available canvas, preferably a visible one.
        {
            this.currentMenu = this.Menus.Where(x => x.gameObject.activeSelf).Count() > 0 ? this.Menus.First(x => x.gameObject.activeSelf) : this.Menus.FirstOrDefault();
        }

        // Set visible only the default menu.
        this.Menus.ForEach(x => { x.gameObject.SetActive(false); });
        this.SetCurrentMenu(currentMenu);

        // Initialise boxcollider on each menu, used to detect that the controller is pointed at a menu.
        if (!IgnoreLaser)
        {
            this.Menus.ForEach(menu =>
            {
                // Add collider the size of the menu canvas.
                var collider = menu.gameObject.AddComponent<BoxCollider>();
                var menuTransform = menu.gameObject.transform;
                var menuRect = menu.GetComponent<RectTransform>();
                collider.size = new Vector3(menuRect.sizeDelta.x, menuRect.sizeDelta.y, .1f);

                // Add Menu tag (required for laser to distinguish menus.)
                menu.tag = "Menu";
            });
        }
        else
        {
            this.gameObject.AddComponent<UIIgnoreRaycast>(); // Tells the LaserInputModule to ignore UI elements on this menu.
        }

        // Get controller 
        if (Controller == null)
        {
            this.Controller = GetComponentInParent<SteamVR_TrackedController>(); // Attempt to find a controller in parent objects.
        }
        if (Controller != null) // We have a controller, we can set up the events for it.
        {
            ViveMenuEvents.Active = true;
            var controllerEventSystem = Controller.GetComponent<ControllerEventSystem>();
            if (controllerEventSystem == null)
            {
                controllerEventSystem = Controller.gameObject.AddComponent<ControllerEventSystem>();
            }
            controllerEventSystem.AddViveMenuEvents(ViveMenuEvents);

            // Add controller events that we want to apply to all menus. (only works while the menu is active).
            ViveMenuEvents.MenuButtonClicked.AddListener(() =>
            {
                this.Show(!gameObject.activeSelf);
            });
        }

        if (this.ControlMode == ViveMenuControlMode.Laser)
        {

        }
        else // Controller mode.
        {
            if (Controller == null)
            {
                Debug.Log("Warning: No controller specified for a menu in controller mode.");
            }
            else
            {
                // Add listeners specifically relevant to controlling the menu with the controller.

                ViveMenuEvents.TriggerClicked.AddListener(() =>
                {
                    if (_currentSelectable is Dropdown)
                    {
                        if (_ddCurrent == null) // No dropdown selected.
                        {
                            this._ddCurrent = ((Dropdown)_currentSelectable);
                            _ddCurrent.Show();
                        }
                    }
                    else
                    {
                        _currentSelectable.ViveSelectableInvoke();
                    }
                    var test = _currentSelectable;
                });



                ViveMenuEvents.PadClickUp.AddListener(() =>
                {


                    SelectNextSelectable(Direction.Up);
                });

                ViveMenuEvents.PadClickDown.AddListener(() =>
                {
                    SelectNextSelectable(Direction.Down);
                });

                ViveMenuEvents.PadClickRight.AddListener(() =>
                {

                    if (_currentSelectable is Slider)
                    {


                        //var currentvalue = ((Slider)_currentSelectable).value;
                        //var minvalue = ((Slider)_currentSelectable).minValue;
                        //var maxvalue = ((Slider)_currentSelectable).maxValue;
                        //var calculatedvalue = (maxvalue - minvalue) / 10;

                        //((Slider)_currentSelectable).value = currentvalue + calculatedvalue;
                    }

                });

                ViveMenuEvents.PadClickLeft.AddListener(() =>
                {

                    if (_currentSelectable is Slider)
                    {
                        //var currentvalue = ((Slider)_currentSelectable).value;
                        //var minvalue = ((Slider)_currentSelectable).minValue;
                        //var maxvalue = ((Slider)_currentSelectable).maxValue;
                        //var calculatedvalue = (maxvalue - minvalue) / 10;

                        //((Slider)_currentSelectable).value = currentvalue - calculatedvalue;
                    }
                });


            }
        }

        if (HideOnStart)
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = Vector3.zero;
        }
    }

    bool isHiding = false;
    public void Show(bool show)
    {
        if ((!gameObject.activeInHierarchy || isHiding ) && show)
        {
            isHiding = false;
            gameObject.transform.DOKill(true);
            gameObject.SetActive(show);

            gameObject.transform.DOScale(new Vector3(0.11f, 0.105f, 0.05999999f), 0.4f).SetEase(Ease.OutBack);            
        }
        else if (gameObject.activeInHierarchy && !show)
        {
            isHiding = true;
            gameObject.transform.DOKill(true);
            gameObject.transform.DOScale(0, 0.4f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
        //gameObject.SetActive(show);
        //ViveMenuEvents.Active = show;
    }

    void Update()
    {
        if (FollowCamera)
        { // Code for the menu to follow the camera.	
            Vector3 v = Camera.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(Camera.transform.position - v);
            transform.Rotate(0, 180, 0);
        }
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void ToggleFollowCamera(bool b)
    {
        FollowCamera = b;
    }

    public void SetCurrentMenu(Canvas newCanvas)
    {
        this.currentMenu.gameObject.SetActive(false);
        this.currentMenu = newCanvas;
        this.currentMenu.gameObject.SetActive(true);

        _currentSelectables = currentMenu.GetComponentsInChildren<Selectable>();

        var first = _currentSelectables.OrderByDescending(selectable => selectable.transform.position.y).FirstOrDefault();
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public Selectable _currentSelectable
    {
        get
        {
            var selectedGameobject = _es.currentSelectedGameObject;
            if (selectedGameobject != null) { return selectedGameobject.GetComponent<Selectable>(); }
            return null;
        }
        set { }
    }

    private Selectable[] _currentSelectables;

    private EventSystem _eventSystem;
    public EventSystem _es
    {
        get
        {
            if (_eventSystem == null)
            {
                var es = FindObjectOfType<EventSystem>();
                if (es == null) { throw new System.Exception("No EventSystem found in the scene. Check that the VRInputModule on the CameraRig is enabled."); }
                _eventSystem = es;
            }
            return _eventSystem;
        }
        set { }
    }

    // Methods 
    public void SelectNextSelectable(Direction direction)
    {
        // Get a direction to find the next button.
        var query = _currentSelectables.Where(x => x.transform.position.y < _currentSelectable.transform.position.y).OrderByDescending(x => x.transform.position.y);
        switch (direction)
        {
            case (Direction.Down):
                query = _currentSelectables.Where(x => x.transform.position.y < _currentSelectable.transform.position.y).OrderByDescending(x => x.transform.position.y);
                break;
            case (Direction.Up):
                query = _currentSelectables.Where(x => x.transform.position.y > _currentSelectable.transform.position.y).OrderBy(x => x.transform.position.y);
                break;
            case (Direction.Left):
                query = _currentSelectables.Where(x => x.transform.position.x < _currentSelectable.transform.position.x).OrderByDescending(x => x.transform.position.x);
                break;
            case (Direction.Right):
                query = _currentSelectables.Where(x => x.transform.position.x > _currentSelectable.transform.position.x).OrderBy(x => x.transform.position.x);
                break;
            default:
                throw new System.Exception();
        }

        Selectable next = null; // The next selectable to be selected.
        if (_currentSelectable != null)
        {
            // Get 
            next = query.FirstOrDefault();
        }
        if (next == null) //  Then there are no more selectables in that direction.
        {
            next = _currentSelectables.OrderByDescending(x => x.transform.position.y).FirstOrDefault();
        }
        if (next != null)
        {
            _es.SetSelectedGameObject(next.gameObject);

        }
        // If next is null there are no selectables in the canvas, do nothing.
    }
}

public static class SelectableExtensions
{
    public static void ViveSelectableInvoke(this Selectable selectable)
    {

        if (selectable is Button)
        {
            ((Button)selectable).onClick.Invoke();
        }

        if (selectable is Toggle)
        {
            bool changetoggle = ((Toggle)selectable).isOn;
            ((Toggle)selectable).isOn = !changetoggle;
        }
    }
}


