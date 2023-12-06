using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private Grid grid;

    int gridWidth = 6;
    int gridHeight = 8;
    float cellSize = 100f;

    //list of all items in inventory
    public List<ItemScriptableObject> inventoryList = new List<ItemScriptableObject>();

    public GameObject inventoryTab;
    public GameObject uiPrefab;
    public bool inventoryOpen;

    public ItemScriptableObject fillerItem;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        GridObject.inventoryTab = inventoryTab;
        GridObject.uiPrefab = uiPrefab;

        //create the grid
        grid = new Grid(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid g, int x, int y) => new GridObject(g, x, y));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                inventoryTab.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                inventoryTab.SetActive(false);
            }
            inventoryOpen = !inventoryOpen;
        }
    }

    #region Functions to sort the inventory

    //assign items to gidobjects
    void AssignItemToSpot(ItemScriptableObject item, List<Vector2> coords)
    {
        for (int i = 0; i < coords.Count; i++)
        {
            int x = (int)coords[i].x;
            int y = (int)coords[i].y;
            if (i != 0)
            {
                grid.GetGridObject(x, y).SetTemp(fillerItem);
            }
            else
            {
                grid.GetGridObject(x, y).SetTemp(item);
            }
        }
    }

    void AssignItemToSpot(ItemScriptableObject item, int x, int y)
    {
        grid.GetGridObject(x, y).SetTemp(item);
    }

    void ResetTempValues()
    {
        Debug.Log("reset temp");
        foreach (GridObject obj in grid.gridArray)
        {
            obj.ClearTemp();
        }
    }

    bool CheckIfFits(ItemScriptableObject item, Vector2 gridCoordinate)
    {
        List<Vector2> coordsToCheck = new List<Vector2>();

        //get all the coordinates based on the size of the item
        for (int x = 0; x < item.size.x; x++)
        {
            for (int y = 0; y > -item.size.y; y--)
            {
                //if one of the coords is out of bounds, return false
                if ((x + gridCoordinate.x) >= gridWidth || (gridCoordinate.y + y) >= gridHeight)
                {
                    return false;
                }

                coordsToCheck.Add(new Vector2(x + gridCoordinate.x, gridCoordinate.y + y));
            }
        }

        //check all the coordinates
        foreach (Vector2 coord in coordsToCheck)
        {
            if (!grid.GetGridObject((int)coord.x, (int)coord.y).EmptyTemp())
            {
                //if there is something in one of these coordinates, return false
                return false;
            }
        }

        //return true
        AssignItemToSpot(item, coordsToCheck);
        return true;
    }

    //check through every spot to find the next available spot
    bool AvailSpot(ItemScriptableObject item)
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //check if the spot is empty
                if (grid.GetGridObject(x, y).EmptyTemp())
                {
                    //check if size one
                    if (item.size == Vector2.one)
                    {
                        AssignItemToSpot(item, x, y);
                        return true;
                    }
                    else
                    {
                        if (CheckIfFits(item, new Vector2(x, y)))
                        {
                            return true;
                        }
                    }
                }

            }
        }

        //after checking every coordinate, no spots found
        return false;
    }

    //function returns true if all items can be sorted, and sorts them properly
    //returns false if items cannot be sorted and deletes all the temporary values
    bool SortItems()
    {
        Debug.Log("SortItems");

        //sort items by size
        var sortedList = inventoryList.OrderByDescending(s => s.size.x * s.size.y);

        //place items systematically
        foreach (ItemScriptableObject item in sortedList)
        {
            bool hasSpot = AvailSpot(item);
            if (hasSpot == false)
            {
                Debug.Log("doesnt fit!");
                ResetTempValues();
                return false;
            }
        }

        foreach (GridObject obj in grid.gridArray)
        {
            obj.SetTempAsReal();
        }

        return true;

    }

    #endregion
}
