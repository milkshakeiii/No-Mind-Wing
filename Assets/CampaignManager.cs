using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        string kingSpriteName = PlayerPrefs.GetString("king sprite name");
        PlayerManager.Instance().SpawnKing(kingSpriteName);
    }

    public static void Being()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}