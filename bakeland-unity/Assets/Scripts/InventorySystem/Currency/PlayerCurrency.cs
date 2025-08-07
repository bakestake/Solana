using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency instance;

    [SerializeField] private NumberCounterUpdater numberCounterUpdater;
    public int startingGold = 0;
    public int playerGold;
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.goldEvents.onGoldGained += AddGold;
    }

    private void Start()
    {
        playerGold = startingGold;
        UpdateUI();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     ES3.Save("Gold", playerGold);
        // }
        // #if UNITY_EDITOR
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     AddGold(5);
        // }

        // if (Input.GetKeyDown(KeyCode.Y))
        // {
        //     if (CanAfford(10))
        //     {
        //         SubtractGold(10);
        //     }
        //     else
        //     {
        //         Debug.Log("Can't afford!");
        //     }
        // }
        // #endif
    }

    public void LoadCurrency()
    {
        // if (ES3.FileExists("Save/Gold.es3"))
        // {
        //     playerGold = ES3.Load<int>("Gold");
        // }
        // else
        // {
        //     playerGold = startingGold;
        //     playerGold = ES3.Load<int>("Gold");
        // }

        UpdateUI();
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        GameEventsManager.instance.goldEvents.GoldChange(amount);
        UpdateUI();
    }

    public void SubtractGold(int amount)
    {
        playerGold -= amount;
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        if (playerGold - amount < 0)
        {
            return false;
        }
        else return true;
    }

    private void UpdateUI()
    {
        goldText.text = playerGold.ToString();
        numberCounterUpdater.SetValue(playerGold);
    }
}
