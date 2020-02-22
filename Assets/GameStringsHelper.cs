using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStringsHelper
{
    public static Vessel PerformBuildFromString(bool requireSource, string buildString, string designation, List<Vessel> sourceVessels)
    {
        string[] inputWords = buildString.Split(' ');
        List<VesselPart> parts = PartsListFromBuildString(buildString);
        string spriteName = inputWords[1];
        float size = float.Parse(inputWords[2]);
        float durability = float.Parse(inputWords[3]);

        return VesselManager.Instance().BuildVessel(requireSource, sourceVessels, spriteName, size, durability, designation, parts);
    }

    public static List<VesselPart> PartsListFromBuildString(string buildstring)
    {
        string[] inputWords = buildstring.Split(' ');
        List<VesselPart> parts = new List<VesselPart>();
        for (int i = 0; i < inputWords.Length; i++)
        {
            string word = inputWords[i];
            bool bay = word.Equals("-bay");
            bool engine = word.Equals("-engine");
            bool launcher = word.Equals("-launcher");
            if (bay || engine || launcher)
            {
                float newSize = float.Parse(inputWords[i + 1]);
                float newQuality1 = float.Parse(inputWords[i + 2]);
                float newQuality2 = float.Parse(inputWords[i + 3]);
                float xPos = float.Parse(inputWords[i + 4]);
                float yPos = float.Parse(inputWords[i + 5]);
                float newFacing = float.Parse(inputWords[i + 6]);

                VesselPartType newPartType;
                if (bay)
                    newPartType = VesselPartType.Bay;
                else if (engine)
                    newPartType = VesselPartType.Engine;
                else
                    newPartType = VesselPartType.Launcher;

                VesselPart newPart = new VesselPart()
                {
                    partType = newPartType,
                    position = new Vector2(xPos, yPos),
                    facing = newFacing,
                    size = newSize,
                    quality1 = newQuality1,
                    quality2 = newQuality2
                };
                parts.Add(newPart);
            }
        }
        return parts;
    }

    private static string LoadCustomSprite(string path)
    {
        if (!System.IO.File.Exists(path))
            return "File " + path + " not found.";
        string fileName = System.IO.Path.GetFileName(path);
        string destinationFolder = System.IO.Path.Combine(Application.persistentDataPath, "Sprites");
        string destinationFilePath = System.IO.Path.Combine(destinationFolder, fileName);
        if (System.IO.File.Exists(destinationFilePath))
            System.IO.File.Delete(destinationFilePath);
        if (!System.IO.Directory.Exists(destinationFolder))
            System.IO.Directory.CreateDirectory(destinationFolder);
        System.IO.File.Copy(path, destinationFilePath);
        SpriteManager.Instance().LoadSprites();
        return "Hull loaded. Name: " + fileName;
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

    public static string VesselNameToPath(string name)
    {
        string vesselDir = VesselDirectory();
        string path = System.IO.Path.Combine(vesselDir, name + ".vessel");
        return path;
    }

    public static string VesselDirectory()
    {
        return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Vessels");
    }
}
