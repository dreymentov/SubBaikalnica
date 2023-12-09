using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployable : ToolBaseClass
{
    internal override void Awake()
    {
        toolAction = DeployItem;
    }

    void DeployItem()
    {
        iSystem.DeployItem(pc.CurrentHotbarItem().hotbarInventoryItemIdentifier);
    }
}