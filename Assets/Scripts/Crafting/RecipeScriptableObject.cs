using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "recipe", menuName = "ScriptableObject/recipeScriptableObject", order = 2)]
public class RecipeScriptableObject : ScriptableObject
{
    public List<ItemScriptableObject> ingredients;
    public ItemScriptableObject craftedItem;
    public float cookingTime = 2f;
    public int category;
}
