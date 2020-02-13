using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangerSlider : MonoBehaviour
{
    public bool red = false;
    public bool green = false;
    public bool blue = false;

    public UnityEngine.UI.Image targetImage;

    public void OnValueChanged(float value)
    {
        if (red)
            targetImage.color = new Color(value, targetImage.color.g, targetImage.color.b);
        if (green)
            targetImage.color = new Color(targetImage.color.r, value, targetImage.color.b);
        if (blue)
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, value);
    }
}
