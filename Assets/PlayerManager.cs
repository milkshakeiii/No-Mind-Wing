using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    static PlayerManager instance;

    public TMPro.TextMeshProUGUI buildStockpileText;
    public TMPro.TextMeshProUGUI moveStockpileText;
    public TMPro.TextMeshProUGUI launchStockpileText;

    private Vessel king;
    private List<string> teamBuildStrings = new List<string>();
    private Dictionary<string, List<Vessel>> vesselsByDesignation = new Dictionary<string, List<Vessel>>();
    private Dictionary<ResourceType, float> stockpiles = new Dictionary<ResourceType, float>();

    private UnityEngine.KeyCode[] engineKeyCodes = new UnityEngine.KeyCode[9]
    {
        UnityEngine.KeyCode.A,
        UnityEngine.KeyCode.S,
        UnityEngine.KeyCode.D,
        UnityEngine.KeyCode.W,
        UnityEngine.KeyCode.Q,
        UnityEngine.KeyCode.E,
        UnityEngine.KeyCode.Z,
        UnityEngine.KeyCode.X,
        UnityEngine.KeyCode.C,
    };

    private UnityEngine.KeyCode[] launcherKeyCodes = new UnityEngine.KeyCode[9]
    {
        UnityEngine.KeyCode.Space,
        UnityEngine.KeyCode.F,
        UnityEngine.KeyCode.G,
        UnityEngine.KeyCode.V,
        UnityEngine.KeyCode.B,
        UnityEngine.KeyCode.R,
        UnityEngine.KeyCode.T,
        UnityEngine.KeyCode.LeftControl,
        UnityEngine.KeyCode.Tab,
    };

    private UnityEngine.KeyCode[] bayKeyCodes = new UnityEngine.KeyCode[9]
    {
        UnityEngine.KeyCode.Alpha1,
        UnityEngine.KeyCode.Alpha2,
        UnityEngine.KeyCode.Alpha3,
        UnityEngine.KeyCode.Alpha4,
        UnityEngine.KeyCode.Alpha5,
        UnityEngine.KeyCode.Alpha6,
        UnityEngine.KeyCode.Alpha7,
        UnityEngine.KeyCode.Alpha8,
        UnityEngine.KeyCode.Alpha9,
    };

    public void SpawnKing(string spriteName, List<VesselPart> parts)
    {
        king = VesselManager.Instance().BuildVessel(false, new List<Vessel>(), spriteName, 1f, 1f, "king", parts);
    }

    public void SpawnKing(string vesselName)
    {
        king = VesselManager.Instance().BuildVessel(false, new List<Vessel>(), vesselName, "king");
    }

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

        stockpiles[ResourceType.Build] = 1f;
        stockpiles[ResourceType.Launch] = 1f;
        stockpiles[ResourceType.Move] = 1f;
    }

    public void AddPlayerVessel(Vessel vessel)
    {
        if (vesselsByDesignation.ContainsKey(vessel.GetDesignation()))
        {
            vesselsByDesignation[vessel.GetDesignation()].Add(vessel);
        }
        else
        {
            vesselsByDesignation[vessel.GetDesignation()] = new List<Vessel>() {vessel};
        }
    }

    public void VesselDestroyed(Vessel vessel)
    {
        vesselsByDesignation[vessel.GetDesignation()].Remove(vessel);
    }

    private void UpdateStockpileText()
    {
        buildStockpileText.text = "<color=#FFFFFF>" + GetResourceStockpile(ResourceType.Build) + "</color>";
        moveStockpileText.text = "<color=#10FF10>" + GetResourceStockpile(ResourceType.Move) + "</color>";
        launchStockpileText.text = "<color=#FF7F00>" + GetResourceStockpile(ResourceType.Launch) + "</color>";
    }

    private void KeyboardStuff()
    {
        if (king == null)
            return;

        for (int i = 0; i < engineKeyCodes.Length; i++)
        {
            UnityEngine.KeyCode keyCode = engineKeyCodes[i];
            if (UnityEngine.Input.GetKeyDown(keyCode))
            {
                king.IgniteEngines(new int[1] { i });
            }
            if (UnityEngine.Input.GetKeyUp(keyCode))
            {
                king.QuenchEngines(new int[1] { i });
            }
        }
        for (int i = 0; i < launcherKeyCodes.Length; i++)
        {
            UnityEngine.KeyCode keyCode = launcherKeyCodes[i];
            if (UnityEngine.Input.GetKeyDown(keyCode))
            {
                king.Fire(new int[1] { i });
            }
        }
    }

    void Update ()
    {
        UpdateStockpileText();

        KeyboardStuff();
    }
}
