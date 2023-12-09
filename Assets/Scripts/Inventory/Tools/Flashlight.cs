using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : ToolBaseClass
{
    public GameObject lightSource;

    // Start is called before the first frame update
    internal override void Awake()
    {

        toolAction = ToggleLightSource;
    }

    void ToggleLightSource()
    {
        lightSource.SetActive(!lightSource.activeInHierarchy);
    }
}
