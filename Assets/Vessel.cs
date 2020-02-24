using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
    private VesselType vesselType = VesselType.Square;
    private float hitpoints = 1;
    private float maxHitpoints = 1;
    private List<VesselPart> vesselParts = new List<VesselPart>();
    private List<Engine> engines = new List<Engine>();
    private List<Bay> bays = new List<Bay>();
    private List<Launcher> launchers = new List<Launcher>();
    private string designation = "defaultdesignation";

    public const float partSizeFactor = 0.5f;
    public const float damageFactor = 1f;

    public string GetDesignation()
    {
        return designation;
    }

    public List<Launcher> GetLaunchers()
    {
        return launchers;
    }

    public float GetRange()
    {
        float range = 0;
        foreach (Launcher launcher in launchers)
        {
            range = Mathf.Max(range, launcher.LaunchRange());
        }
        return range;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile" && 
            !(launchers.Contains(ProjectileManager.Instance().GetLauncherOfProjectile(other.gameObject))))
        {
            TakeDamage(other);
            ProjectileManager.Instance().Explode(other.gameObject);
        }
    }

    private void TakeDamage(Collider2D projectileCollider)
    {
        float damage = projectileCollider.transform.localScale.x *
                       projectileCollider.attachedRigidbody.velocity.magnitude;
        hitpoints -= damage;
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        Color newColor = color * (hitpoints / maxHitpoints);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(newColor.r, newColor.g, newColor.b, 1);
        if (hitpoints < 0)
        {
            Destroy(gameObject);
        }
    }

    public List<Bay> LargeEnoughBays(float neededSize)
    {
        List<Bay> largeEnoughBays = new List<Bay>();
        foreach (Bay bay in bays)
        {
            if (bay.gameObject.transform.lossyScale.x > neededSize)
                largeEnoughBays.Add(bay);
        }
        return largeEnoughBays;
    }

    private void BuildParts(List<VesselPart> parts, string designation)
    {
        foreach(VesselPart part in parts)
        {
            GameObject newPart = new GameObject(designation + part.partType.ToString());

            SpriteRenderer newSpriteRenderer = newPart.AddComponent<SpriteRenderer>();

            newPart.transform.parent = gameObject.transform;

            SpriteRenderer vesselSprite = gameObject.GetComponent<SpriteRenderer>();
            newPart.transform.localPosition = new Vector3(part.position.x * vesselSprite.size.x * 0.5f, part.position.y * vesselSprite.size.y * 0.5f, -1);
            newPart.transform.localScale = Vector3.one * part.size * partSizeFactor;
            newPart.transform.localEulerAngles = new Vector3(0, 0, part.facing);
            float quality = (part.quality1 + part.quality2) / 2;
            newPart.GetComponent<SpriteRenderer>().color = new Color(quality, quality, quality);

            if (part.partType == VesselPartType.Bay)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("square");
                Bay newBay = newPart.AddComponent<Bay>();
                newBay.Initiate(part.size, part.quality1, part.quality2);
                newPart.AddComponent<BoxCollider2D>();
                newPart.layer = 8; //Resource

                bays.Add(newBay);
            }
            else if (part.partType == VesselPartType.Engine)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("halfcircle");
                Engine newEngine = newPart.AddComponent<Engine>();
                newEngine.Initiate(part.size, part.quality1, part.quality2);

                engines.Add(newEngine);
            }
            else if (part.partType == VesselPartType.Launcher)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("triangle");
                Launcher newLauncher = newPart.AddComponent<Launcher>();
                newLauncher.Initiate(part.size, part.quality1, part.quality2);

                launchers.Add(newLauncher);
            }

            vesselParts.Add(part);
        }
    }

    public void Initiate(VesselType newVesselType, float newSize, float newDurability, string newDesignation, List<VesselPart> parts)
    {
        vesselType = newVesselType;
        transform.localScale = new Vector3(newSize, newSize, newSize);
        hitpoints = newSize * newSize * newDurability;
        maxHitpoints = hitpoints;
        designation = newDesignation;
        BuildParts(parts, newDesignation);
    }

    public void IgniteEngines(int[] indexes)
    {
        foreach (int index in indexes)
            engines[index].TurnOn();
    }

    public void QuenchEngines(int[] indexes)
    {
        foreach (int index in indexes)
            engines[index].TurnOff();
    }

    public int Fire(int index)
    {
        return Fire(new int[1] { index });
    }

    public int Fire(int[] indexes)
    {
        int firedCount = 0;
        foreach (int index in indexes)
        {
            if (ProjectileManager.Instance().Fire(launchers[index]))
                firedCount++;
        }
        return firedCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach (Launcher launcher in launchers)
        {
            ProjectileManager.Instance().Explode(launcher);
        }

        PlayerManager.Instance().VesselDestroyed(this);
    }
}