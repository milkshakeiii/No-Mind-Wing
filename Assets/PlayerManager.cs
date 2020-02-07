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
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            foreach (Vessel vessel in selection)
            {
                vessel.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            yield return new WaitForSeconds(0.3f);
            foreach (Vessel vessel in selection)
            {
                vessel.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            yield return new WaitForSeconds(1.7f);
        }
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
        selection = VesselManager.Instance().GetVesselsByDesignation(designations);
    }

    public void AddToSelection(List<string> designations)
    {
        selection.AddRange(VesselManager.Instance().GetVesselsByDesignation(designations));
    }
}
