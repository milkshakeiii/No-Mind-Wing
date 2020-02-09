using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private struct VesselSprite
    {
        public string name;
        public VesselType vesselType;
        public Sprite sprite;
    }

    private static SpriteManager instance;

    private Dictionary<string, VesselSprite> vesselSprites = new Dictionary<string, VesselSprite>();

    public static SpriteManager Instance()
    {
        return instance;
    }

    // Awake is called before Start
    void Awake()
    {
        instance = this;

        LoadSprites();
    }

    public void LoadSprites()
    {
        string customSpritesPath = System.IO.Path.Combine(Application.persistentDataPath, "Sprites");
        if (!System.IO.Directory.Exists(customSpritesPath))
            System.IO.Directory.CreateDirectory(customSpritesPath);
        List<string> customSpritePaths = new List<string>();
        foreach (string path in System.IO.Directory.EnumerateFiles(customSpritesPath))
        {
            LoadCustomSprite(path);
        }
        LoadDefaultSprite("square");
        LoadDefaultSprite("circle");
        LoadDefaultSprite("halfcircle");
        LoadDefaultSprite("triangle");
    }

    private void LoadDefaultSprite(string fileName)
    {
        Texture2D newTexture = (Texture2D)Resources.Load("Sprites/" + fileName);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, 196, 196), new Vector2(0.5f, 0.5f));
        VesselSprite newVesselSprite = new VesselSprite
        {
            name = fileName,
            sprite = newSprite,
            vesselType = VesselType.Square
        };
        vesselSprites[fileName] = newVesselSprite;
    }

    private void LoadCustomSprite(string path)
    {
        byte[] data = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(196, 196);
        texture.LoadImage(data);
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, 196, 196), new Vector2(0.5f, 0.5f));
        for (int i = 0; i < 196; i++)
        {
            for (int j = 0; j < 196; j++)
            {
                Color pixel = newSprite.texture.GetPixels(i, j, 1, 1)[0];
                if (pixel.r == 0f && pixel.g == 0f && pixel.b == 0f)
                    newSprite.texture.SetPixel(i, j, new Color(1, 1, 1, 0));
                else
                    newSprite.texture.SetPixel(i, j, new Color(1, 1, 1, 1));
            }
        }
        newSprite.texture.Apply();

        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        VesselSprite newVesselSprite = new VesselSprite
        {
            name = fileName,
            sprite = newSprite,
            vesselType = VesselType.Square
        };
        vesselSprites[fileName] = newVesselSprite;
    }

    public bool SpriteNameIsGood(string name)
    {
        return vesselSprites.ContainsKey(name);
    }

    public Sprite SpriteFromName(string name)
    {
        return vesselSprites[name].sprite;
    }

    public VesselType VesselTypeFromName(string name)
    {
        return vesselSprites[name].vesselType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
