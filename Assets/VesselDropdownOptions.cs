using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselDropdownOptions : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;

    void Start()
    {
        Dictionary<string, string> namesToBuildstrings = GameStringsHelper.AllSavedNamesToBuildstrings();
        dropdown.AddOptions(new List<string>(namesToBuildstrings.Keys));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
