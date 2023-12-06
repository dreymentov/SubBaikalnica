using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/itemScriptableObject", order = 51)]
public class ItemScriptableObject : ScriptableObject
{
    public int id;
    public string itemName;
    public Vector2 size; //how many across x and how many across y
    public Sprite sprite;
    public GameObject WorldPrefab;
}
