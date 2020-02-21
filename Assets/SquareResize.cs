using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareResize : MonoBehaviour
{
    public UnityEngine.UI.Image image;

    // Start is called before the first frame update
    void Start()
    {
        image.rectTransform.sizeDelta = new Vector2(Screen.width / 10, Screen.width / 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
