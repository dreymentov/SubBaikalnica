using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CraftingManager : UIBaseClass
{
    InventorySystem iSystem;
    public static CraftingManager Instance;

    public List<RecipeScriptableObject> unlockedRecipes = new List<RecipeScriptableObject>();
    public List<Button> recipeButtons = new List<Button>();

    public InteractableObject currentTable;
    public List<Transform> categories;

    public GameObject panel;

    public GameObject menuButtonPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        iSystem = InventorySystem.Instance;

        InitialiseCraftingTableUI();
    }

    #region UI Functions
    //function to set the current crafting table
    public void SetTable(InteractableObject currentTable)
    {
        this.currentTable = currentTable;
    }

    //unlock and set up a recipe
    public void UnlockRecipe(RecipeScriptableObject recipe)
    {
        unlockedRecipes.Add(recipe);
        unlockedRecipes = unlockedRecipes.OrderBy(recipe => recipe.category).ToList<RecipeScriptableObject>();

        //set up the button
        SetUpButton(recipe);
    }

    //creates a button for the recipe
    public void SetUpButton(RecipeScriptableObject recipe)
    {
        Button currentButton = Instantiate(menuButtonPrefab, menu.transform.GetChild(recipe.category).GetChild(1)).GetComponent<Button>();
        recipeButtons.Add(currentButton);
        //set up the button and it's pop up
        currentButton.GetComponent<RecipeInteractableUIObject>().SetUpButton(recipe);

        //add function to button
        currentButton.onClick.AddListener(() => CraftObjectFunc(recipe));
    }

    //initialise the crafting list
    public void InitialiseCraftingTableUI()
    {
        foreach(RecipeScriptableObject recipe in unlockedRecipes)
        {
            SetUpButton(recipe);
        }
    }

    //override the open menu functions
    public override void OpenMenuFunctions()
    {
        //turn on panel to close menu
        panel.SetActive(true);

        //check whats craftable and enable buttons
        for(int i=0; i < recipeButtons.Count; i++)
        {
            //set up button based on ingredient availability
            recipeButtons[i].interactable = CheckIngredients(unlockedRecipes[i].ingredients);
        }
    }

    public override void CloseMenuFunctions()
    {
        //turn off the panel
        panel.SetActive(false);

        //reset the table
        ResetCraftingTable();
    }

    public void ResetCraftingTable()
    {
        foreach(Transform category in categories)
        {
            category.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void ToggleCategory(GameObject buttonHolder)
    {
        if (!buttonHolder.activeInHierarchy)
        {
            ResetCraftingTable();
            buttonHolder.SetActive(true);
        }
        else
        {
            ResetCraftingTable();
        }
    }

    #endregion

    #region Crafting Functions

    public void CraftObjectFunc(RecipeScriptableObject recipe)
    {
        StartCoroutine(CraftObject(recipe));
    }

    public IEnumerator CraftObject(RecipeScriptableObject recipe)
    {
        //disable the crafting table
        currentTable.Clickable(false);

        //close the menu
        ToggleMenu();

        //remove items from Inventory
        iSystem.RemoveItem(recipe.ingredients);

        //wait for the crafting time
        yield return new WaitForSeconds(recipe.cookingTime);

        //create the item on top of the table
        GameObject craftedItem = Instantiate(recipe.craftedItem.worldPrefab, currentTable.transform.GetChild(0).position, Quaternion.identity, currentTable.transform);
        craftedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //enable the crafting table
        currentTable.Clickable(true);
    }

    //check if have ingredients for a recipe
    public bool CheckIngredients(List<ItemScriptableObject> ingredients)
    {
        //make a temp list
        List<ItemScriptableObject> tempList = new List<ItemScriptableObject>(iSystem.itemsList);

        //check if the inventory list has all of the ingredients
        for(int i = 0; i < ingredients.Count; i++)
        {
            if (!tempList.Contains(ingredients[i]))
            {
                //if the item is missing, the recipe is invalid
                return false;
            }
            else
            {
                //checking for multiple of the same item
                tempList.Remove(ingredients[i]);
            }
        }

        //if all of the ingredients are available, return true
        return true;
    }
    #endregion
}
