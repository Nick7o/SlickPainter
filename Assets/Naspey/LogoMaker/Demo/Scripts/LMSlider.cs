using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Naspey.LogoMaker;
using TMPro;

public class LMSlider : LMBaseComponent
{
    [System.Serializable]
    enum SliderPropertyType { Size, Hardness }

    [SerializeField] SliderPropertyType sliderType;

    [Space]
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text infoLabel;

    private void Start()
    {
        // 'infoLabel' can be null in case we don't need to display the information.
        Debug.Assert(slider != null, "LogoMaker :: Slider is not assigned.", this);

        slider.onValueChanged.AddListener(
            delegate (float value)
            {
                foreach (var canvas in affectedCanvas)
                {
                    var brushCopy = canvas.Brush;

                    switch (sliderType)
                    {
                        case SliderPropertyType.Size:
                            brushCopy.Size = (int)value;
                            break;
                        case SliderPropertyType.Hardness:
                            brushCopy.Hardness = value;
                            break;
                        default:
                            break;
                    }

                    canvas.Brush = brushCopy;
                }

                if (infoLabel != null)
                {
                    string suffix = string.Empty;
                    switch (sliderType)
                    {
                        case SliderPropertyType.Size:
                            suffix = "px";
                            break;
                        case SliderPropertyType.Hardness:
                            suffix = "%";
                            break;
                        default:
                            break;
                    }

                    infoLabel.text = $"{value.ToString("0")} {suffix}";
                }
            });
    }
}
