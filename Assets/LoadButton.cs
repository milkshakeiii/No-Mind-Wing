using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton : MonoBehaviour
{
    public TMPro.TMP_InputField input;

    public void Load()
    {
        string pathOrName = input.text;
        if (SpriteManager.Instance().SpriteNameIsGood(pathOrName))
        {
            string vesselDir = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Vessels");
            string buildStringPath = System.IO.Path.Combine(vesselDir, pathOrName + ".vessel");
            string spriteName = pathOrName;
            bool buildStringExists = System.IO.File.Exists(buildStringPath);
            if (buildStringExists)
            {
                string buildString = System.IO.File.ReadAllText(buildStringPath);
                spriteName = buildString.Split(' ')[1];
            }
            Sprite sprite = SpriteManager.Instance().SpriteFromName(pathOrName);
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
        List<VesselPart> parts = TerminalInputManager.PartsListFromBuildString(buildString);
        foreach (VesselPart part in parts)
        {
            PixelGridButton button = PixelGridManager.Instance().ButtonFromPosition(part.position);
            PartPlacementManager.Instance().PlacePart(button, part);
        }
    }
}
