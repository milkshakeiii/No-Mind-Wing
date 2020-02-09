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

    public string BuildVessel(string spriteName, float newSize, float newDurability, string newDesignation, List<VesselPart> parts)
    {
        if (!SpriteManager.Instance().SpriteNameIsGood(spriteName))
            return "No sprite was found with name: " + spriteName;

        GameObject newVessel = new GameObject(newDesignation);

        SpriteRenderer newSpriteRenderer = newVessel.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName(spriteName);
        VesselType newVesselType = SpriteManager.Instance().VesselTypeFromName(spriteName);

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

        return "Built vessel, designation: " + newDesignation;
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
