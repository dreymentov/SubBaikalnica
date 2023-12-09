using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : ToolBaseClass
{
    ScannedInfoManager sim;
    float timer = 0;
    ItemScriptableObject currentHoverItem;

    // Start is called before the first frame update
    internal override void Awake()
    {
        sim = FindObjectOfType<ScannedInfoManager>();
    }

    // Update is called once per frame
    internal override void Update()
    {
        if (pc.CurrentHotbarItem() != null && pc.CurrentHotbarItem().tool == this)
        {
            if(pc.currentHoverObject != null)
            {
                currentHoverItem = pc.currentHoverObject.item;
            }
            else
            {
                currentHoverItem = null;
            }

            //display percentage
            sim.UpdateScanDisplayer(currentHoverItem);

            if(currentHoverItem != null && currentHoverItem.scannable)
            {
                if (Input.GetMouseButton(1))
                {
                    Scan(currentHoverItem);
                    return;
                }
            }

        }

    }

    public void Scan(ItemScriptableObject currentHoveritem)
    {
        //if one hundred percent, mark as scanned
        if (currentHoverItem.scanPercentage >= 100)
        {
            sim.UnlockScannedItem(currentHoverItem);
        }

        if(timer >= currentHoverItem.scanRate / 100)
        {
            currentHoverItem.scanPercentage++;
            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
