using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPlacementManager : MonoBehaviour
{
    private static PartPlacementManager instance;

    private VesselPart partInProgress = new VesselPart()
    {
        quality1 = 1f,
        quality2 = 1f,
        size = 0.5f,
        facing = 0f,
    };
    private Dictionary<PixelGridButton, VesselPart> buttonToPlacedPart = new Dictionary<PixelGridButton, VesselPart>();

    public UnityEngine.UI.Image demoImage;

    public static PartPlacementManager Instance()
    {
        return instance;
    }

    public void ResetParts()
    {
        buttonToPlacedPart = new Dictionary<PixelGridButton, VesselPart>();
    }
    
    public void PlacePart(PixelGridButton button)
    {
        if (buttonToPlacedPart.ContainsKey(button))
        {
            Debug.Log("Dictionary already contains key " + button.ToString());
            Debug.Log("There is already a part there.");
            return;
        }

        button.gameObject.transform.SetSiblingIndex(button.transform.parent.childCount - 1);

        GameObject placedPart = new GameObject("Placed Part");
        RectTransform rectTransform = placedPart.AddComponent<RectTransform>();
        placedPart.transform.SetParent(button.transform);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        float halfsize = partInProgress.size * 
                         Vessel.partSizeFactor *
                         SpriteManager.SPRITE_SIZE * 
                         PixelGridManager.Instance().GetSizeFactor() / 2;
        rectTransform.offsetMin = Vector2.one * -1 * halfsize;
        rectTransform.offsetMax = Vector2.one * halfsize;
        rectTransform.eulerAngles = new Vector3(0, 0, partInProgress.facing);
        UnityEngine.UI.Image image = placedPart.AddComponent<UnityEngine.UI.Image>();
        string spriteName;
        if (partInProgress.partType == VesselPartType.Bay)
            spriteName = "square";
        else if (partInProgress.partType == VesselPartType.Engine)
            spriteName = "halfcircle";
        else //(partInProgress.partType == VesselPartType.Launcher)
            spriteName = "triangle";
        image.sprite = SpriteManager.Instance().SpriteFromName(spriteName);
        image.color = Color.white * GetPartColor();
        image.raycastTarget = false;

        VesselPart newPart = partInProgress;
        buttonToPlacedPart[button] = newPart;

        GameObject deleter = new GameObject("Deleter");
        rectTransform = deleter.AddComponent<RectTransform>();
        deleter.transform.SetParent(button.transform);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        image = deleter.AddComponent<UnityEngine.UI.Image>();
        image.sprite = SpriteManager.Instance().SpriteFromName("X");
        image.color = new Color(0.75f, 0, 0);
        image.raycastTarget = false;
    }

    public void DeletePart(PixelGridButton button)
    {
        buttonToPlacedPart.Remove(button);
        for (int i = 0; i < button.transform.childCount; i++)
            Destroy(button.transform.GetChild(i).gameObject);
    }

    public void SetQuality1(float quality1)
    {
        partInProgress.quality1 = quality1;
        UpdateDemoImage();
    }

    public void SetQuality2(float quality2)
    {
        partInProgress.quality2 = quality2;
        UpdateDemoImage();
    }

    public void SetSize(float size)
    {
        partInProgress.size = size;
        UpdateDemoImage();
    }

    public void SetFacing(float facing)
    {
        partInProgress.facing = facing;
        UpdateDemoImage();
    }

    private float GetPartColor()
    {
        return (partInProgress.quality1 + partInProgress.quality2) / 2;
    }

    private void UpdateDemoImage()
    {
        float averageQuality = GetPartColor();
        demoImage.color = Color.white * averageQuality;
        demoImage.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 
                                                                       SpriteManager.SPRITE_SIZE *
                                                                       partInProgress.size;
        demoImage.rectTransform.eulerAngles = new Vector3(0, 0, partInProgress.facing);
    }

    public void SelectBay()
    {
        partInProgress.partType = VesselPartType.Bay;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("square");
        PixelGridManager.Instance().SetPartPlacing();
    }

    public void SelectEngine()
    {
        partInProgress.partType = VesselPartType.Engine;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("halfcircle");
        PixelGridManager.Instance().SetPartPlacing();
    }

    public void SelectLauncher()
    {
        partInProgress.partType = VesselPartType.Launcher;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("triangle");
        PixelGridManager.Instance().SetPartPlacing();
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        UpdateDemoImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
