using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIObject : MonoBehaviour
{
    //this calss is used for the ui prefabs in the inventory and storage system. it keeps track of what storage area its in
    public ItemScriptableObject item;
    public bool interactable = true;
    public InventorySystem storageBox;

    public void Clickable(bool b)
    {
        interactable = b;
    }

    public void SetSystem(InventorySystem i)
    {
        storageBox = i;
    }
}
