using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bay : MonoBehaviour
{
    private List<GameObject> currentlyHarvestable = new List<GameObject>();
    private float gatheringWarmup;
    private float gatheringMax;

    private float gatheringRate = 0f;
    private const float gatheringWarmupFactor = 0.1f;
    private const float gatheringMaxFactor = 1f;

    public void Initiate(float size, float quality1, float quality2)
    {
        gatheringWarmup = quality1;
        gatheringMax = quality2;
        gameObject.transform.localScale = new Vector3(size, size, size);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Build") ||
            collision.tag.Equals("Move") ||
            collision.tag.Equals("Launch"))
        {
            currentlyHarvestable.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentlyHarvestable.Remove(collision.gameObject);
        if (currentlyHarvestable.Count == 0)
        {
            gatheringRate = 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyHarvestable.Count > 0)
        {
            gatheringRate = Mathf.Min(gatheringMax * gatheringMaxFactor,
                                      gatheringRate + Time.deltaTime * gatheringWarmup * gatheringWarmupFactor);
            GameObject gatheringTarget = currentlyHarvestable[0];
            float gatheredAmount = gatheringRate * Time.deltaTime;
            ResourceType gatheringType;
            if (gatheringTarget.tag.Equals("Build"))
                gatheringType = ResourceType.Build;
            else if (gatheringTarget.tag.Equals("Move"))
                gatheringType = ResourceType.Move;
            else //gatheringTarget.tag.Equals("Launch")
                gatheringType = ResourceType.Launch;
            PlayerManager.Instance().AddResource(gatheredAmount, gatheringType);
            gatheringTarget.transform.localScale = gatheringTarget.transform.localScale -
                                                   Vector3.one * gatheredAmount;
            if (gatheringTarget.transform.localScale.x < 0)
            {
                currentlyHarvestable.Remove(gatheringTarget);
                Destroy(gatheringTarget);
            }
        }
    }
}
