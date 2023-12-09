using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySystem : UIBaseClass
{
    public static InventorySystem Instance { get; private set; }

    internal Grid<GridObject> grid;

    internal int gridWidth = 6;
    internal int gridHeight = 8;
    internal float cellSize = 100f;

    //list of all items in the inventory
    public List<ItemScriptableObject> itemsList = new List<ItemScriptableObject>();

    public GameObject uiPrefab;
    public ItemScriptableObject fillerItem;
    //StorageManager sm;
    public Transform dropItemPoint;

    [Header("Hotbar")]
    public List<HotbarSlot> hotbar;
    public List<KeyCode> hotbarKeys;
    public Sprite blankSprite;
    public GameObject hotbarHolder;

    [Header("Gear Slots")]
    public List<GearScriptableObject> gearSlots = new List<GearScriptableObject>(5); // 0 is body, 1 is helmet, 2 is flipper, 3 is tank, 4 and 5 are upgrade slots
    public List<Image> gearSlotUI;
    public GameObject gearMenu;
    public GearUniversalFunctions guf;

    //event that tells scripts when the inventory is sorted
    public delegate void InventoryUpdated();
    public event InventoryUpdated OnInventoryUpdated;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        GridObject.uiPrefab = uiPrefab;

        //create the grid
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        SortItems();

        //sm = StorageManager.Instance;

        HotbarSlot.blankSprite = blankSprite;

    }

    // Update is called once per frame
    internal virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }

        //only run the code if the menu is open
        if (!menuOpen)
        {
            hotbarHolder.SetActive(true);
            return;
        }

        //toggle hotbar
        if (!CurrentMenuIsThis())
        {
            hotbarHolder.SetActive(false);
        }
        else
        {
            hotbarHolder.SetActive(true);
        }

        //check whats hovered
        PointerEventData hoveredObj = ExtendedStandaloneInputModule.GetPointerEventData();
        InteractableUIObject io = null;

        foreach( GameObject currentObj in hoveredObj.hovered)
        {
            io = currentObj.GetComponent<InteractableUIObject>();
        }

        //if not hovering over anything, return
        if(io == null)
        {
            return;
        }

        //movement stuff
        if(Input.GetMouseButtonDown(0) && (CurrentMenuIsThis() /*|| CurrentMenuIsThis(sm.menu)*/))
        {
            //move item from one storage space to another
            io.storageBox.MoveItem(io.item);
        }

        if (Input.GetMouseButtonDown(1) && CurrentMenuIsThis())
        {
            DropItem(io.item);
        }

        //hotbar keypressing detection
        for (int key = 0; key < 5; key++)
        {
            if (Input.GetKeyDown(hotbarKeys[key]))
            {
                Debug.Log("hotbar " + key);

                //assign item to hotbar
                AssignItemToHotbar(io.item, key);
                break;
            }
        }
    }

    #region Interacting with items

    //called when you pick up something from the world
    public void PickUpItem(InteractableObject itemPicked)
    {
        if (AddItem(itemPicked.item))
        {
            //if all goes well, destroy the object
            Destroy(itemPicked.gameObject);
        }
    }

    //move from the current inventory system from the other
    public virtual void MoveItem(ItemScriptableObject item)
    {
        //if we are not in a storage menu
        if (CurrentMenuIsThis())
        {
            //if the item is gear
            if (item.itemType == ItemScriptableObject.itemCategories.Gear)
            {
                //attempt to equip, but if cant, add item back
                if (!EquipGear(item as GearScriptableObject))
                {
                    Debug.Log("cannot equip gear");
                }
            }
            //if the item isnt gear, or cannot be equipped as gear, do nothing
            return;
        }

        //functions for if moving between storages

        RemoveItem(item);

        Debug.Log("otherside sorting");
        /*if (!StorageManager.Instance.AddItem(item))
        {
            Debug.Log("cannot fit in storage");
            AddItem(item);
            return;
        }*/

        //so the function doesnt run unessesarily
        if(item.itemType == ItemScriptableObject.itemCategories.Tool)
        {
            CheckHotbar();
        }
    }

    public void DropItem(ItemScriptableObject item)
    {
        RemoveItem(item);

        //spawn in the world
        GameObject spawnedItem = Instantiate(item.worldPrefab, dropItemPoint.position, Quaternion.identity);
        spawnedItem.GetComponent<InteractableObject>().FreezeMovement(false);

        if (item.itemType == ItemScriptableObject.itemCategories.Tool)
        {
            CheckHotbar();
        }
        return;
    }

    //add an item to the current inventory
    public bool AddItem(ItemScriptableObject itemAdded)
    {
        itemsList.Add(itemAdded);

        //sort inventory
        if (SortItems() == false)
        {
            //remove it from the inventory list
            itemsList.Remove(itemAdded);

            //error
            Debug.Log("inventory full!");

            return false;
        }

        return true;
    }

    //remove object from current inventory
    public void RemoveItem(ItemScriptableObject item)
    {
        itemsList.Remove(item);
        SortItems();
    }

    //remove multiple items at once
    public void RemoveItem(List<ItemScriptableObject> items)
    {
        for(int i = 0; i < items.Count; i++)
        {
            itemsList.Remove(items[i]);
        }
        SortItems();
    }

    //function for deploying items
    public void DeployItem(ItemScriptableObject item)
    {
        RemoveItem(item);

        //spawn in the world
        Instantiate(item.worldPrefab, dropItemPoint.position, Quaternion.identity);
        CheckHotbar();
        return;
    }

    #endregion

    #region Hotbar Functions

    public bool IsHotbarEquippable(ItemScriptableObject item)
    {
        return item.itemType != ItemScriptableObject.itemCategories.Generic;
    }

    //function to assign or disassign hotbar slots and has assigning logic for the slots
    public void AssignItemToHotbar(ItemScriptableObject item, int slotNum)
    {
        //exits function if the item is not equipable to the hotbar
        if (!IsHotbarEquippable(item))
        {
            return;
        }

        //if same as current obj, disassign the hotbarslot.
        if(hotbar[slotNum].hotbarInventoryItemIdentifier == item)
        {
            hotbar[slotNum].SetSlot(null);
            return;
        }

        //if object is in a different hotbarslot, disassign that slot
        for(int i = 0; i < 5; i++)
        {
            if(hotbar[i].hotbarInventoryItemIdentifier == item)
            {
                hotbar[i].SetSlot(null);
            }
        }

        //assign to hotbar slot
        hotbar[slotNum].SetSlot(item);
    }

    public void CheckHotbar()
    {
        for(int slotNum = 0; slotNum < 5; slotNum++)
        {
            if (!itemsList.Contains(hotbar[slotNum].hotbarInventoryItemIdentifier))
            {
                hotbar[slotNum].SetSlot(null);

                //reset the slot spawned items
                hotbar[slotNum].SpawnHotbarItems();
            }
        }
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

    internal void ResetTempValues()
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
                if((x + gridCoordinate.x) >= gridWidth || (gridCoordinate.y + y) >= gridHeight || (x + gridCoordinate.x) < 0 || (gridCoordinate.y + y) < 0)
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
    internal bool AvailSpot(ItemScriptableObject item)
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
                        if(CheckIfFits(item as ItemScriptableObject,new Vector2(x, y)))
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
    internal virtual bool SortItems()
    {
        //Debug.Log("SortItems");

        //sort items by size
        var sortedList = itemsList.OrderByDescending(s => s.size.x * s.size.y);

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
            obj.SetTempAsReal(Instance);
        }

        //everytime we sort items, call an event
        if(OnInventoryUpdated != null)
        {
            OnInventoryUpdated();
        }
        return true;
    }

    #endregion

    #region Gear Equipping Functions

    public bool EquipGear(GearScriptableObject gear) //logic for which slot to put a gear into
    {
        int gearInt = (int)gear.gearType;
        Debug.Log(gearInt);

        //remove gear from inventory
        RemoveItem(gear);

        //determine what kind of gear it is
        if(gear.gearType == GearScriptableObject.GearCategories.Upgrade)
        {
            //check if slot 4 is empty. If it is not, set to slot 5
            //when the code continues, if 5 is empty, it sets it as 5
            //if 5 is not empty, unequip 5 and set it as the new 5.
            //when both slots are full, it will default to swapping out the 5 slot.
            if(gearSlots[4] != null)
            {
                gearInt = 5;
            }
        }

        //if not empty, add back equipped gear into inventory
        //because all gears of the same type have the same size, there is no problem when swapping gear.
        if (gearSlots[gearInt] != null)
        {
            AddItem(gearSlots[gearInt]);
        }

        //add the gear to the gearslot
        SetGear(gear,gearInt);

        guf.ActivateGearFunctions();
        
        return false;
    }

    //is put on each button on the gear menu ui
    public void UnequipGear(int gearInt) //function to remove a gear from gear slots
    {
        //check inventory for space, put gear back in
        if (AddItem(gearSlots[gearInt]))
        {
            SetGear(null, gearInt);

            guf.ActivateGearFunctions();
        }


    }

    public void SetGear(GearScriptableObject gear, int gearInt) //set up a gears functionality and UI in the gear menu 
    {
        Debug.Log("gear set");
        //if its already the same gear, do nothing
        if (gearSlots[gearInt] == gear)
        {
            
            return;
        }

        //unequip prev gear
        //if (gearSlots[gearInt] != null)
        //{
        //    gearSlots[gearInt].worldPrefab.GetComponent<GearBaseClass>().currentlyEquipped = false;
        //}

        //sets the gear in the slot
        gearSlots[gearInt] = gear;

        //updates the gear ui to display whats equipped
        Image currentGearSlot = gearSlotUI[gearInt];

        if (gearSlots[gearInt] == null)
        {
            currentGearSlot.sprite = null;
            currentGearSlot.gameObject.SetActive(false);
            return;
        }

        currentGearSlot.sprite = gear.sprite;
        currentGearSlot.gameObject.SetActive(true);

        
    }

    #endregion

    public override void OpenMenuFunctions()
    {
        gearMenu.SetActive(true);
    }

    public override void CloseMenuFunctions()
    {
        gearMenu.SetActive(false);
        //initialise hot bar items
        foreach (HotbarSlot slot in hotbar)
        {
            //Debug.Log("spawn items");
            slot.SpawnHotbarItems();
        }
    }
}
