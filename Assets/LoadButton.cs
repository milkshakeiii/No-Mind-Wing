using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton : MonoBehaviour
{
    public TMPro.TMP_InputField input;
    public TMPro.TMP_InputField saveInput;
    public bool loadKingOnStart = false;

    private void Start()
    {
        if (loadKingOnStart && PlayerPrefs.HasKey("king vessel name"))
        {
            string kingVesselName = PlayerPrefs.GetString("king vessel name");
            LoadByPathOrName(kingVesselName);
            saveInput.text = kingVesselName;
        }
    }

    public void Load()
    {
        LoadByPathOrName(input.text);
    }

    public void LoadByPathOrName(string pathOrName)
    {
        if (SpriteManager.Instance().SpriteNameIsGood(pathOrName))
        {
            string vesselDir = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Vessels");
            string buildStringPath = GameStringsHelper.VesselNameToPath(pathOrName);
            string spriteName = pathOrName;
            bool buildStringExists = System.IO.File.Exists(buildStringPath);
            if (buildStringExists)
            {
                string buildString = System.IO.File.ReadAllText(buildStringPath);
                spriteName = buildString.Split(' ')[1];
            }
            Sprite sprite = SpriteManager.Instance().SpriteFromName(spriteName);
            PixelGridManager.Instance().LoadTexture(sprite.texture);
            if (buildStringExists)
            {
                string buildString = System.IO.File.ReadAllText(buildStringPath);
                LoadVesselParts(buildString);
            }
        }
        else
        {
            byte[] data = System.IO.File.ReadAllBytes(pathOrName);
            Texture2D texture = new Texture2D(SpriteManager.SPRITE_SIZE, SpriteManager.SPRITE_SIZE);
            texture.LoadImage(data);
            PixelGridManager.Instance().LoadTexture(texture);
        }
    }

    public void LoadVesselParts(string buildString)
    {
        List<VesselPart> parts = GameStringsHelper.PartsListFromBuildString(buildString);
        foreach (VesselPart part in parts)
        {
            PixelGridButton button = PixelGridManager.Instance().ButtonFromRelativePosition(part.position);
            PartPlacementManager.Instance().PlacePart(button, part);
        }
    }
}
