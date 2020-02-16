using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public const int SPRITE_SIZE = 210;

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

    public string GetCustomSpritesPath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "Sprites");
    }

    public void LoadSprites()
    {
        string customSpritesPath = GetCustomSpritesPath();
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
        LoadDefaultSprite("X");
    }

    private void LoadDefaultSprite(string fileName)
    {
        Texture2D newTexture = (Texture2D)Resources.Load("Sprites/" + fileName);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, SPRITE_SIZE, SPRITE_SIZE), new Vector2(0.5f, 0.5f));
        VesselSprite newVesselSprite = new VesselSprite
        {
            name = fileName,
            sprite = newSprite,
            vesselType = VesselType.Square
        };
        vesselSprites[fileName] = newVesselSprite;
    }

    public void SaveCustomSprite(Texture2D texture, string name)
    {
        byte[] data = texture.EncodeToPNG();
        string filepath = System.IO.Path.Combine(GetCustomSpritesPath(), name);
        if (System.IO.File.Exists(filepath))
        {
            System.IO.File.Delete(filepath);
        }
        System.IO.File.WriteAllBytes(filepath, data);
        LoadCustomSprite(filepath);
    }

    public string LoadCustomSprite(string path)
    {
        byte[] data = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(SPRITE_SIZE, SPRITE_SIZE);
        texture.LoadImage(data);
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, SPRITE_SIZE, SPRITE_SIZE), new Vector2(0.5f, 0.5f));
        for (int i = 0; i < SPRITE_SIZE; i++)
        {
            for (int j = 0; j < SPRITE_SIZE; j++)
            {
                Color pixel = newSprite.texture.GetPixels(i, j, 1, 1)[0];
                if (pixel.r == 0f && pixel.g == 0f && pixel.b == 0f)
                    newSprite.texture.SetPixel(i, j, new Color(0, 0, 0, 0));
                else
                    newSprite.texture.SetPixel(i, j, new Color(pixel.r, pixel.g, pixel.b, 1));
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
        Debug.Log("Loaded sprite: " + fileName.ToString());

        return fileName;
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
