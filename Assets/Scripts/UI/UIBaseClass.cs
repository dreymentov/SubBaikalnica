using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseClass : MonoBehaviour
{

    public static bool menuOpen = false;
    public static GameObject currentMenu = null;

    public GameObject menu;

    public static PlayerController pc;

    private void Start()
    {
        pc = PlayerController.Instance;
    }

    //opens and closes the menu
    public void ToggleMenu()
    {
        if (!menuOpen)
        {
            Debug.Log("openMenu");
            OpenMenu();
        }
        else if (CurrentMenuIsThis())
        {
            Debug.Log("closeMenu");
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        //hide hotbar item
        pc.HideHotbaritem();

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        OpenMenuFunctions();
        menu.SetActive(true);
        currentMenu = menu;
        menuOpen = true;
    }

    //specific menu actions
    public virtual void OpenMenuFunctions()
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

    //specific menu actions
    public virtual void CloseMenuFunctions()
    {
        return;
    }

    public bool CurrentMenuIsThis()
    {
        return currentMenu == menu;
    }

    public bool CurrentMenuIsThis(GameObject menu)
    {
        return currentMenu == menu;
    }
}
