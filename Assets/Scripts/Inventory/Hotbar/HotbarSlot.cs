using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public static Transform hand;
    public GameObject hotbarItem;

    Image slotSprite;
    public ItemScriptableObject hotbarInventoryItemIdentifier;

    public static Sprite blankSprite;

    public ToolBaseClass tool;

    // Start is called before the first frame update
    void Start()
    {
        slotSprite = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hotbarInventoryItemIdentifier == null)
        {
            slotSprite.sprite = blankSprite;
        }
    }

    public void SetSlot(ItemScriptableObject item)
    {
        hotbarInventoryItemIdentifier = item;

        //set image of hotbar
        if(hotbarInventoryItemIdentifier == null)
        {
            slotSprite.sprite = blankSprite;
            return;
        }

        slotSprite.sprite = hotbarInventoryItemIdentifier.sprite;
    }

    //function to create the hotbar item prefabs in the world
    //called when the inventory menu closes
    public void SpawnHotbarItems()
    {

        //destroy item
        Destroy(hotbarItem);

        if (hotbarInventoryItemIdentifier == null)
        {
            return;
        }

        //spawn in item
        hotbarItem = Instantiate(hotbarInventoryItemIdentifier.worldPrefab, hand);
        hotbarItem.SetActive(false);

        //set up item for tool use instead of world use. 
        Destroy(hotbarItem.GetComponent<Rigidbody>());
        Destroy(hotbarItem.GetComponent<BoxCollider>());
        tool = hotbarItem.GetComponent<ToolBaseClass>();
    }

    public void DisplayItem()
    {
        if(hotbarItem != null)
        {
            hotbarItem.SetActive(true);
        }
    }

    public void HideItem()
    {
        if(hotbarItem != null)
        {
            hotbarItem.SetActive(false);
        }
    }
}
