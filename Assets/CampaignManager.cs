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
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        Vessel enemy = VesselManager.Instance().BuildVessel(false, new List<Vessel>(), "pewpew", "enemy1");
        enemy.transform.position = new Vector2(2, 2);
    }

    public static void Begin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}