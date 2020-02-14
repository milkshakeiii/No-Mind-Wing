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
        Color color = Color.white;

        if (red)
            color = new Color(value, targetImage.color.g, targetImage.color.b);
        if (green)
            color = new Color(targetImage.color.r, value, targetImage.color.b);
        if (blue)
            color = new Color(targetImage.color.r, targetImage.color.g, value);

        targetImage.color = color;
        targetImage.gameObject.GetComponent<ColorSelector>().ColorChange();
    }
}
