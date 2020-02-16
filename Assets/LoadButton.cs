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
            Sprite sprite = SpriteManager.Instance().SpriteFromName(pathOrName);
            PixelGridManager.Instance().LoadTexture(sprite.texture);
        }
        else
        {
            byte[] data = System.IO.File.ReadAllBytes(pathOrName);
            Texture2D texture = new Texture2D(SpriteManager.SPRITE_SIZE, SpriteManager.SPRITE_SIZE);
            texture.LoadImage(data);
            PixelGridManager.Instance().LoadTexture(texture);
        }
    }
}
