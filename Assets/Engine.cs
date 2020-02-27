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

    public float GetMaxThrust()
    {
        return gameObject.transform.localScale.x * maxThrustFactor;
    }

    public float GetThrustWarmupPerSecond()
    {
        return thrustWarmup * thrustWarmupFactor;
    }

    public void Initiate(float size, float quality1, float quality2)
    {
        thrustWarmup = quality1;
        maxSpeed = quality2;
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
            currentThrust = Mathf.Min(currentThrust + GetThrustWarmupPerSecond() * Time.deltaTime,
                                      GetMaxThrust());
            Vector2 force = currentThrust * gameObject.transform.up;

            if (Mathf.Abs(targetRigidbody.angularVelocity) < absoluteMaxAngularVelocity &&
                targetRigidbody.velocity.sqrMagnitude < maxSpeed*maxSpeed*maxSpeedFactor*maxSpeedFactor)
            {
//                Debug.Log(currentThrust);
                targetRigidbody.AddForceAtPosition(force, gameObject.transform.position);
            }
//            else
//            {
//                if (!(Mathf.Abs(targetRigidbody.angularVelocity) < absoluteMaxAngularVelocity))
//                    Debug.Log("angular velocity exceeded");
//                if (!(targetRigidbody.velocity.sqrMagnitude < maxSpeed * maxSpeed * maxSpeedFactor * maxSpeedFactor))
//                    Debug.Log("max speed exceeded");
//            }
        }
        else
        {
            currentThrust = 0f;
        }
    }
}
