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
    private ToolType currentTool = ToolType.paint;

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

    public Vector2 GetRelativeForm(Vector2 gridPosition)
    {
        return -Vector2.one + 
               2 * (gridPosition * CurrentPixelsPerBox() / SpriteManager.SPRITE_SIZE) +
               Vector2.one * (CurrentPixelsPerBox() / SpriteManager.SPRITE_SIZE);
    }

    public Vector2 GetPixelForm(Vector2 relativeForm)
    {
        float xMultiplier = (relativeForm.x + 1) / 2;
        float yMultiplier = (relativeForm.y + 1) / 2;
        int gridI = Mathf.FloorToInt(((SpriteManager.SPRITE_SIZE) / CurrentPixelsPerBox()) * xMultiplier);
        int gridJ = Mathf.FloorToInt(((SpriteManager.SPRITE_SIZE) / CurrentPixelsPerBox()) * yMultiplier);
        return new Vector2(gridI, gridJ);
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

        colorsByNumber[0] = Color.white;
        colorsByNumber[1] = Color.black;
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
        colorButtons = new Dictionary<int, HashSet<UnityEngine.UI.Image>>();
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
                UnityEngine.EventSystems.EventTrigger eventTrigger = buttonObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
                entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) => { buttonScript.MouseEnter(); });
                eventTrigger.triggers.Add(entry);
                buttonGrid[i, j] = buttonComponent;
            }
        }
        if (PartPlacementManager.Instance() != null)
            PartPlacementManager.Instance().ResetParts();
    }

    public PixelGridButton ButtonFromRelativePosition(Vector2 position)
    {
        position = GetPixelForm(position);
        return buttonGrid[(int)position.x, (int)position.y].gameObject.GetComponent<PixelGridButton>();
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
            PartPlacementManager.Instance().PlacePartFromUI(button);
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
            ColorButtonsAdd(selectedColor, image);
        }
    }

    private void ColorButtonsAdd(int key, UnityEngine.UI.Image value)
    {
        if (!colorButtons.ContainsKey(key))
        {
            colorButtons[key] = new HashSet<UnityEngine.UI.Image>();
        }
        colorButtons[key].Add(value);
    }

    public void SaveTexture(string vesselName)
    {
        Texture2D texture = new Texture2D(SpriteManager.SPRITE_SIZE, SpriteManager.SPRITE_SIZE);
        int currentPixelsPerBox = CurrentPixelsPerBox();
        for (int i = 0; i < buttonGrid.GetLength(0); i++)
        {
            for (int j = 0; j < buttonGrid.GetLength(1); j++)
            {
                Color color = buttonGrid[i, j].GetComponent<UnityEngine.UI.Image>().color;
                for (int k = 0; k < currentPixelsPerBox; k++)
                {
                    for (int l = 0; l < currentPixelsPerBox; l++)
                    {
                        texture.SetPixel(i * currentPixelsPerBox + k, j * currentPixelsPerBox + l, color);
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
                    currentChainVertical = 1;
                }
                if ((i == 0 && j == 0) || currentColorHorizontal == lastColorHorizontal)
                {
                    currentChainHorizontal++;
                }
                else
                {
                    shortestChain = Mathf.Min(currentChainHorizontal, shortestChain);
                    currentChainHorizontal = 1;
                }
                lastColorVertical = currentColorVertical;
                lastColorHorizontal = currentColorHorizontal;
            }
        }

        if (SpriteManager.SPRITE_SIZE % shortestChain != 0)
        {
            throw new UnityException("This texture is divided into units which don't divide SPRITE_SIZE");
        }

        Debug.Log("I detected " + shortestChain.ToString() + " pixel sided squares.");
        SpawnButtons(shortestChain);

        Color color1 = Color.white;
        Color color2 = Color.white;
        for (int i = 0; i < SpriteManager.SPRITE_SIZE / shortestChain; i ++)
        {
            for (int j = 0; j < SpriteManager.SPRITE_SIZE / shortestChain; j++)
            {
                UnityEngine.UI.Button button = buttonGrid[i, j];
                UnityEngine.UI.Image image = button.gameObject.GetComponent<UnityEngine.UI.Image>();
                Color color = texture.GetPixel(i * shortestChain, j * shortestChain);
                if (color.a == 0)
                {
                    color = Color.black;
                    ColorButtonsAdd(1, image);
                }
                else if (color == Color.white)
                {
                    ColorButtonsAdd(0, image);
                }
                else if (color1 == Color.white || color == color1)
                {
                    color1 = color;
                    ColorButtonsAdd(2, image);
                }
                else
                {
                    color2 = color;
                    ColorButtonsAdd(3, image);
                }
                image.color = color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
