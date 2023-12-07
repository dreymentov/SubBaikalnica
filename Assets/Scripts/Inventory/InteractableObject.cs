using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public ItemScriptableObject item;
    public bool picked;

    void Start()
    {
    	if (!picked)
    	{
    		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    		transform.localRotation = new Quaternion(0,0,0,0);
    	}
    }
}
