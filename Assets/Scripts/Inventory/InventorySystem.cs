using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySystem : UIBaseClass
{
    public InteractableObject io;
    public static InventorySystem Instance { get; private set; }

    private Grid<GridObject> grid;

    int gridWidth = 6;
    int gridHeight = 8;
    float cellSize = 100f;

    //list of all items in the inventory
    public List<ItemScriptableObject> inventoryList = new List<ItemScriptableObject>();

    public GameObject inventoryTab;
    public GameObject uiPrefab;
    public bool inventoryOpen;

    public GameObject cam;

    public float playerReach;

    public ItemScriptableObject fillerItem;

    public Transform dropItemPoint;

    [SerializeField] private Transform spawnPoint;

    [Header("Hotbar")]
    public List<HotbarSlot> hotbar;
    public List<KeyCode> hotbarKeys;
    public Sprite blankSprite;

    public GameObject hotbarHolder;

    public InteractableObject i;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        GridObject.inventoryTab = inventoryTab;
        GridObject.uiPrefab = uiPrefab;

        //create the grid
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        SortItems();

        //sm = StorageManager.Instance;

        HotbarSlot.blankSprite = blankSprite;
    }

    // Update is called once per frame
    void Update()
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

        i = HoverObject();

        if(i!= null)
        {
            //check if the player left clicks
            if (Input.GetMouseButtonDown(0))
            {
                //pickup item
                PickUpItem(i);
            }
        }


        if (Input.GetMouseButtonDown(1))// && (CurrentmenuIsThis() || CurrentmenuIsThis(/*sm.menu*/)))
        {
            PointerEventData hoveredObj = ExtendedStandaloneInputModule.GetPointerEventData();
            io = null;

            foreach (GameObject currentObj in hoveredObj.hovered)
            {
                io = currentObj.GetComponent<InteractableObject>();
            }

            Debug.Log(io);

            if (io == null)
            {
                return;
            }

            RemoveItem(io.item);

            CheckHotbar();
        }

        for(int key = 0; key < 5; key ++)
        {
            PointerEventData hoveredObj = ExtendedStandaloneInputModule.GetPointerEventData();
            io = null;

            foreach (GameObject currentObj in hoveredObj.hovered)
            {
                io = currentObj.GetComponent<InteractableObject>();
            }

            if (Input.GetKeyDown(hotbarKeys[key]))
            {
                AssignItemToHotbar(io.item, key);
                break;
            }
        }

        if (!menuOpen)
        {
            hotbarHolder.SetActive(true);
            return;
        }
    
        if (!CurrentmenuIsThis())
        {
            hotbarHolder.SetActive(false);
        }
        else
        {
            hotbarHolder.SetActive(true);
        }
    }

    #region Interacting with items

    InteractableObject HoverObject()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit, playerReach))
        {
            return hit.collider.gameObject.GetComponent<InteractableObject>();
        }

        return null;
    }

    //called when you pick up something
    public void PickUpItem(InteractableObject itemPicked)
    {
        inventoryList.Add(itemPicked.item);

        //sort inventory
        if(SortItems() == false)
        {
            //remove it from the inventory list
            inventoryList.Remove(itemPicked.item);

            //error
            Debug.Log("inventory full!");

            return;
        }

        //if all goes well, destroy the object
        Destroy(itemPicked.gameObject);
    }

    //remove object from inventory and spawn it in the world
    public void RemoveItem(ItemScriptableObject item)
    {
        inventoryList.Remove(item);
        SortItems();
        GameObject g = Instantiate(item.worldPrefab, spawnPoint.position, Quaternion.identity);
        g.GetComponent<InteractableObject>().picked = true;
    }

    public void RemoveItem(List<ItemScriptableObject> items)
    {
        for(int i = 0; i < items.Count; i++)
        {
            inventoryList.Remove(items[i]);
        }
        SortItems();
    }

    #endregion

    #region Hotbar Functions
    public bool IsHotbarEquippable(ItemScriptableObject item)
    {
        return item.itemType != ItemScriptableObject.itemCategories.Generic;
    }

    public void AssignItemToHotbar(ItemScriptableObject item, int slotNum)
    {
        if (!IsHotbarEquippable(item))
        {
            return;
        }

        if (hotbar[slotNum].hotbarInventoryItemIdentifier == item)
        {
            hotbar[slotNum].SetSlot(null);
            return;
        }

        for(int i = 0; i < 5; i++)
        {
            if (hotbar[i].hotbarInventoryItemIdentifier == item)
            {
                hotbar[i].SetSlot(null);
            }
        }

        hotbar[slotNum].SetSlot(item);
    }
    #endregion

    #region Functions to sort the inventory

    //assign items to gidobjects
    void AssignItemToSpot(ItemScriptableObject item, List<Vector2> coords)
    {
        for (int i = 0; i<coords.Count; i++)
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
        foreach(GridObject obj in grid.gridArray)
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
                if((x + gridCoordinate.x) >= gridWidth || (gridCoordinate.y + y) >= gridHeight)
                {
                    return false;
                }

                coordsToCheck.Add(new Vector2(x + gridCoordinate.x, gridCoordinate.y + y));
            }
        }

        //check all the coordinates
        foreach(Vector2 coord in coordsToCheck)
        {
            if(!grid.GetGridObject((int)coord.x, (int)coord.y).EmptyTemp())
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
            for(int x = 0; x < gridWidth; x++)
            {
                //check if the spot is empty
                if (grid.GetGridObject(x, y).EmptyTemp())
                {
                    //check if size one
                    if(item.size == Vector2.one)
                    {
                        AssignItemToSpot(item, x, y);
                        return true;
                    }
                    else
                    {
                        if(CheckIfFits(item,new Vector2(x, y)))
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

    public override void CloseMenuFunctions()
    {
        foreach (HotbarSlot slot in hotbar)
        {
            slot.SpawnHotbarItems();
        }
    }

    public void CheckHotbar()
    {
        for(int slotNum = 0; slotNum < 5; slotNum++)
        {
            if (!inventoryList.Contains(hotbar[slotNum].hotbarInventoryItemIdentifier))
            {
                hotbar[slotNum].SetSlot(null);
            }
        }
    }
}
