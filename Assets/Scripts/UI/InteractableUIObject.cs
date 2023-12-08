using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIObject : MonoBehaviour
{
    public ItemScriptableObject item;
    public bool interactable = true;
    public InventorySystem iSystem;

    public void Clickable(bool b)
    {
        interactable = b;
    }

    public void SetSystem(InventorySystem i)
    {

    }
}
