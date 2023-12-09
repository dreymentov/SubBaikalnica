using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeInteractableUIObject : MonoBehaviour
{
    public RecipeScriptableObject recipe;
    public bool interactable = true;

    public GameObject popUp;
    public TextMeshProUGUI recipeName;
    public GameObject ingredientUIPrefab;

    public void Clickable(bool b)
    {
        interactable = b;
    }

    public void SetUpButton(RecipeScriptableObject recipe)
    {
        //set up sprite and name
        GetComponent<Image>().sprite = recipe.craftedItem.sprite;
        recipeName.text = recipe.craftedItem.name;

        //spawn inb the ingredients needed in the popup
        Transform ingredientsList = popUp.transform.GetChild(1);
        foreach(ItemScriptableObject ingredient in recipe.ingredients)
        {
            GameObject currentIngredient = Instantiate(ingredientUIPrefab, ingredientsList);

            currentIngredient.GetComponentInChildren<Image>().sprite = ingredient.sprite;
        }
    }

    public void HoverEnter()
    {
        popUp.SetActive(true);
    }

    public void HoverExit()
    {
        popUp.SetActive(false);
    }
}
