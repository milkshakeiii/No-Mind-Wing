using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelector : MonoBehaviour
{
    public int colorNumber;

    public void OnClicked()
    {
        ColorChange();
        Color color = gameObject.GetComponent<UnityEngine.UI.Image>().color;
        PixelGridManager.Instance().SelectColor(colorNumber);
    }

    public void ColorChange()
    {
        PixelGridManager.Instance().SetColor(colorNumber, gameObject.GetComponent<UnityEngine.UI.Image>().color);
    }
}