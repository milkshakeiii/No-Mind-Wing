using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VesselType
{
    Square = 1,
    Round = 2,
    Jagged = 3
}

public enum VesselPartType
{
    Bay = 1,
    Engine = 2,
    Launcher = 3
}

public struct VesselPart
{
    public VesselPartType partType;
    public Vector2 position;
    public float facing; //in degrees
    public float size; // maximum vessel size, thrust maximum, projectile size
    public float quality1; // gathering warmup, thrust warmup, projectile velocity
    public float quality2; // gathering maximum, speed maximum, range
}
public class VesselManager : MonoBehaviour
{
    private Dictionary<string, List<Vessel>> vesselDict = new Dictionary<string, List<Vessel>>();

    private static VesselManager instance;

    public static VesselManager Instance()
    {
        return instance;
    }

    public List<Vessel> GetVesselsByDesignation(List<string> designations)
    {
        List<Vessel> vessels = new List<Vessel>();
        foreach (string designation in designations)
        {
            vessels.AddRange(vesselDict[designation]);
        }
        return vessels;
    }

    public string BuildVessel(bool requireSource, List<Vessel> sourceVessels, string spriteName, float newSize, float newDurability, string newDesignation, List<VesselPart> parts)
    {
        if (requireSource && sourceVessels.Count == 0)
        {
            return "No vessels selected";
        }
        Vector2 buildLocation = Vector2.zero;
        bool locationFound = false;
        if (sourceVessels.Count == 0)
        {
            buildLocation = new Vector2(0, 0);
            locationFound = true;
        }
        foreach (Vessel vessel in sourceVessels)
        {
            List<Bay> largeEnoughBays = vessel.LargeEnoughBays(newSize);
            if (largeEnoughBays.Count > 0)
            {
                buildLocation = largeEnoughBays[0].gameObject.transform.position;
                locationFound = true;
                break;
            }
        }
        if (!locationFound)
        {
            return "No selected vessel has a large enough bay";
        }

        if (!SpriteManager.Instance().SpriteNameIsGood(spriteName))
        {
            return "No sprite was found with name: " + spriteName;
        }

        VesselType newVesselType = SpriteManager.Instance().VesselTypeFromName(spriteName);
        float cost = VesselCost(newVesselType, newSize, newDurability, parts);
        float stockpile = PlayerManager.Instance().GetResourceStockpile(ResourceType.Build);
        if (requireSource && (cost > stockpile))
        {
            return cost.ToString() + " is required but only " + stockpile.ToString() + " is stockpiled.";
        }

        GameObject newVessel = new GameObject(newDesignation);
        newVessel.transform.position = buildLocation;

        SpriteRenderer newSpriteRenderer = newVessel.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName(spriteName);

        Rigidbody2D newRigidbody = newVessel.AddComponent<UnityEngine.Rigidbody2D>();
        newRigidbody.gravityScale = 0;

        //Collider2D newCollider = 
        newVessel.AddComponent<PolygonCollider2D>();

        Vessel newVesselScript = newVessel.AddComponent<Vessel>();
        newVesselScript.Initiate(newVesselType, newSize, newDurability, newDesignation, parts);

        if (!vesselDict.ContainsKey(newDesignation))
        {
            vesselDict[newDesignation] = new List<Vessel>();
        }
        vesselDict[newDesignation].Add(newVesselScript);

        if(requireSource && !PlayerManager.Instance().AddResource(-cost, ResourceType.Build))
            return "Problem: I thought there were enough resources but AddResource returned false";
        return "Built vessel, designation: " + newDesignation;
    }

    private float VesselCost(VesselType type, float size, float durability, List<VesselPart> parts)
    {
        return 0.1f;
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
