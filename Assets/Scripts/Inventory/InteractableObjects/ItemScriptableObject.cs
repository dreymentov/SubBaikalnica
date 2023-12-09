using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="item", menuName ="ScriptableObject/itemScriptableObject",order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public int id;
    public itemCategories itemType;
    public Vector2 size = Vector2.one; //how many across x and how many across y
    public Sprite sprite;
    public GameObject worldPrefab;
    [TextArea(15, 20)]
    public string description;

    //scanning
    public bool scannable;
    public float scanPercentage = 0;
    public float scanRate = 10;

    public enum itemCategories
    {
        Generic, Tool, SmallFauna, Placeable, Equiptable, Gear
    }
}
