using System;
using System.Collections;
using Bakeland;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;

public class LocalSaveManager : MonoBehaviour
{
    public static LocalSaveManager instance;
    [Header("Cloud Save Settings")]
    [SerializeField] private string url = "http://localhost/bakeland";
    [SerializeField] private string apiKey;

    [Header("User Info")]
    public string walletAddress;

    [Header("Chainsaw")]
    public GameObject chainsawItemPf;
    public Item chainsawItem;
    public GameObject chainsawItemPos;

    [Header("References")]
    public ItemContainer inventoryContainer;
    public QuestManager questManager;
    public TextMeshProUGUI lastLoginText;
    public AccessCodeChecker accessCodeChecker;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (inventoryContainer == null)
        {
            inventoryContainer = LocalGameManager.Instance.inventoryContainer;
        }

        // Clear the contents of the inventory
        ClearInventory();

        PlayerController.canMove = false;
    }

    private void ClearInventory()
    {
        // Assuming inventoryContainer has a method to clear items
        if (inventoryContainer != null)
        {
            inventoryContainer.ClearItems(); // Clear the items while keeping the slots
            //Debug.Log("Inventory contents cleared.");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T) && PlayerController.canInteract && ThirdwebManager.Instance.GetActiveWallet() != null)
        {
            // SaveEverything();
            CloudSave(false);
        }

        if (Input.GetKeyDown(KeyCode.Y) && PlayerController.canInteract && ThirdwebManager.Instance.GetActiveWallet() != null)
        {
            // LoadSaveState();
            CloudLoad();
        }
        if (Input.GetKeyDown(KeyCode.M) && PlayerController.canInteract && ThirdwebManager.Instance.GetActiveWallet() != null)
        {
            PlayerController.canMove = true;
            PlayerController.canInteract = true;
        }
#endif
    }

    public void SetWalletAddress(string wallet)
    {
        if (wallet.Length > 0)
        {
            Debug.Log($"Setting wallet address: {wallet}");
            walletAddress = wallet;
            Debug.Log(walletAddress);
            Debug.Log("Starting cloud load");
            CloudLoad();
        }
    }

    public bool CheckAccessCodeEntry()
    {
        if (ES3.FileExists("Save/AccessCode.es3"))
        {
            bool accessCode = ES3.Load<bool>("AccessCode", "Save/AccessCode.es3");
            return accessCode;
        }
        else return false;
    }

    public void SaveInventory()
    {
        if (inventoryContainer != null)
        {
            ES3.Save("Inventory", inventoryContainer, "Save/Inventory.es3");
            //InventoryApi.MintBasket(WalletConnectScript.connectedWalletAddress);
        }
        else
        {
            Debug.LogError("Cannot save inventory: inventoryContainer is null");
        }
    }

    public void SaveLastLogin()
    {
        ES3.Save("LastLogin", DateTime.Now.ToString(), "Save/LastLogin.es3");
    }

    public void SavePlayerGold()
    {
        ES3.Save("Gold", PlayerCurrency.instance.playerGold, "Save/Gold.es3");
    }

    public void SaveQuestLog()
    {
        questManager.SaveQuests();
        ES3.Save("QuestLog", "logged", "Save/QuestLog.es3");
    }

    public void SaveAccess()
    {
        ES3.Save("AccessCode", accessCodeChecker.hasAccess, "Save/AccessCode.es3");
        Debug.Log("Access code saved: " + accessCodeChecker.hasAccess);
    }

    public void SaveEverything()
    {
        SaveInventory();
        SaveLastLogin();
        SavePlayerGold();
        SaveQuestLog();
        questManager.SaveQuests();
        SaveAccess();
        // questManager.SaveTest();

        Debug.Log("Saved game state!");
    }

    public void LoadSaveState()
    {
        Debug.Log("Inventory Load Start");
        if (ES3.FileExists("Save/Inventory.es3"))
        {
            inventoryContainer = ES3.Load<ItemContainer>("Inventory", "Save/Inventory.es3");
        }
        Debug.Log("Inventory Load End");

        Debug.Log("Last Login Load Start");
        if (ES3.FileExists("Save/LastLogin.es3"))
        {
            DateTime lastLogin = DateTime.Parse(ES3.Load<string>("LastLogin", "Save/LastLogin.es3"));
            TimeSpan timeSpan = DateTime.Now - lastLogin;

            lastLoginText.text = string.Format("Last login: {0} days {1} hours {2} minutes {3} seconds ago", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            ES3.Save("LastLogin", DateTime.Now.ToString(), "Save/LastLogin.es3");
        }
        Debug.Log("Last Login Load End");

        Debug.Log("Gold Load Start");
        if (ES3.FileExists("Save/Gold.es3"))
        {
            PlayerCurrency.instance.playerGold = ES3.Load<int>("Gold", "Save/Gold.es3");
            PlayerCurrency.instance.LoadCurrency();
        }
        Debug.Log("Gold Load End");

        Debug.Log("Quest Log Load Start");
        if (ES3.FileExists("Save/QuestLog.es3"))
        {
            Debug.Log("Questlog exists.");
            // questManager.LoadQuests();
            // questManager.questMap = ES3.Load<Dictionary<string, Quest>>("QuestLog", "Save/QuestLog.es3");
        }

        Debug.Log("Quests Info Load Start");

        questManager.LoadQuests();
        // questManager.LoadTest();


        Debug.Log("Quests Info Load End");
        // loadEvent?.Invoke();
    }

    public void CloudSave(bool withLoad)
    {
        StartCoroutine(CloudSaveRoutine(withLoad));
    }

    public void CloudLoad()
    {
        StartCoroutine(CloudLoadRoutine());
    }

    private IEnumerator CloudSaveRoutine(bool withLoad)
    {
        SaveEverything();

        LoadingWheel.instance.EnableLoading();
        PlayerController.canMove = false;

        var cloud = new ES3Cloud(url, apiKey);
        int errorCount = 0;

        yield return StartCoroutine(cloud.UploadFile("Save/Inventory.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
            Debug.LogError("Error code: " + cloud.errorCode);
        }
        yield return StartCoroutine(cloud.UploadFile("Save/LastLogin.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }
        yield return StartCoroutine(cloud.UploadFile("Save/Gold.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }
        yield return StartCoroutine(cloud.UploadFile("Save/QuestLog.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }
        yield return StartCoroutine(cloud.UploadFile("Save/AccessCode.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }

        foreach (Quest quest in questManager.questMap.Values)
        {
            yield return StartCoroutine(cloud.UploadFile("Save/" + quest.info.id + ".es3", walletAddress));
            if (cloud.isError)
            {
                errorCount++;
                Debug.LogError(cloud.error);
                Debug.LogError("Error code: " + cloud.errorCode);
            }
        }

        if (errorCount > 0)
        {
            Debug.Log("save failed");
            // saveGameButton.SaveFailed();
            // saveLoadScreenAnimator.SetTrigger("savefail");
        }
        else
        {
            Debug.Log("save succeeded");

            if (withLoad)
            {
                CloudLoad();
            }
            // saveGameButton.SaveSuccessful();
            // saveLoadScreenAnimator.SetTrigger("savesuccess");
        }

        LoadingWheel.instance.DisableLoading();
        if (!accessCodeChecker.accessCodePanel.activeInHierarchy)
        {
            PlayerController.canMove = true;
        }

    }

    private IEnumerator CloudLoadRoutine()
    {
        LoadingWheel.instance.EnableLoading();
        PlayerController.canMove = false;

        var cloud = new ES3Cloud(url, apiKey);
        int errorCount = 0;

        Debug.Log("Starting cloud download for Inventory.es3");
        yield return StartCoroutine(cloud.DownloadFile("Save/Inventory.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError($"Error downloading inventory: {cloud.error}");
        }
        else
        {
            Debug.Log("Successfully downloaded Inventory.es3 from cloud");
        }

        yield return StartCoroutine(cloud.DownloadFile("Save/LastLogin.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }

        yield return StartCoroutine(cloud.DownloadFile("Save/Gold.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }

        yield return StartCoroutine(cloud.DownloadFile("Save/QuestLog.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }

        yield return StartCoroutine(cloud.DownloadFile("Save/AccessCode.es3", walletAddress));
        if (cloud.isError)
        {
            errorCount++;
            Debug.LogError(cloud.error);
        }

        foreach (Quest quest in questManager.questMap.Values)
        {
            yield return StartCoroutine(cloud.DownloadFile("Save/" + quest.info.id + ".es3", walletAddress));
            if (cloud.isError)
            {
                errorCount++;
                Debug.LogError(cloud.error);
                Debug.LogError("Error code: " + cloud.errorCode);
            }
        }

        Debug.Log($"Cloud load completed with {errorCount} errors");
        if (errorCount > 0)
        {
            CloudSave(true);
            Debug.Log("load failed");
        }
        else
        {
            Debug.Log("About to call LoadSaveState()");
            LoadSaveState();
            Debug.Log("load succeeded");
            CheckChainsawItem();
        }

        accessCodeChecker.HidePanel();
        LoadingWheel.instance.DisableLoading();
    }

    private void CheckChainsawItem()
    {
        if (!LocalGameManager.Instance.inventoryContainer.ContainsItem(chainsawItem))
        {
            //Instantiate(chainsawItemPf, chainsawItemPos.transform.position, Quaternion.identity);
        }
    }

}
