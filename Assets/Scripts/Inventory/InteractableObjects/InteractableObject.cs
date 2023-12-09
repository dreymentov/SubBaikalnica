using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableObject : MonoBehaviour
{
    public ItemScriptableObject item;
    public bool interactable = true;

    Outline ol;
    public static PlayerController pc;

    private void Awake()
    {
        ol = GetComponent<Outline>();
        ol.enabled = false;
    }

    private void Update()
    {
        if(pc.currentHoverObject !=this || !interactable)
        {
            ol.enabled = false;
        }
    }

    public void Highlight()
    {
        ol.enabled = true;
    }

    public void Clickable(bool b)
    {
        interactable = b;
    }

    public void FreezeMovement(bool freeze)
    {
        if (freeze)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
