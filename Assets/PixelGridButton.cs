using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGridButton : MonoBehaviour
{
    private Vector2 position;

    public void Initialize(Vector2 newPosition)
    {
        position = newPosition;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void OnClicked()
    {
        PixelGridManager.Instance().ButtonClicked(this);
    }

    public void MouseEnter()
    {
        if (Input.GetMouseButton(0))
        {
            PixelGridManager.Instance().ButtonClicked(this);
        }
    }
}
