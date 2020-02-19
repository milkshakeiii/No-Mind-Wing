using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    static PlayerManager instance;

    public TMPro.TextMeshProUGUI buildStockpileText;
    public TMPro.TextMeshProUGUI moveStockpileText;
    public TMPro.TextMeshProUGUI launchStockpileText;

    private List<Vessel> selection = new List<Vessel>();
    private List<string> teamBuildStrings = new List<string>();

    private Dictionary<ResourceType, float> stockpiles = new Dictionary<ResourceType, float>();

    public void AddToTeam(string buildstring)
    {
        teamBuildStrings.Add(buildstring);
    }

    public bool AddResource(float amount, ResourceType resource)
    {
        float stockpile = 0;
        stockpiles.TryGetValue(resource, out stockpile);
        if (stockpile + amount < 0)
            return false;
        stockpile += amount;
        stockpiles[resource] = stockpile;
        return true;
    }

    public float GetResourceStockpile(ResourceType resource)
    {
        float stockpile = 0;
        stockpiles.TryGetValue(resource, out stockpile);
        return stockpile;
    }

    public static PlayerManager Instance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        StartCoroutine(Blink());

        stockpiles[ResourceType.Build] = 1f;
        stockpiles[ResourceType.Launch] = 1f;
        stockpiles[ResourceType.Move] = 1f;
    }

    private IEnumerator Blink()
    {
        Dictionary<Vessel, Color> vesselToColor = new Dictionary<Vessel, Color>();
        while (true)
        {
            foreach (Vessel vessel in selection)
            {
                vesselToColor[vessel] = vessel.gameObject.GetComponent<SpriteRenderer>().color;
                vessel.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            yield return new WaitForSeconds(0.3f);
            foreach (Vessel vessel in selection)
            {
                if (vesselToColor.ContainsKey(vessel))
                    vessel.gameObject.GetComponent<SpriteRenderer>().color = vesselToColor[vessel];
            }
            yield return new WaitForSeconds(1.7f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        buildStockpileText.text = "<color=#FFFFFF>" + GetResourceStockpile(ResourceType.Build) + "</color>";
        moveStockpileText.text = "<color=#10FF10>" + GetResourceStockpile(ResourceType.Move) + "</color>";
        launchStockpileText.text = "<color=#FF7F00>" + GetResourceStockpile(ResourceType.Launch) + "</color>";
    }

    public List<Vessel> GetSelection()
    {
        return selection;
    }

    public void Select(List<string> designations)
    {
        selection = VesselManager.Instance().GetVesselsByDesignation(designations);
    }

    public void AddToSelection(List<string> designations)
    {
        selection.AddRange(VesselManager.Instance().GetVesselsByDesignation(designations));
    }

    public void RemoveFromSelection(Vessel vessel)
    {
        selection.Remove(vessel);
    }
}
