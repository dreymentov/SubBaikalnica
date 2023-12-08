using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseClass : MonoBehaviour
{

    public static bool menuOpen = false;
    public static GameObject currentMenu = null;

    public static PlayerController pc;

    public GameObject menu;

    public void Start()
    {
        pc = PlayerController.Instance;
    }
    public void ToggleMenu()
    {
        if(!menuOpen)
        {
            OpenMenu();
        }
        else if(CurrentmenuIsThis())
        {
            CloseMenu();
        }
    }
    public void OpenMenu()
    {
        Debug.Log(pc);
        pc.HideHotbaritem();

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        OpenMenuFunctions();
        menu.SetActive(true);
        currentMenu = menu;
        menuOpen = true;
    }

    public virtual void OpenMenuFunctions()
    {
        return;
    }

    public virtual void CloseMenuFunctions()
    {
        return;
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        CloseMenuFunctions();
        menu.SetActive(false);
        currentMenu = null;
        menuOpen = false;
    }

    public bool CurrentmenuIsThis()
    {
        return currentMenu = menu;
    }

    public bool CurrentMenuIsThis(GameObject menu)
    {
        return currentMenu == menu;
    }
}
