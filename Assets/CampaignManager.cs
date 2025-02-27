﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        string kingSpriteName = PlayerPrefs.GetString("king vessel name");
        PlayerManager.Instance().SpawnKing(kingSpriteName);
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        Vessel enemy = VesselManager.Instance().BuildVessel(false,
                                                            new List<Vessel>(),
                                                            "enemy_vessel_name_0",
                                                            "enemy1");
        VesselManager.Instance().GetMind("enemy1").AddBehavior(0, new AttackPlayerKingBehavior());
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