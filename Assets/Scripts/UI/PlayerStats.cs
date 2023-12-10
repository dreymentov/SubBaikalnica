using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : Health
{
    [SerializeField] private Transform player;

    public List<int> maxStats; //0 is oxygen, 1 is food, 2 is thirst
    List<int> currentStats;

    Coroutine oxygenCo;
    Coroutine hungerCo;
    Coroutine thirstCo;

    bool swimCheck;

    [Header("UI")]
    public List<Slider> statBars;
    public List<TextMeshProUGUI> statNums;
    public GameObject panelUnderwater;
    public TextMeshProUGUI textDepthMeter;
    public static GameObject panelMessage;
    public static TextMeshProUGUI textMessage;
    public static string testStr;
    public GameObject panelMessageStart;
    public TextMeshProUGUI textMessageStart;

    bool isInvoked;
    bool isInvokedToClose;

    private void Reset()
    {
        maxStats = new List<int>(new int[3] { 30, 100, 100 });
        currentHealth = maxHealth = 100;
        Canvas c = FindObjectOfType<Canvas>();
        statBars = new List<Slider>(c.GetComponentsInChildren<Slider>());
        statNums = new List<TextMeshProUGUI>(c.GetComponentsInChildren<TextMeshProUGUI>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        //initialise currentStats to the max amount
        currentStats = new List<int>(maxStats);

        //start decreasing the values of hunger and thirst
        hungerCo = StartCoroutine(DecreaseStats(1, 20, 1));
        thirstCo = StartCoroutine(DecreaseStats(2, 20, 1));

        //initialise the statBars
        for (int i = 0; i < maxStats.Count; i++)
        {
            statBars[i].maxValue = maxStats[i];
        }
        statBars[3].maxValue = maxHealth;

        //disable panel underwater in awake
        panelUnderwater.SetActive(false);

        panelMessage = panelMessageStart;
        textMessage = textMessageStart;

        isInvoked = false;
        isInvokedToClose = false;

        testStr = "Мир: Добро пожаловать в игру!";
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.isSwimming && swimCheck == true)
        {
            panelUnderwater.SetActive(false);
            swimCheck = false;
            StopCoroutine(oxygenCo);
            ChangeStat(0, maxStats[0]);
        }
        if (PlayerController.isSwimming && swimCheck == false)
        {
            panelUnderwater.SetActive(true);
            swimCheck = true;
            oxygenCo = StartCoroutine(DecreaseStats(0, 3, 3));
        }

        if (PlayerController.isSwimming)
        {
            textDepthMeter.text = "Глубина: " + (int)player.position.y*2 +"м";
        }
        else
        {
            textDepthMeter.text = "Глубина: " + "на суше";
        }

        //displaye currentstats in stat ui
        for (int i = 0; i < maxStats.Count; i++)
        {
            statBars[i].value = currentStats[i];
            statNums[i].text = currentStats[i].ToString();
        }
        statBars[3].value = currentHealth;
        statNums[3].text = currentHealth.ToString();

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (isInvoked == false)
            {
                isInvoked = true;
                StartCoroutine(ShowMessage(testStr));
            }
        }

        if(panelMessage.activeInHierarchy == true)
        {
            if (isInvokedToClose == false)
            {
                isInvokedToClose = true;
                StartCoroutine(CloseMessage());
            }
        }
    }

    IEnumerator DecreaseStats(int stat, int interval, int amount)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if(currentStats[stat] > 0)
            {
                currentStats[stat] = Mathf.Max(currentStats[stat] - amount, 0);
            }
        }
    }

    public void ChangeStat(int stat, int refreshAmount)
    {
        if(refreshAmount > 0)
        {
            currentStats[stat] = Mathf.Min(currentStats[stat] + refreshAmount, maxStats[stat]);
        }
        else
        {
            currentStats[stat] = Mathf.Max(currentStats[stat] + refreshAmount, 0);
        }
    }

    IEnumerator ShowMessage(string textMsg)
    {
        textMessage.text = textMsg;
        panelMessage.gameObject.SetActive(true);
        textMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        panelMessage.gameObject.SetActive(false);
        textMessage.gameObject.SetActive(false);
        isInvoked = false;
        yield break;
    }

    IEnumerator CloseMessage()
    {
        yield return new WaitForSeconds(5f);
        panelMessage.gameObject.SetActive(false);
        textMessage.gameObject.SetActive(false);
        isInvoked = false;
        isInvokedToClose = false;
        yield break;
    }
}
