﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;

    private Dictionary<GameObject, Launcher> projectileToLauncher = new Dictionary<GameObject, Launcher>();
    private Dictionary<Launcher, GameObject> launcherToProjectile = new Dictionary<Launcher, GameObject>();

    private Queue<GameObject> freeProjectiles = new Queue<GameObject>();

    public static ProjectileManager Instance()
    {
        return instance;
    }

    public Launcher GetLauncherOfProjectile(GameObject projectile)
    {
        if (projectile.tag != "Projectile")
            throw new UnityException("Can't get launcher of something which isn't a projectile.");
        return projectileToLauncher[projectile];
    }

    public bool Fire(Launcher source)
    {
        float stockpile = PlayerManager.Instance().GetResourceStockpile(ResourceType.Launch);
        float cost = FireCost(source);
        if (stockpile < cost)
        {
            return false;
        }
        if (launcherToProjectile.ContainsKey(source) && launcherToProjectile[source] != null)
        {
            return false;
        }

        if (!PlayerManager.Instance().AddResource(-cost, ResourceType.Launch))
            throw new UnityException("Fire attempt to fire without sufficient resources");

        GameObject projectile = GetFreeProjectile();
        projectileToLauncher[projectile] = source;
        projectile.SetActive(true);
        projectile.transform.localScale = Vector3.one * source.ProjectileSize();
        projectile.transform.position = source.transform.position;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = source.transform.up * source.LaunchVelocity();
        launcherToProjectile[source] = projectile;
        StartCoroutine(ExplodeTimer(projectile, source));

        return true;
    }

    private IEnumerator ExplodeTimer(GameObject projectile, Launcher source)
    {
        float range = source.LaunchRange();
        float velocity = source.LaunchVelocity();
        float lifetime = range / velocity;
        float launchTime = Time.time;
        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        float fadeTime = lifetime / 8;
        while (Time.time < launchTime + fadeTime)
        {
            float elapsedTime = Time.time - launchTime;
            float coefficient = (elapsedTime / fadeTime);
            spriteRenderer.color = (1 - coefficient) * Color.clear + (coefficient) * new Color(1, 0.7f, 0);
            yield return null;
        }
        while (Time.time < launchTime + lifetime - fadeTime)
        {
            yield return null;
        }
        while (Time.time < launchTime + lifetime)
        {
            float elapsedTime = Time.time - launchTime - lifetime + fadeTime;
            float coefficient = (elapsedTime / fadeTime);
            spriteRenderer.color = (coefficient) * Color.clear + (1 - coefficient) * new Color(1, 0.7f, 0);
            yield return null;
        }
        ProjectileTimeout(projectile);
    }

    public void ProjectileTimeout(GameObject projectile)
    {
        projectile.SetActive(false);
        launcherToProjectile[projectileToLauncher[projectile]] = null;
        projectileToLauncher[projectile] = null;
        freeProjectiles.Enqueue(projectile);
    }

    public void Explode(GameObject projectile)
    {
        if (projectile != null)
            projectile.SetActive(false);
    }

    public void Explode(Launcher launcher)
    {
        if (launcherToProjectile.ContainsKey(launcher))
            Explode(launcherToProjectile[launcher]);
    }

    private GameObject GetFreeProjectile()
    {
        if (freeProjectiles.Count != 0)
        {
            return freeProjectiles.Dequeue();
        }

        GameObject newProjectile = new GameObject("projectile");
        newProjectile.tag = "Projectile";

        SpriteRenderer newSpriteRenderer = newProjectile.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("square");
        BoxCollider2D newCollider = newProjectile.AddComponent<BoxCollider2D>();
        newCollider.isTrigger = true;
        Rigidbody2D newRigidbody = newProjectile.AddComponent<UnityEngine.Rigidbody2D>();
        newRigidbody.gravityScale = 0;

        newProjectile.transform.parent = gameObject.transform;

        return newProjectile;
    }

    private float FireCost(Launcher source)
    {
        return 0.01f;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
