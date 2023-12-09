using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class CuttableInteractableObject : InteractableObject
{
    public GameObject spawningItem;
    public Health health;
    public int dropCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        health.die = Die;
    }

    public void Die()
    {
        Debug.Log("Spawn in " + dropCount + " " + spawningItem.name);
        //spawn in the item
        for (int i = 0; i < dropCount; i++)
        {
            Instantiate(spawningItem, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
