using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTerminalLines : MonoBehaviour
{
    public GameObject logLine;
    public GameObject inputLine;

    private TMPro.TextMeshProUGUI[] logTexts;

    public void PushLine(string line)
    {
        for (int i = logTexts.Length-1; i > 0; i--)
        {
            logTexts[i].text = logTexts[i-1].text;
        }
        logTexts[0].text = line;
    }

    // Start is called before the first frame update
    void Start()
    {
        logTexts = new TMPro.TextMeshProUGUI[9];
        logTexts[0] = logLine.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        for (int i = 1; i < (logTexts.Length); i++)
        {
            GameObject newLogLine = Instantiate(logLine);
            RectTransform newLogLineRect = newLogLine.GetComponent<RectTransform>();
            newLogLineRect.SetParent(gameObject.transform, false);
            newLogLineRect.anchorMin = new Vector2(newLogLineRect.anchorMin.x, ((float)(i+1) / (logTexts.Length+1)));
            newLogLineRect.anchorMax = new Vector2(newLogLineRect.anchorMax.x, ((float)(i+2) / (logTexts.Length+1)));
            newLogLineRect.offsetMin = Vector2.zero;
            newLogLineRect.offsetMax = Vector2.zero;
            logTexts[i] = newLogLine.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
