using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeInteractableUIObject : MonoBehaviour
{
    public RecipeScriptableObject recipe;
    public bool interactable = true;

    public void Clickable(bool b)
    {
        interactable = b;
    }
}
