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

        string vesselDir = VesselDirectory();
        if (!System.IO.Directory.Exists(vesselDir))
        {
            System.IO.Directory.CreateDirectory(vesselDir);
        }
        string path = VesselNameToPath(name);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        System.IO.File.WriteAllText(path, buildstring);
    }

    public static string VesselNameToPath(string name)
    {
        string vesselDir = VesselDirectory();
        string path = System.IO.Path.Combine(vesselDir, name + ".vessel");
        return path;
    }

    private static string VesselDirectory()
    {
        return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Vessels");
    }

    public static Dictionary<string, string> AllSavedNamesToBuildstrings()
    {
        Dictionary<string, string> returnDict = new Dictionary<string, string>();
        foreach (string path in System.IO.Directory.EnumerateFiles(VesselDirectory()))
        {
            if (System.IO.Path.GetExtension(path) == ".vessel")
            {
                returnDict[System.IO.Path.GetFileNameWithoutExtension(path)] =
                    System.IO.File.ReadAllText(path);
            }
        }
        return returnDict;
    }
}
