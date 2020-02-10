using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private float launchVelocity;
    private float launchRange;

    const float launchVelocityFactor = 1f;
    const float launchRangeFactor = 2f;

    public float ProjectileSize()
    {
        return gameObject.transform.localScale.x;
    }

    public float LaunchVelocity()
    {
        return launchVelocity * launchVelocityFactor;
    }

    public float LaunchRange()
    {
        return launchRange * launchRangeFactor;
    }

    public void Initiate(float size, float quality1, float quality2)
    {
        launchVelocity = quality1;
        launchRange = quality2;
    }

    public void Fire()
    {
        ProjectileManager.Instance().Fire(this);
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
