using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ToolBaseClass
{
    public int damage = 5;

    internal override void Awake()
    {
        toolAction = Attack;
    }

    void Attack()
    {
        //check if hit something
        if(pc.currentHoverObject != null)
        {
            //Check if we hit something with a Health component
            Health targetHealth = pc.currentHoverObject.GetComponent<Health>();
            if(targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }
}
