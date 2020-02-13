using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGridManager : MonoBehaviour
{
    private UnityEngine.UI.Button[,] buttonGrid = new UnityEngine.UI.Button[0, 0];

    // Start is called before the first frame update
    void Start()
    {
        SetPixelSize();

        SpawnButtons(10);
    }

    private void SetPixelSize()
    {
        int imageEndSize = SpriteManager.SPRITE_SIZE;

        Vector2 pixelCenter = new Vector2(Screen.width / 4, Screen.height / 2);

        int halfSize = imageEndSize / 2;
        int wideMultiplesAvailable = Mathf.FloorToInt((float)pixelCenter.x / (float)halfSize);
        int tallMultiplesAvailable = Mathf.FloorToInt((float)pixelCenter.y / (float)halfSize);
        int multiplesAvailable = Mathf.Min(wideMultiplesAvailable, tallMultiplesAvailable);
        if (multiplesAvailable == 0)
        {
            throw new UnityException("It looks like there aren't " + imageEndSize.ToString() +
                                     " pixels available.  I only have " + pixelCenter.x.ToString() + ".");
        }
        int rectTransformSize = imageEndSize * multiplesAvailable;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransformSize, rectTransformSize);
    }

    private void SpawnButtons(int buttonSize)
    {
        if (SpriteManager.SPRITE_SIZE % buttonSize != 0)
        {
            throw new UnityException(buttonSize.ToString() + " does not divide " + SpriteManager.SPRITE_SIZE + "!");
        }
        for (int i = 0; i < buttonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < buttonGrid.GetLength(1); j++)
            {
                UnityEngine.UI.Button button = buttonGrid[i, j];
                Destroy(button);
            }
        }
        int buttonGridSize = SpriteManager.SPRITE_SIZE / buttonSize;
        buttonGrid = new UnityEngine.UI.Button[buttonGridSize, buttonGridSize];
        for (int i = 0; i < buttonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < buttonGrid.GetLength(1); j++)
            {
                GameObject buttonObject = new GameObject("Button " + i.ToString() + j.ToString());
                UnityEngine.UI.Button buttonComponent = buttonObject.AddComponent<UnityEngine.UI.Button>();
                RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
                buttonObject.transform.SetParent(gameObject.transform);
                rectTransform.anchorMin = new Vector2((float)i / buttonGridSize, (float)j / buttonGridSize);
                rectTransform.anchorMax = new Vector2((float)(i+1) / buttonGridSize, (float)(j+1) / buttonGridSize);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                UnityEngine.UI.Image image = buttonObject.AddComponent<UnityEngine.UI.Image>();
                image.sprite = SpriteManager.Instance().SpriteFromName("square");
                //buttonScript.targetGraphic = image;
                PixelGridButton buttonScript = buttonObject.AddComponent<PixelGridButton>();
                buttonComponent.onClick.AddListener(buttonScript.OnClicked);
            }
        }
    }

    private void ButtonClicked()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
