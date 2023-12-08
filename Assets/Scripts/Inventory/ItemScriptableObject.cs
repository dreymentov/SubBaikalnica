using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="item", menuName ="ScriptableObject/itemScriptableObject",order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public int id;
    public itemCategories itemType;
    public string itemName;
    public Vector2 size; //how many across x and how many across y
    public Sprite sprite;
    public GameObject worldPrefab;

    public enum itemCategories
    {
        Generic, Tool, SmallFauna, Placeable, Equiptable
    }
}
