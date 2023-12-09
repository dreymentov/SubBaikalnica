using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GearUniversalFunctions : MonoBehaviour
{
    public static PlayerController pc;
    public static InventorySystem iSystem;
    [System.Serializable]
    public delegate void GearFunction(bool i);

    public GearScriptableObject[] gearTypes;
    public List<GearFunction> gearFunctions;

    private void Awake()
    {
        //set up geartypes
        gearTypes = Resources.LoadAll<GearScriptableObject>("Scriptable Objects/Gear").ToArray();

        gearFunctions = new List<GearFunction> { ActivateFlippers, DefaultGearFunction, DefaultGearFunction, ActivateStillSuit, DefaultGearFunction };
    
        iSystem = InventorySystem.Instance;

        //still suit setup
        iSystem.OnInventoryUpdated += AddStoredWaterToInventory;
    }

    //function that adds stuff to inventory event system based on whos active
    //call this function whenever we set gear to update the activated gear
    public void ActivateGearFunctions()
    {
        //deactivate all functions first
        for (int i=0;i<gearTypes.Count();i++)
        {
            gearFunctions[i](false);
        }

        foreach (GearScriptableObject gearScriptable in iSystem.gearSlots)
        {
            if(gearScriptable == null)
            {
                return;
            }

            gearFunctions[System.Array.IndexOf(gearTypes,gearScriptable)](true);
        }
    }

    private void Update()
    {
        //based on the bools activated, activate different fucntions
        if (stillsuitActive)
        {
            ProduceWater();

        }
    }

    void DefaultGearFunction(bool i)
    {
        return;
    }

    #region StillSuit
    [Header("Still Suit")]
    public ItemScriptableObject waterItem;
    bool storedWater;
    float timer;
    public float waterGenerationRate;
    public bool stillsuitActive;

    private void ProduceWater()
    {

        //if theres water stored, do not do timer and search for way to get rid of water
        if (!storedWater)
        {
            //timer for creating water
            if (timer >= waterGenerationRate)
            {
                if (iSystem.AddItem(waterItem))
                {
                    timer = 0;
                    storedWater = false;
                }
                else
                {
                    storedWater = true;
                }
            }

            timer += Time.deltaTime;
        }

    }

    void AddStoredWaterToInventory()
    {
        if (storedWater)
        {
            //if put in inventory, unstore water
            if (iSystem.AddItem(waterItem))
            {
                timer = 0;
                storedWater = false;
            }
        }
    }

    void ActivateStillSuit(bool i)
    {
        Debug.Log("stillsuit activated");
        if (i)
        { 
            stillsuitActive = true;
        }
        else
        {
            stillsuitActive = false;
        }
    }
    #endregion

    #region Flippers

    [Header("Flippers")]
    public float flipperSpeedBonus;
    void ActivateFlippers(bool i)
    {
        if (i)
        {
            pc.SwimBonuses.Add(flipperSpeedBonus);
        }
        else
        {
            
            if (!pc.SwimBonuses.Contains(flipperSpeedBonus))
            {
                return;
            }

            pc.SwimBonuses.Remove(flipperSpeedBonus);
        }
    }
    #endregion
}
