using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VesselType
{
    Square = 1,
    Round = 2,
    Jagged = 3
}

public class Vessel : MonoBehaviour
{
    private static Dictionary<string, List<Vessel>> vesselDict = new Dictionary<string, List<Vessel>>();

    private VesselType vesselType = VesselType.Square;
    private float hitpoints = 1;
    private float maxHitpoints = 1;

    public static void BuildVessel(string spriteName, float newSize, float newDurability, string newDesignation)
    {
        GameObject newVessel = new GameObject(newDesignation);

        SpriteRenderer newSpriteRenderer = newVessel.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName(spriteName);
        VesselType newVesselType = SpriteManager.Instance().VesselTypeFromName(spriteName);

        Rigidbody2D newRigidbody = newVessel.AddComponent<UnityEngine.Rigidbody2D>();
        newRigidbody.gravityScale = 0;

        Vessel newVesselScript = newVessel.AddComponent<Vessel>();
        newVesselScript.Initiate(newVesselType, newSize, newDurability, newDesignation);

        if (!vesselDict.ContainsKey(newDesignation))
        {
            vesselDict[newDesignation] = new List<Vessel>();
        }
        vesselDict[newDesignation].Add(newVesselScript);
    }

    public void Initiate(VesselType newVesselType, float newSize, float newDurability, string newDesignation)
    {
        vesselType = newVesselType;
        transform.localScale = new Vector3(newSize, newSize, newSize);
        hitpoints = newSize * newSize * newDurability;
        maxHitpoints = hitpoints;
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
