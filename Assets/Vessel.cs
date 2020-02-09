using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
    private VesselType vesselType = VesselType.Square;
    private float hitpoints = 1;
    private float maxHitpoints = 1;
    private List<VesselPart> vesselParts = new List<VesselPart>();

    private void BuildParts(List<VesselPart> parts, string designation)
    {
        foreach(VesselPart part in parts)
        {
            GameObject newPart = new GameObject(designation + part.partType.ToString());

            SpriteRenderer newSpriteRenderer = newPart.AddComponent<SpriteRenderer>();
            if (part.partType == VesselPartType.Bay)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("square");
                newPart.AddComponent<Bay>().Initiate(part.size, part.quality1, part.quality2);
                newPart.AddComponent<BoxCollider2D>();
                Rigidbody2D newRigidbody = newPart.AddComponent<UnityEngine.Rigidbody2D>();
                newRigidbody.gravityScale = 0;
                newPart.layer = 8; //Resource
            }
            else if (part.partType == VesselPartType.Engine)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("circle");
                newPart.AddComponent<Bay>().Initiate(part.size, part.quality1, part.quality2);
            }
            else if (part.partType == VesselPartType.Launcher)
            {
                newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("triangle");
            }

            newPart.transform.parent = gameObject.transform;

            SpriteRenderer vesselSprite = gameObject.GetComponent<SpriteRenderer>();
            newPart.transform.localPosition = new Vector3(part.position.x * vesselSprite.size.x * 0.5f, part.position.y * vesselSprite.size.y * 0.5f, -1);
            newPart.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            newPart.transform.localEulerAngles = new Vector3(0, 0, part.facing);
            float quality = (part.quality1 + part.quality2) / 2;
            newPart.GetComponent<SpriteRenderer>().color = new Color(quality, quality, quality);

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
