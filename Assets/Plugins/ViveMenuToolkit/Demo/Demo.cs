using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wacki;

public class Demo : MonoBehaviour {

    public ViveUILaserPointer RightPointer;
    public ViveUILaserPointer LeftPointer;
    public Toggle DisableLaserToggle;
    public ViveMenu CompactMenu;
    public ViveMenu OriginalMenu;
    public MenuType Type = MenuType.CompactMenu;
    public Slider SpinSpeedSlider;
    public Slider RedSlider;
    public Slider GreenSlider;
    public Slider BlueSlider;
    public Dropdown BasicColour;
    private Vector3 originalControllerMenuScale;
    private ViveMenu FullMenu;
    private bool Spin = true;
    private float Speed = 20f;
    private float red = 255;
    private float green = 0;
    private float blue = 0;

    public enum MenuType
    {
        FullMenu,
        CompactMenu
    }

    private bool _showControllerMenu = true;

    // Use this for initialization
    void Start()
    {
        // Copy the main menu for the demo.
        FullMenu = Instantiate(OriginalMenu);
        FullMenu.FollowCamera = false;
        FullMenu.transform.position = CompactMenu.transform.position;
        FullMenu.transform.localScale = CompactMenu.transform.localScale;
        originalControllerMenuScale = CompactMenu.transform.localScale;
        FullMenu.transform.localRotation = CompactMenu.transform.localRotation;
        FullMenu.transform.SetParent(LeftPointer.gameObject.transform);
        

        this.CompactMenu.Show(false);
        this.FullMenu.Show(_showControllerMenu);


        //BasicColour.onValueChanged.AddListener((value) =>
        //{
        //    Color color = new Color();
        //    switch (value)
        //    {
        //        case 0:
        //            color = Color.blue;
        //            break;
        //        case 1:
        //            color = Color.green;
        //            break;
        //        case 2:
        //            color = Color.red;
        //            break;
        //        default:
        //            break;
        //    }

        //    red = (int)color.r * 255;
        //    green = (int)color.g * 255;
        //    blue = (int)color.b * 255;

        //    UpdateColour();
        //});

        //GreenSlider.onValueChanged.AddListener((value) =>
        //{
        //    green = value;
        //    UpdateColour();
        //});

        //RedSlider.onValueChanged.AddListener((value) =>
        //{
        //    red = value;
        //    UpdateColour();
        //});

        //BlueSlider.onValueChanged.AddListener((value) =>
        //{
        //    blue = value;
        //    UpdateColour();
        //});

        //UpdateColour();
    }

    public void ScaleControllerMenu(float value)
    {
        var newVector = new Vector3(originalControllerMenuScale.x * value, originalControllerMenuScale.y * value, originalControllerMenuScale.z * value);
        this.FullMenu.transform.localScale = newVector;
        this.CompactMenu.transform.localScale = newVector;
    }

    public void Exit()
    {
        this.Type = MenuType.CompactMenu;
        UpdateControllerMenu();
        this.OriginalMenu.Show(false);
    }

    public void ToggleOriginalMenu(bool show)
    {
        this.OriginalMenu.Show(show);
    }

    public void SetPointer(bool value) {
        RightPointer.SetLaserAlwaysOn(value);
    }

    public void ShowControllerMenu(bool show)
    {
        _showControllerMenu = show;
        UpdateControllerMenu();
    }

    private void UpdateControllerMenu()
    {
        if (Type == MenuType.CompactMenu)
        {
            CompactMenu.Show(_showControllerMenu);
            FullMenu.Show(false);
        } else
        {
            CompactMenu.Show(false);
            FullMenu.Show(_showControllerMenu);
        }
    }

    public void ToggleLeftPointerEnabled(bool value)
    {
        LeftPointer.DisableLaser = value;
    }

    public void SwitchControllerMenuType(int val)
    {
        switch (val)
        {
            case 0:
                this.Type = MenuType.CompactMenu;
                break;
            case 1:
                this.Type = MenuType.FullMenu;
                break;
            default:
                break;
        }
        UpdateControllerMenu();
    }

    public void UpdateSpinSpeed(float value)
    {
        Speed = value;
    }

    public void SetSpin(bool value)
    {
        this.Spin = value;
    }

    private void UpdateColour()
    {
        // Change color;
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(red / 255, green / 255, blue / 255);

        //RedSlider.value = red;
        //BlueSlider.value = blue;
        //GreenSlider.value = green;
    }


    // Update is called once per frame
    void Update () {
        if (Spin)
        {
            transform.Rotate(0, Speed * Time.deltaTime, 0);
        }
    }
}
