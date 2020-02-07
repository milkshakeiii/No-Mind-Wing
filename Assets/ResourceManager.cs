using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Build = 1,
    Fuel = 2,
    Launch = 3
}

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;

    public static ResourceManager Instance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        LayOutResources(4, 3, 2, 5f, 10f, 0.5f, 1f);
    }

    public void LayOutResources(int buildPatches, 
                                int fuelPatches,
                                int launchPatches,
                                float height,
                                float width,
                                float minSize,
                                float maxSize)
    {
        for (int i = 0; i < buildPatches+fuelPatches+launchPatches; i++)
        {
            ResourceType resourceType;
            if (i < buildPatches)
                resourceType = ResourceType.Build;
            else if (i < buildPatches + fuelPatches)
                resourceType = ResourceType.Fuel;
            else
                resourceType = ResourceType.Launch;
            Vector2 position = new Vector2(Random.Range(-width/2, width/2), Random.Range(0, -height));
            float size = Random.Range(minSize, maxSize);
            BuildResource(resourceType, position, size);
            BuildResource(resourceType, new Vector3(position.x, -position.y), size);
        }
    }

    public void BuildResource(ResourceType resourceType, Vector2 position, float size)
    {
        GameObject newResource = new GameObject(resourceType.ToString());
        newResource.transform.parent = gameObject.transform;
        newResource.transform.localScale = new Vector3(size, size, size);
        newResource.layer = 8; //Resource
        newResource.transform.position = position;

        SpriteRenderer newSpriteRenderer = newResource.AddComponent<SpriteRenderer>();
        newSpriteRenderer.sprite = SpriteManager.Instance().SpriteFromName("square");

        //Collider2D newCollider = 
        newResource.AddComponent<BoxCollider2D>();

        if (resourceType == ResourceType.Build)
        {
            newSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        if (resourceType == ResourceType.Fuel)
        {
            newSpriteRenderer.color = new Color(0, 1, 0, 0.5f);
        }
        if (resourceType == ResourceType.Launch)
        {
            newSpriteRenderer.color = new Color(1f, 0.647f, 0, 0.5f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
