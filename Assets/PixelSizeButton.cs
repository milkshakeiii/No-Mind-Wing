using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSizeButton : MonoBehaviour
{
    int i;

    public void Initiate(int newi)
    {
        i = newi;
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Submit);
    }

    public void Submit()
    {
        PixelGridManager.Instance().SpawnButtons(i);
    }
}