using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public TMPro.TMP_InputField input;

    public UnityEngine.UI.Slider sizeSlider;
    public UnityEngine.UI.Slider qualitySlider;

    public void Save()
    {
        string name = input.text;
        PixelGridManager.Instance().SaveTexture(name);
        string buildstring = "build ";
        buildstring += name + " ";
        buildstring += sizeSlider.value.ToString() + " ";
        buildstring += qualitySlider.value.ToString() + " ";
        foreach (VesselPart part in PartPlacementManager.Instance().GetListOfParts())
        {
            if (part.partType == VesselPartType.Bay)
                buildstring += "-bay ";
            else if (part.partType == VesselPartType.Engine)
                buildstring += "-engine ";
            else if (part.partType == VesselPartType.Launcher)
                buildstring += "-launcher ";
            buildstring += part.size.ToString() + " ";
            buildstring += part.quality1.ToString() + " ";
            buildstring += part.quality2.ToString() + " ";
            Vector2 positionInRelativeForm = PixelGridManager.Instance().GetRelativeForm(part.position);
            buildstring += positionInRelativeForm.x.ToString() + " ";
            buildstring += positionInRelativeForm.y.ToString() + " ";
            buildstring += part.facing.ToString() + " ";
        }

        string vesselDir = GameStringsHelper.VesselDirectory();
        if (!System.IO.Directory.Exists(vesselDir))
        {
            System.IO.Directory.CreateDirectory(vesselDir);
        }
        string path = GameStringsHelper.VesselNameToPath(name);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        System.IO.File.WriteAllText(path, buildstring);
    }

    public void SetKingSpriteName()
    {
        PlayerPrefs.SetString("king sprite name", input.text);
    }

    public void BeginCampaign()
    {
        CampaignManager.Begin();
    }
}
