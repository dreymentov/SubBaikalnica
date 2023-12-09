using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScannedInfoDisplayer : MonoBehaviour
{
    public Image picture;
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;

    public void DisplayInfo(ItemScriptableObject item)
    {
        picture.sprite = item.sprite;
        title.text = item.name;
        text.text = item.description;

        gameObject.SetActive(true);
    }
}
