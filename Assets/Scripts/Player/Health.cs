using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public delegate void DieAction();
    public DieAction die;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        die = DefaultDie;
    }

    public void RestoreHealth(int restoreAmount)
    {
        //make sure health cannot exceed max
        currentHealth = Mathf.Min(currentHealth += restoreAmount, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        CheckDead();
    }

    public void CheckDead()
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            //die
            die();
        }
    }

    public void DefaultDie()
    {
        //get destroyed
        Debug.Log(name + " died!");
        Destroy(gameObject);
    }
}
