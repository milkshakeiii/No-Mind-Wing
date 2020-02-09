using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private float thrustWarmup;
    private float maxSpeed;
    private float currentThrust = 0f;
    private bool isOn = false;

    private Rigidbody2D targetRigidbody;

    private const float maxThrustFactor = 1f;
    private const float thrustWarmupFactor = 0.1f;
    private const float maxSpeedFactor = 10f;
    private const float absoluteMaxAngularVelocity = 1080f; //degrees per second

    private float GetMaxThrust()
    {
        return gameObject.transform.localScale.x * maxThrustFactor;
    }

    public void Initiate(float size, float quality1, float quality2)
    {
        thrustWarmup = quality1;
        maxSpeed = quality2;
        gameObject.transform.localScale = new Vector3(size, size, size);
        targetRigidbody = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
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
        if (isOn)
        {
            currentThrust = Mathf.Min(currentThrust + thrustWarmup * thrustWarmupFactor * Time.deltaTime,
                                      GetMaxThrust());
            Vector2 force = currentThrust * gameObject.transform.forward;

            if (targetRigidbody.angularVelocity < absoluteMaxAngularVelocity &&
                targetRigidbody.velocity.sqrMagnitude < maxSpeed*maxSpeed*maxSpeedFactor*maxSpeedFactor)
            {
                targetRigidbody.AddForceAtPosition(force, gameObject.transform.position);
            }
        }
        else
        {
            currentThrust = 0f;
        }
    }
}
