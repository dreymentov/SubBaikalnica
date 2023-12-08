using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CraftingManager : UIBaseClass
{
    public InventorySystem iSystem;
    public static CraftingManager Instance;

    public List<RecipeScriptableObject> unlockedRecipes = new List<RecipeScriptableObject>();
    public List<Button> recipeButtons = new List<Button>();

    public InteractableObject currentTable;
    public List<Transform> categories;

    public Transform spawnCoords;
    public GameObject panel;

    public GameObject menuButtonPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        //iSystem = InventorySystem.Instance;

        InitialiseCraftingTableUI();
    }

    public void SetTable(InteractableObject currentTable)
    {
        this.currentTable = currentTable;
    }

    public void UnlockRecipe(RecipeScriptableObject recipe)
    {
        unlockedRecipes.Add(recipe);
        unlockedRecipes = unlockedRecipes.OrderBy(recipe => recipe.category).ToList<RecipeScriptableObject>();
    
        SetUpButton(recipe);
    }

    public void SetUpButton(RecipeScriptableObject recipe)
    {
        Button currentButton = Instantiate(menuButtonPrefab, menu.transform.GetChild(recipe.category).GetChild(1)).GetComponent<Button>();
        recipeButtons.Add(currentButton);
        currentButton.GetComponent<Image>().sprite = recipe.craftedItem.sprite;

        currentButton.onClick.AddListener( () => CraftObjectFunc(recipe));
    }

    public void InitialiseCraftingTableUI()
    {
        foreach(RecipeScriptableObject recipe in unlockedRecipes)
        {
            SetUpButton(recipe);
        }
    }

    public override void OpenMenuFunctions()
    {
        panel.SetActive(true);

        for(int i = 0; i < recipeButtons.Count; i++)
        {
            // set button based on ingredient availability
            recipeButtons[i].interactable = CheckIngredients(unlockedRecipes[i].ingredients);
        }
    }

    public override void CloseMenuFunctions()
    {
        panel.SetActive(false);

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
        if(!buttonHolder.activeInHierarchy)
        {
            ResetCraftingTable();
            buttonHolder.SetActive(true);
        }
        else
        {
            ResetCraftingTable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Crafting Functions

    public IEnumerator CraftObject (RecipeScriptableObject recipe)
    {
        //currentTable.Clickable(false);

        ToggleMenu();
        iSystem.RemoveItem(recipe.ingredients);

        yield return new WaitForSeconds(recipe.cookingTime);

        GameObject craftedItem = Instantiate(recipe.craftedItem.worldPrefab, spawnCoords.position, spawnCoords.rotation, spawnCoords.transform);
        craftedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //currentTable.Clickable(true);
    }

    public void CraftObjectFunc(RecipeScriptableObject recipe)
    {
        StartCoroutine(CraftObject(recipe));
    }


    //check if have ingredientes for a recipe
    public bool CheckIngredients(List<ItemScriptableObject> ingredients)
    {
        //make a temp list
        List<ItemScriptableObject> tempList = new List<ItemScriptableObject>(iSystem.inventoryList);

        //check if the inventory list has all of the ingredients
        for(int i = 0; i < ingredients.Count; i++)
        {
            if (!tempList.Contains(ingredients[i]))
            {
                return false;
            }
            else
            {
                tempList.Remove(ingredients[i]);
            }
        }

        return true;
    }

    #endregion
}
