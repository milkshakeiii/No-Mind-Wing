using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private float thrustWarmup;
    private bool isOn = false;

    private const float maxThrustFactor = 1f;
    private const float thrustWarmupFactor = 0.1f;

    private float GetMaxThrust()
    {
        return gameObject.transform.localScale.x * maxThrustFactor;
    }

    public void Initiate(float size, float quality1)
    {
        thrustWarmup = quality1;
        gameObject.transform.localScale = new Vector3(size, size, size);
    }

    public void TurnOn()
    {
        isOn = true;
    }

    public void TurnOff()
    {
        isOn = false;
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
