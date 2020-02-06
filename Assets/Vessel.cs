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
    public float quality;
}

public class Vessel : MonoBehaviour
{
    private static Dictionary<string, List<Vessel>> vesselDict = new Dictionary<string, List<Vessel>>();

    private VesselType vesselType = VesselType.Square;
    private float hitpoints = 1;
    private float maxHitpoints = 1;
    private List<VesselPart> vesselParts = new List<VesselPart>();

    public static List<Vessel> GetVesselsByDesignation(List<string> designations)
    {
        List<Vessel> vessels = new List<Vessel>();
        foreach (string designation in designations)
        {
            vessels.AddRange(vesselDict[designation]);
        }
        return vessels;
    }

    public static string BuildVessel(string spriteName, float newSize, float newDurability, string newDesignation, List<VesselPart> parts)
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

    private void BuildParts(List<VesselPart> parts, string designation)
    {
        foreach(VesselPart part in parts)
        {
            GameObject newPart = new GameObject(designation + part.partType.ToString());

            SpriteRenderer newSpriteRenderer = newPart.AddComponent<SpriteRenderer>();
            if (part.partType == VesselPartType.Bay)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("square");
            }
            else if (part.partType == VesselPartType.Engine)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("circle");
            }
            else if (part.partType == VesselPartType.Launcher)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("triangle");
            }

            newPart.transform.parent = gameObject.transform;

            SpriteRenderer vesselSprite = gameObject.GetComponent<SpriteRenderer>();
            newPart.transform.localPosition = new Vector3(part.position.x * vesselSprite.size.x * 0.5f, part.position.y * vesselSprite.size.y * 0.5f, -1);
            newPart.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            newPart.GetComponent<SpriteRenderer>().color = new Color(part.quality, part.quality, part.quality);

            vesselParts.Add(part);
        }
    }

    public void Initiate(VesselType newVesselType, float newSize, float newDurability, string newDesignation, List<VesselPart> parts)
    {
        vesselType = newVesselType;
        transform.localScale = new Vector3(newSize, newSize, newSize);
        hitpoints = newSize * newSize * newDurability;
        maxHitpoints = hitpoints;

        BuildParts(parts, newDesignation);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
