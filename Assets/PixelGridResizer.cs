using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGridResizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int imageEndSize = SpriteManager.SPRITE_SIZE;

        Vector2 pixelCenter = new Vector2(Screen.width / 4, Screen.height / 2);

        int halfSize = imageEndSize / 2;
        int wideMultiplesAvailable = Mathf.FloorToInt((float)pixelCenter.x / (float)halfSize);
        int tallMultiplesAvailable = Mathf.FloorToInt((float)pixelCenter.y / (float)halfSize);
        int multiplesAvailable = Mathf.Min(wideMultiplesAvailable, tallMultiplesAvailable);
        if (multiplesAvailable == 0)
        {
            throw new UnityException("It looks like there aren't " + imageEndSize.ToString() +
                                     " pixels available.  I only have " + pixelCenter.x.ToString() + ".");
        }
        int rectTransformSize = imageEndSize * multiplesAvailable;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransformSize, rectTransformSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
