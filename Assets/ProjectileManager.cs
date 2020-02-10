using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;

    private Dictionary<GameObject, Launcher> projectiveToLauncher = new Dictionary<GameObject, Launcher>();
    private Dictionary<Launcher, GameObject> launcherToProjectile = new Dictionary<Launcher, GameObject>();

    private Queue<GameObject> freeProjectiles = new Queue<GameObject>();

    public static ProjectileManager Instance()
    {
        return instance;
    }

    public bool Fire(Launcher source)
    {
        float stockpile = PlayerManager.Instance().GetResourceStockpile(ResourceType.Launch);
        float cost = FireCost(source);
        if (stockpile < cost)
            return false;
        if (launcherToProjectile[source] != null)
            return false;

        if (!PlayerManager.Instance().AddResource(-cost, ResourceType.Launch))
            throw new UnityException("Fire attempt to fire without sufficient resources");

        GameObject projectile = GetFreeProjectile();
        projectile.transform.localScale = Vector3.one * source.ProjectileSize();
        projectile.transform.position = source.transform.position;
        projectile.GetComponent<Rigidbody2D>().velocity = source.transform.up * source.LaunchVelocity();
        StartCoroutine(ExplodeTimer(projectile, source.LaunchRange())); //start here

        return true;
    }

    private IEnumerator ExplodeTimer(GameObject projectile, float range)
    {
        yield break;
    }

    private GameObject GetFreeProjectile()
    {
        if (freeProjectiles.Count != 0)
        {
            return freeProjectiles.Dequeue();
        }

        GameObject newProjectile = new GameObject("projectile");

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
