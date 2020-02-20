using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTerminal : MonoBehaviour
{
    public GameObject terminal;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            terminal.SetActive(!terminal.activeSelf);
        }
    }
}
