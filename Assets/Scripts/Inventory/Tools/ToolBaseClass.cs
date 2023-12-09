using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBaseClass : MonoBehaviour
{
    public static PlayerController pc;
    public static InventorySystem iSystem;
    public delegate void ToolAction();
    internal ToolAction toolAction;

    internal virtual void Awake()
    {
        toolAction = DefaultAction;
    }

    internal virtual void Update()
    {
        if(pc.CurrentHotbarItem() != null && pc.CurrentHotbarItem().tool == this)
        {
            if (Input.GetMouseButtonDown(1))
            {
                toolAction();
            }
        }
    }

    void DefaultAction()
    {
        Debug.Log("DefaultToolAction");
        return;
    }
}
