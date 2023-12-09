using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "ScriptableObject/gearScriptableObject", order = 2)]
public class GearScriptableObject : ItemScriptableObject
{
    public enum GearCategories
    { 
        Body, 
        Helmet, 
        Flipper,
        Tank,
        Upgrade
    }

    public GearCategories gearType;

    private void OnValidate()
    {
        switch (gearType)
        {
            case GearCategories.Body:
                size = new Vector2(3, 3);
                break;

            case GearCategories.Helmet:
                size = new Vector2(2, 2);

                break;

            case GearCategories.Flipper:
                size = new Vector2(2, 2);

                break;

            case GearCategories.Tank:
                size = new Vector2(2, 3);

                break;

            case GearCategories.Upgrade:
                size = new Vector2(1, 1);

                break;
        }
    }

    //set default value of gearType
    void Reset()
    {
        itemType = itemCategories.Gear;

    }
}
