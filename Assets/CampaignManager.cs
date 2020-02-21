using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        PlayerManager.Instance().SpawnKing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}