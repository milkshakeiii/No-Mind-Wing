using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPlacementManager : MonoBehaviour
{
    private static PartPlacementManager instance;

    private VesselPart partInProgress = new VesselPart();
    private Dictionary<PixelGridButton, VesselPart> buttonToPlacedPart = new Dictionary<PixelGridButton, VesselPart>();

    public UnityEngine.UI.Image demoImage;

    public static PartPlacementManager Instance()
    {
        return instance;
    }
    
    public void PlacePart(PixelGridButton button)
    {
        if (buttonToPlacedPart.ContainsKey(button))
        {
            Debug.Log("There is already a part there");
            return;
        }

        GameObject placedPart = new GameObject("Placed Part");
        RectTransform rectTransform = placedPart.AddComponent<RectTransform>();
        placedPart.transform.SetParent(button.transform);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        float halfsize = partInProgress.size * SpriteManager.SPRITE_SIZE * PixelGridManager.Instance().GetSizeFactor() / 2;
        rectTransform.offsetMin = Vector2.one * -1 * halfsize;
        rectTransform.offsetMax = Vector2.one * halfsize;
        UnityEngine.UI.Image image = placedPart.AddComponent<UnityEngine.UI.Image>();
        image.sprite = SpriteManager.Instance().SpriteFromName("square");
        image.color = Color.black;

        VesselPart newPart = partInProgress;
        buttonToPlacedPart[button] = newPart;
    }

    public void DeletePart(PixelGridButton button)
    {
        buttonToPlacedPart.Remove(button);
        Destroy(button.gameObject.transform.GetChild(0));
    }

    public void SetQuality1(float quality1)
    {
        partInProgress.quality1 = quality1;
        UpdateDemoImageSizeAndColor();
    }

    public void SetQuality2(float quality2)
    {
        partInProgress.quality2 = quality2;
        UpdateDemoImageSizeAndColor();
    }

    public void SetSize(float size)
    {
        partInProgress.size = size;
        UpdateDemoImageSizeAndColor();
    }

    private void UpdateDemoImageSizeAndColor()
    {
        float averageQuality = (partInProgress.quality1 + partInProgress.quality2) / 2;
        demoImage.color = Color.white * averageQuality;
        demoImage.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 
                                                                       SpriteManager.SPRITE_SIZE *
                                                                       partInProgress.size;
    }

    public void SelectBay()
    {
        partInProgress.partType = VesselPartType.Bay;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("square");
    }

    public void SelectEngine()
    {
        partInProgress.partType = VesselPartType.Engine;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("halfcicle");
    }

    public void SelectLauncher()
    {
        partInProgress.partType = VesselPartType.Launcher;
        demoImage.sprite = SpriteManager.Instance().SpriteFromName("triangle");
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
