using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridObject
{
    public static GameObject uiPrefab;
    private Grid<GridObject> grid;
    public int x;
    public int y;
    private GameObject itemImage;

    public ItemScriptableObject item;
    public ItemScriptableObject tempItem;

    //class constructor
    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        item = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + item.name;
    }

    //changes what object placed in this grid object
    public void SetItem(ItemScriptableObject item, InventorySystem iSystem)
    {
        this.item = item;
        if(itemImage == null)
        {
            itemImage = GameObject.Instantiate(uiPrefab, new Vector3(0, 0, 0) * grid.GetCellSize(), Quaternion.identity, iSystem.menu.transform);
        }
        itemImage.GetComponentInChildren<Image>().sprite = item.sprite;
        itemImage.GetComponentsInChildren<RectTransform>()[1].sizeDelta = grid.GetCellSize() * item.size;
        itemImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0) * grid.GetCellSize();
        itemImage.GetComponentInChildren<InteractableUIObject>().item = item;
        itemImage.GetComponentInChildren<InteractableUIObject>().storageBox = iSystem;
        itemImage.SetActive(true);
        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //clear item from the gridobject
    public void ClearItem()
    {
        item = null;
        if (itemImage != null)
        {
            itemImage.SetActive(false);
        }
        //trigger event handler
        grid.TriggerGridObjectChanged(x, y);
    }

    //returns the current scriptable object
    public ItemScriptableObject GetItem()
    {
        return item;
    }

    //checks if there is no itemscriptableobject in the gridobject
    public bool EmptyItem()
    {
        return item == null;
    }

    public void SetTemp(ItemScriptableObject item)
    {
        tempItem = item;
    }

    public bool EmptyTemp()
    {
        return tempItem == null;
    }

    public void ClearTemp()
    {
        tempItem = null;
    }

    public ItemScriptableObject GetTemp()
    {
        return tempItem;
    }

    public void SetTempAsReal(InventorySystem iSystem)
    {
        ClearItem();
        if (!EmptyTemp())
        {
            SetItem(tempItem, iSystem);
        }
        ClearTemp();
    }
}
