using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    static PlayerManager instance;

    private List<Vessel> selection = new List<Vessel>();

    public static PlayerManager Instance()
    {
        return instance;
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

    public List<Vessel> GetSelection()
    {
        return selection;
    }

    public void Select(List<string> designations)
    {
        selection = Vessel.GetVesselsByDesignation(designations);
    }

    public void AddToSelection(List<string> designations)
    {
        selection.AddRange(Vessel.GetVesselsByDesignation(designations));
    }
}
