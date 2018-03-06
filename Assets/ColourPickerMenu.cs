using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ColourPickerMenu : MonoBehaviour {

    public UnityEngine.UI.Slider redSlider;

    public UnityEngine.UI.Slider greenSlider;

    public UnityEngine.UI.Slider blueSlider;

    public UnityEngine.UI.Image colourPanel;

    public delegate void ColourChanged(Color color);
    public ColourChanged OnColourChanged;

    public delegate void HidePicker();
    public HidePicker OnHidePicker;

    public delegate void ShowPicker();
    public ShowPicker OnShowPicker;


    public void ShowColorPicker(Color color)
    {
        if (OnShowPicker != null)
        {
            OnShowPicker();
        }

        redSlider.value = color.r;
        greenSlider.value = color.g;
        blueSlider.value = color.b;

        OnValueChanged(0);

        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(0.005f, 0.4f).SetEase(Ease.OutBack);
    }
    
    public void HideColorPicker()
    {
        transform.DOScale(0, 0.3f).OnComplete(() =>
            {
                if (OnHidePicker != null)
                {
                    OnHidePicker();
                }
                gameObject.SetActive(false);
            });
    }
    
    public void OnValueChanged(float value)
    {
        colourPanel.color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        if (OnColourChanged != null)
        {        
            OnColourChanged(colourPanel.color);
        }
    }

}
