using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScannedInfoManager : UIBaseClass
{
    public List<ItemScriptableObject> scannedObjects;

    //iteminfo prefab
    public GameObject buttonPrefab;

    [Header("Info Popup")]
    //handles the info page of the menu
    public ScannedInfoDisplayer infoPopup;
    public Transform infoPage;

    [Header("Displaying Scan Percentage Variables")]
    public GameObject scanDisplayer;
    public TextMeshProUGUI scanDisplayerText;
    public Image scanDisplayerSlider;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleMenu();
        }
    }

    public void UnlockScannedItem(ItemScriptableObject item)
    {
        item.scanPercentage = 100;
        item.scannable = false;

        scannedObjects.Add(item);
        scannedObjects = scannedObjects.OrderBy(item => item.id).ToList<ItemScriptableObject>();

        //generate button for the item
        GenerateButton(item);
    }

    public void GenerateButton(ItemScriptableObject item)
    {
        //create a button
        Button currentButton = Instantiate(buttonPrefab, infoPage).GetComponent<Button>();

        //set the button to have the correct function
        currentButton.onClick.AddListener(() => infoPopup.DisplayInfo(item));

        currentButton.GetComponentInChildren<Image>().sprite = item.sprite;

        currentButton.transform.SetSiblingIndex(scannedObjects.IndexOf(item));
    }

    public void InitialiseButtons()
    {
        scannedObjects = scannedObjects.OrderBy(item => item.id).ToList<ItemScriptableObject>();

        foreach(ItemScriptableObject item in scannedObjects)
        {
            GenerateButton(item);
        }
    }

    public void UpdateScanDisplayer(ItemScriptableObject selectedItem)
    {
        if (selectedItem == null || selectedItem.scannable == false)
        {
            scanDisplayerSlider.fillAmount = 0;
            scanDisplayerText.text = "0%";
            ToggleScanDisplayer(false);
            return;
        }

        scanDisplayerSlider.fillAmount = selectedItem.scanPercentage / 100;
        scanDisplayerText.text = selectedItem.scanPercentage.ToString() + "%";
        ToggleScanDisplayer(true);
    }

    public void ToggleScanDisplayer(bool show)
    {
        scanDisplayer.SetActive(show);
        pc.crosshair.SetActive(!show);
    }
}
