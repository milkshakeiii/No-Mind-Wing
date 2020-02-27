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

    private Dictionary<string, string> vesselNamesToBuildstrings = new Dictionary<string, string>();

    private static VesselManager instance;

    public static VesselManager Instance()
    {
        return instance;
    }

    public Vessel BuildVessel(bool requireSource,
                              List<Vessel> sourceVessels,
                              string vesselName,
                              string designation)
    {
        Debug.Log("build vessel: " + vesselName);
        string buildString = vesselNamesToBuildstrings[vesselName];
        return GameStringsHelper.PerformBuildFromString(requireSource, buildString, designation, sourceVessels);
    }

    public Vessel BuildVessel(bool requireSource,
                              List<Vessel> sourceVessels,
                              string spriteName,
                              float newSize,
                              float newDurability,
                              string newDesignation,
                              List<VesselPart> parts)
    {
        if (requireSource && sourceVessels.Count == 0)
        {
            throw new UnityException("Tried to build from source vessel with no source vessels");
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
            //"No selected vessel has a large enough bay";
            return null;
        }

        if (!SpriteManager.Instance().SpriteNameIsGood(spriteName))
        {
            throw new UnityException("Tried to build vessel with invalid sprite name: " + spriteName);
        }

        VesselType newVesselType = SpriteManager.Instance().VesselTypeFromName(spriteName);
        float cost = VesselCost(newVesselType, newSize, newDurability, parts);
        float stockpile = PlayerManager.Instance().GetResourceStockpile(ResourceType.Build);
        if (requireSource && (cost > stockpile))
        {
            // cost.ToString() + " is required but only " + stockpile.ToString() + " is stockpiled.";
            return null;
        }

        GameObject newVessel = new GameObject(newDesignation);
        newVessel.transform.position = buildLocation;

        SpriteRenderer newSpriteRenderer = newVessel.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName(spriteName);

        Rigidbody2D newRigidbody = newVessel.AddComponent<UnityEngine.Rigidbody2D>();
        newRigidbody.gravityScale = 0;
        newRigidbody.useAutoMass = true;
        newRigidbody.drag = 1f;
        newRigidbody.angularDrag = 1f;

        //Collider2D newCollider = 
        newVessel.AddComponent<PolygonCollider2D>();

        Vessel newVesselScript = newVessel.AddComponent<Vessel>();
        newVesselScript.Initiate(newVesselType, newSize, newDurability, newDesignation, parts);

        PlayerManager.Instance().AddPlayerVessel(newVesselScript);
        vesselMindUpdateQueue.Enqueue(newVesselScript);
        if (!mindsByDesignation.ContainsKey(newDesignation))
            mindsByDesignation[newDesignation] = new Mind();

        if(requireSource && !PlayerManager.Instance().AddResource(-cost, ResourceType.Build))
            throw new UnityException ("Problem: I thought there were enough resources but AddResource returned false");
        return newVesselScript;
    }

    private float VesselCost(VesselType type, float size, float durability, List<VesselPart> parts)
    {
        return 0.1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        vesselNamesToBuildstrings = GameStringsHelper.AllSavedNamesToBuildstrings();
    }

    public Mind GetMind(string designation)
    {
        return mindsByDesignation[designation];
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
    private Dictionary<int, List<Behavior>> modeToPrioritizedBehaviorList = new Dictionary<int, List<Behavior>>()
    {
        { 0, new List<Behavior>() },
        { 1, new List<Behavior>() },
        { 2, new List<Behavior>() },
    };
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

    public void SetMode(int newMode)
    {
        if (!modeToPrioritizedBehaviorList.ContainsKey(newMode))
        {
            throw new UnityException("Invalid mind mode: " + newMode.ToString());
        }
        currentMode = newMode;
    }

    public void AddBehavior(int mode, Behavior behavior)
    {
        modeToPrioritizedBehaviorList[mode].Add(behavior);
    }
}

public abstract class Behavior : Object
{
    private float argument;

    public void SetArgument(float newArgument)
    {
        argument = newArgument;
    }

    private float GetArgument()
    {
        return argument;
    }

    public abstract bool ChooseActionOrPass(Vessel actor); //true to choose, false to pass
}

public abstract class HarvestBehavior : Behavior
{
    public override bool ChooseActionOrPass(Vessel actor)
    {
        return true;
    }
}

public abstract class AttackBehavior : Behavior
{
    public const float shootAccuracyTolerance = 10f;

    public override bool ChooseActionOrPass(Vessel actor)
    {
        Vector2 positionToAttack = ChoosePositionToAttack(actor);

        List<Launcher> launchers = actor.GetLaunchers();
        for (int i = 0; i < launchers.Count; i++)
        {
            Launcher launcher = launchers[i];
            Vector2 launcherPosition = launcher.transform.position;
            Vector2 differenceVectorToTarget = (positionToAttack - launcherPosition);
            if (differenceVectorToTarget.magnitude > launcher.LaunchRange())
            {
                continue;
            }

            float facingToTarget = Mathf.Acos(differenceVectorToTarget.y / differenceVectorToTarget.x) *
                                   Mathf.Rad2Deg;
            float myFacing = launcher.transform.eulerAngles.z;
            float offBy = Mathf.Abs(facingToTarget - myFacing);
            if (offBy < shootAccuracyTolerance)
            {
                actor.Fire(i);
                return true;
            }
        }
        return false;
    }

    public abstract Vector2 ChoosePositionToAttack(Vessel actor);
}

public abstract class MoveBehavior : Behavior
{
    public override bool ChooseActionOrPass(Vessel actor)
    {
        return false;
    }

    public abstract Vector2 ChoosePositionToMoveTo(Vessel actor);
}

public class MoveTowardPlayerKingBehavior : MoveBehavior
{

    public override Vector2 ChoosePositionToMoveTo(Vessel actor)
    {
        return PlayerManager.Instance().GetKing().transform.position;
    }
}

public class AttackPlayerKingBehavior : AttackBehavior
{
    public override Vector2 ChoosePositionToAttack(Vessel actor)
    {
        return PlayerManager.Instance().GetKing().transform.position;
    }
}