using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGridManager : MonoBehaviour
{
    private enum ToolType
    {
        noTool = 0,
        paint = 1,
        placePart = 2,
        deletePart = 3
    }
    private ToolType currentTool = ToolType.noTool;

    private int sizeFactor;

    private UnityEngine.UI.Button[,] buttonGrid = new UnityEngine.UI.Button[0, 0];

    private int selectedColor = 0;
    private Dictionary<int, HashSet<UnityEngine.UI.Image>> colorButtons = new Dictionary<int, HashSet<UnityEngine.UI.Image>>();
    private Dictionary<int, Color> colorsByNumber = new Dictionary<int, Color>();

    private static PixelGridManager instance;

    public int GetSizeFactor()
    {
        return sizeFactor;
    }

    public void SelectColor(int colorNumber)
    {
        selectedColor = colorNumber;
        currentTool = ToolType.paint;
    }

    public void SetColor(int colorNumber, Color color)
    {
        colorsByNumber[colorNumber] = color;
        if (colorButtons.ContainsKey(colorNumber))
        {
            foreach (UnityEngine.UI.Image image in colorButtons[colorNumber])
                image.color = color;
        }
    }

    private Color SelectedColor()
    {
        return colorsByNumber[selectedColor];
    }

    public static PixelGridManager Instance()
    {
        return instance;
    }

    public int CurrentPixelsPerBox()
    {
        return SpriteManager.SPRITE_SIZE / buttonGrid.GetLength(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

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
        sizeFactor = multiplesAvailable;
    }

    public void SpawnButtons(int buttonSize)
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
                Destroy(button.gameObject);
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
                image.color = Color.black;
                //buttonScript.targetGraphic = image;
                PixelGridButton buttonScript = buttonObject.AddComponent<PixelGridButton>();
                buttonScript.Initialize(new Vector2(i, j));
                buttonComponent.onClick.AddListener(buttonScript.OnClicked);
                buttonGrid[i, j] = buttonComponent;
            }
        }
        PartPlacementManager.Instance().ResetParts();
    }

    public void SetPartPlacing()
    {
        currentTool = ToolType.placePart;
    }

    public void SetPartDeleting()
    {
        currentTool = ToolType.deletePart;
    }

    public void ButtonClicked(PixelGridButton button)
    {
        if (currentTool == ToolType.placePart)
        {
            PartPlacementManager.Instance().PlacePart(button);
            currentTool = ToolType.noTool;
        }
        else if (currentTool == ToolType.deletePart)
        {
            PartPlacementManager.Instance().DeletePart(button);
        }
        else if (currentTool == ToolType.paint) //regular color-painting click
        {
            UnityEngine.UI.Image image = button.gameObject.GetComponent<UnityEngine.UI.Image>();
            image.color = SelectedColor();
            foreach (int key in colorButtons.Keys)
            {
                colorButtons[key].Remove(image);
            }
            if (!colorButtons.ContainsKey(selectedColor))
            {
                colorButtons[selectedColor] = new HashSet<UnityEngine.UI.Image>();
            }
            colorButtons[selectedColor].Add(image);
        }
    }

    public void SaveTexture(string vesselName)
    {
        Texture2D texture = new Texture2D(SpriteManager.SPRITE_SIZE, SpriteManager.SPRITE_SIZE);
        int currentPixelsPerBox = CurrentPixelsPerBox();
        for (int i = 0; i < buttonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < buttonGrid.GetLength(1); j++)
            {
                Color color = buttonGrid[i, j].image.color;
                for (int k = 0; k < currentPixelsPerBox; k++)
                {
                    for (int l = 0; l < currentPixelsPerBox; l++)
                    {
                        texture.SetPixel(i + k, j + l, color);
                    }
                }
            }
        }
        SpriteManager.Instance().SaveCustomSprite(texture, vesselName);
    }

    public void LoadTexture(Texture2D texture)
    {
        if (texture.width != SpriteManager.SPRITE_SIZE || texture.height != SpriteManager.SPRITE_SIZE)
        {
            throw new UnityException("I expected a square texture with sides of pixel length: " 
                                     + SpriteManager.SPRITE_SIZE.ToString());
        }

        int shortestChain = 10;
        int currentChainVertical = 0;
        int currentChainHorizontal = 0;
        Color lastColorVertical = Color.white;
        Color lastColorHorizontal = Color.white;
        for (int i = 0; i < SpriteManager.SPRITE_SIZE; i++)
        {
            for (int j = 0; j < SpriteManager.SPRITE_SIZE; j++)
            {
                Color currentColorVertical = texture.GetPixel(i, j);
                Color currentColorHorizontal = texture.GetPixel(j, i);
                if ((i == 0 && j == 0) || currentColorVertical == lastColorVertical)
                {
                    currentChainVertical++;
                }
                else
                {
                    shortestChain = Mathf.Min(currentChainVertical, shortestChain);
                }
                if ((i == 0 && j == 0) || currentColorHorizontal == lastColorHorizontal)
                {
                    currentChainHorizontal++;
                }
                else
                {
                    shortestChain = Mathf.Min(currentChainHorizontal, shortestChain);
                }
            }
        }

        if (SpriteManager.SPRITE_SIZE % shortestChain != 0)
        {
            throw new UnityException("This texture is divided into units which don't divide SPRITE_SIZE");
        }

        SpawnButtons(shortestChain);

        for (int i = 0; i < SpriteManager.SPRITE_SIZE; i += shortestChain)
        {
            for (int j = 0; j < SpriteManager.SPRITE_SIZE; j += shortestChain)
            {
                buttonGrid[i, j].image.color = texture.GetPixel(i * shortestChain, j * shortestChain);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
