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

        if(hotbarInventoryItemIdentifier == null)
        {
            slotSprite.sprite = blankSprite;
            return;
        }

        slotSprite.sprite = hotbarInventoryItemIdentifier.sprite;
    }

    public void SpawnHotbarItems()
    {
        if(hotbarInventoryItemIdentifier == null)
        {
            return;
        }

        Destroy(hotbarItem);

        Debug.Log("Spawn Item");
        hotbarItem = Instantiate(hotbarInventoryItemIdentifier.worldPrefab, hand);
        hotbarItem.SetActive(false);

        Destroy(hotbarItem.GetComponent<Rigidbody>());
        Destroy(hotbarItem.GetComponent<BoxCollider>());
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
