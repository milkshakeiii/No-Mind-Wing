using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSizeButtonSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<int> divisors = new List<int>();
        for (int i = 1; i < 11; i++)
        {
            if (SpriteManager.SPRITE_SIZE % i == 0)
                divisors.Add(i);
        }

        for (int i = 0; i < divisors.Count; i++)
        {
            int divisor = divisors[i];
            GameObject buttonObject = new GameObject("Resize Button " + divisor.ToString() );
            UnityEngine.UI.Button buttonComponent = buttonObject.AddComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.ColorBlock colorBlock = buttonComponent.colors;
            colorBlock.highlightedColor = Color.grey;
            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            buttonObject.transform.SetParent(gameObject.transform);
            buttonObject.AddComponent<PixelSizeButton>().Initiate(divisor);
            TMPro.TextMeshProUGUI text = buttonObject.AddComponent<TMPro.TextMeshProUGUI>();
            text.color = Color.black;
            text.text = divisor.ToString();
            rectTransform.anchorMin = new Vector2((float)i / divisors.Count, 0);
            rectTransform.anchorMax = new Vector2((float)(i + 1) / divisors.Count, 1);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            text.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}