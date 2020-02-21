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
    private Dictionary<string, Mind> mindsByDesignation = new Dictionary<string, Mind>();
    private Queue<Vessel> vesselMindUpdateQueue = new Queue<Vessel>();

    private Dictionary<string, string> namesToBuildstrings = new Dictionary<string, string>();

    private static VesselManager instance;

    public static VesselManager Instance()
    {
        return instance;
    }

    public string BuildVessel(bool requireSource,
                              List<Vessel> sourceVessels,
                              string spriteName,
                              float newSize,
                              float newDurability,
                              string newDesignation,
                              List<VesselPart> parts)
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
        newRigidbody.useAutoMass = true;
        newRigidbody.drag = 0.33f;

        //Collider2D newCollider = 
        newVessel.AddComponent<PolygonCollider2D>();

        Vessel newVesselScript = newVessel.AddComponent<Vessel>();
        newVesselScript.Initiate(newVesselType, newSize, newDurability, newDesignation, parts);

        PlayerManager.Instance().AddPlayerVessel(newVesselScript);
        vesselMindUpdateQueue.Enqueue(newVesselScript);

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

        namesToBuildstrings = SaveButton.AllSavedNamesToBuildstrings();
    }

    // Update is called once per frame
    void Update()
    {
        int vesselUpdatesThisFrame = Mathf.Min
        (
            Mathf.CeilToInt(vesselMindUpdateQueue.Count * Time.deltaTime),
            20
        );
        for (int i = 0; i < vesselUpdatesThisFrame; i++)
        {
            Vessel updateMe = vesselMindUpdateQueue.Dequeue();
            Mind mind = mindsByDesignation[updateMe.GetDesignation()];
            mind.ChooseAction(updateMe);
            vesselMindUpdateQueue.Enqueue(updateMe);
        }
    }
}

public class Mind : Object
{
    private Dictionary<int, List<Behavior>> modeToPrioritizedBehaviorList = new Dictionary<int, List<Behavior>>();
    private int currentMode = 0;
    public void ChooseAction(Vessel actor)
    {
        foreach (Behavior behavior in modeToPrioritizedBehaviorList[currentMode])
        {
            if (behavior.ChooseActionOrPass(actor))
            {
                return; //we chose an action
            }
        }
    }
}

public abstract class Behavior : Object
{
    public abstract bool ChooseActionOrPass(Vessel actor);
}

public abstract class HarvestBehavior : Behavior
{
    public override bool ChooseActionOrPass(Vessel actor)
    {
        return true;
    }
}