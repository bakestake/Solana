using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System;

public class TransactionTracker : MonoBehaviour
{
    public static TransactionTracker Instance { get; private set; }

    // you can make this class a singleton if you wish so
    [Header("Cellphone References")]
    public GameObject cellphoneUI;
    public CanvasGroup cellphoneUICanvasGroup;
    public GameObject liveTransactionPanel;
    public GameObject noLiveTransactionSkeleton;
    public GameObject liveTxUiAlert;
    public TMP_Text mobileClockText;
    public GameObject liveTransactionPrefab;
    public Transform liveTransactionContainer;
    public TMP_Text[] lastTransactionsText;
    [Header("Notification References")]
    public GameObject notificationBar;
    public TMP_Text notificationText;
    public TMP_Text notificationTimeText;

    [Header("Settings")]
    public bool allowMultipleTransactions = false; // control whether multiple transactions can be active at the same time
    public int maxLiveTransactions = 3;
    [Space]
    public float cellphonePosY;

    private Queue<string> lastTransactions = new Queue<string>();
    private List<GameObject> activeTransactions = new List<GameObject>();
    private const int maxTransactions = 5;
    private Coroutine notificationCoroutine;
    private Tween currentTween;
    private string currentScene;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        cellphoneUICanvasGroup.alpha = 0;
        notificationBar.SetActive(false);
        HandleNoLiveTransaction();

        cellphoneUI.transform.localPosition = new Vector3(cellphoneUI.transform.localPosition.x, -Screen.height, 0);
    }

    private void Update()
    {
        mobileClockText.text = DateTime.Now.ToString("HH:mm");

        if (currentScene != "Game_Multiplayer" && LocalGameManager.Instance.canUseKeybinds && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCellphoneUI();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            int rand = UnityEngine.Random.Range(0, 10);

            if (rand <= 2)
            {
                AddTransaction("Raid", "Binance", "Base", 5f);
            }
            else if (rand > 2 && rand <= 4)
            {
                AddTransaction("Stake", "Arbitrum", 20f);
            }
            else if (rand > 4 && rand <= 6)
            {
                AddTransaction("Burn", "Avalanche", 35f);
            }
            else if (rand > 8 && rand <= 8)
            {
                AddTransaction("Unstake", "Berachain", 35f);
            }
            else
            {
                AddTransaction("Claim", "Polygon", 10f);
            }

        }

        // to find the raid transaction, change the text to whatever you want to pinpoint an active transaction from another script
        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < activeTransactions.Count; i++)
            {
                if (activeTransactions[i].GetComponentsInChildren<Transform>()
                               .FirstOrDefault(c => c.gameObject.name == "TransactionType")?.GetComponent<TMP_Text>().text == "Raid:")
                {
                    EndTransaction(activeTransactions[i], true);
                    break;
                }
            }
        }
#endif
    }

    public void CallingEndTransaction(string typeOfTransaction, bool bWasuccessful)
    {
        Debug.Log("Command is here");
        for (int i = 0; i < activeTransactions.Count; i++)
        {
            Debug.Log(activeTransactions[i].GetComponentsInChildren<Transform>().FirstOrDefault(c => c.gameObject.name == "TransactionType").GetComponent<TMP_Text>().text);
            Debug.Log("The type of transaction" + typeOfTransaction);
            if (activeTransactions[i].GetComponentsInChildren<Transform>()
                           .FirstOrDefault(c => c.gameObject.name == "TransactionType")?.GetComponent<TMP_Text>().text == (typeOfTransaction + ":"))
            {
                Debug.Log("This is Working");
                Debug.Log("Was it successful:" + bWasuccessful);
                EndTransaction(activeTransactions[i], bWasuccessful);
                break;
            }
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationText.text = message;
        notificationTimeText.text = DateTime.Now.ToString("HH:mm tt");
        notificationBar.SetActive(true);

        notificationBar.transform.localPosition = new Vector3(0, Screen.height, 0);
        notificationBar.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutCubic);

        notificationCoroutine = StartCoroutine(HideNotificationAfterDelay(3f));
    }

    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        notificationBar.transform.DOLocalMoveY(Screen.height, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            notificationBar.SetActive(false);
        });

        notificationCoroutine = null;
    }

    public bool AddTransaction(string type, string fromNetwork, string toNetwork, float estimatedTime)
    {
        if (activeTransactions.Count > 0 && !allowMultipleTransactions)
        {
            ShowNotification("Transaction already in progress");
            return false;
        }

        if (allowMultipleTransactions && activeTransactions.Count >= maxLiveTransactions)
        {
            ShowNotification("Too many live transactions!");
            return false;
        }

        string activeTransactionInfo = $"{fromNetwork} --> {toNetwork}";

        GameObject newTransaction = Instantiate(liveTransactionPrefab, liveTransactionContainer);

        TMP_Text transactionTypeText = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "TransactionType")?.GetComponent<TMP_Text>();
        transactionTypeText.text = type + ":";

        TMP_Text network1Text = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network1")?.GetComponent<TMP_Text>();
        network1Text.text = fromNetwork;

        GameObject arrow = newTransaction.GetComponentsInChildren<Transform>()
                           .FirstOrDefault(c => c.gameObject.name == "Arrow")?.gameObject;
        arrow.SetActive(true);

        GameObject dot = newTransaction.GetComponentsInChildren<Transform>()
                           .FirstOrDefault(c => c.gameObject.name == "Dot")?.gameObject;
        dot.SetActive(false);

        TMP_Text network2Text = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network2")?.GetComponent<TMP_Text>();
        network2Text.text = toNetwork;

        TMP_Text estimatedTimeText = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "EstimatedTime")?.GetComponent<TMP_Text>();
        estimatedTimeText.text = "Est. time: " + estimatedTime + " seconds";

        ProgressBar progressBar = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "ProgressBar")?.GetComponent<ProgressBar>();
        progressBar.SetProgress(1, 1 / estimatedTime);

        activeTransactions.Add(newTransaction);

        liveTransactionPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
        HandleNoLiveTransaction();
        newTransaction.GetComponent<Animator>().SetTrigger("in");

        ShowNotification("Transaction started - <color=orange>" + type);
        return true;
    }

    public bool AddTransaction(string type, string onNetwork, float estimatedTime)
    {
        if (activeTransactions.Count > 0 && !allowMultipleTransactions)
        {
            ShowNotification("Transaction already in progress");
            return false;
        }

        if (allowMultipleTransactions && activeTransactions.Count >= maxLiveTransactions)
        {
            ShowNotification("Too many live transactions!");
            return false;
        }

        string activeTransactionInfo = $"{onNetwork}";

        GameObject newTransaction = Instantiate(liveTransactionPrefab, liveTransactionContainer);

        TMP_Text transactionTypeText = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "TransactionType")?.GetComponent<TMP_Text>();
        transactionTypeText.text = type + ":";

        TMP_Text network1Text = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network1")?.GetComponent<TMP_Text>();
        network1Text.text = onNetwork;

        GameObject arrow = newTransaction.GetComponentsInChildren<Transform>()
                           .FirstOrDefault(c => c.gameObject.name == "Arrow")?.gameObject;
        arrow.SetActive(false);

        GameObject dot = newTransaction.GetComponentsInChildren<Transform>()
                           .FirstOrDefault(c => c.gameObject.name == "Dot")?.gameObject;
        dot.SetActive(true);

        TMP_Text network2Text = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network2")?.GetComponent<TMP_Text>();
        network2Text.gameObject.SetActive(false);

        TMP_Text estimatedTimeText = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "EstimatedTime")?.GetComponent<TMP_Text>();
        estimatedTimeText.text = "Est. time: " + estimatedTime + " seconds"; ;

        ProgressBar progressBar = newTransaction.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "ProgressBar")?.GetComponent<ProgressBar>();
        progressBar.SetProgress(1, 1 / estimatedTime);

        activeTransactions.Add(newTransaction);

        liveTransactionPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
        HandleNoLiveTransaction();

        newTransaction.GetComponent<Animator>().SetTrigger("in");
        ShowNotification("Transaction started - <color=orange>" + type);
        return true;
    }

    public void EndTransaction(GameObject transactionObject, bool wasSuccessful)
    {
        TMP_Text transactionTypeText = transactionObject.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "TransactionType")?.GetComponent<TMP_Text>();

        TMP_Text network1Text = transactionObject.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network1")?.GetComponent<TMP_Text>();

        TMP_Text network2Text = transactionObject.GetComponentsInChildren<Transform>()
                            .FirstOrDefault(c => c.gameObject.name == "Network2")?.GetComponent<TMP_Text>();

        string transactionType = transactionTypeText.text;
        string network1 = network1Text.text;
        string network2 = network2Text?.text + "";
        string arrow = network2Text ? "-->" : "";
        string currentTime = DateTime.Now.ToString("HH:mm");

        string successColor = wasSuccessful ? "<color=green>" : "<color=red>";
        string successMessage = wasSuccessful ? "<color=green>successful" : "<color=red>failed";
        string transactionRecord = successColor + transactionType + "</color> <color=white>" + network1 + "</color> <color=grey>" + arrow + "</color> <color=white>" + network2 + "</color> <color=grey>" + currentTime + "</color>";

        lastTransactions.Enqueue(transactionRecord);
        if (lastTransactions.Count > maxTransactions)
        {
            lastTransactions.Dequeue();
        }
        UpdateTransactionHistory(wasSuccessful);

        StartCoroutine(HandleTransactionCompletion(transactionObject, wasSuccessful));

        transactionObject.GetComponent<Animator>().SetTrigger(wasSuccessful ? "success" : "fail");

        activeTransactions.Remove(transactionObject);
        Canvas.ForceUpdateCanvases();
        ShowNotification($"Transaction {successMessage}");
    }

    private IEnumerator HandleTransactionCompletion(GameObject transactionObject, bool wasSuccessful)
    {
        Animator animator = transactionObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(wasSuccessful ? "success" : "fail");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        }
        else
        {
            yield return new WaitForSeconds(0f);
        }
        Destroy(transactionObject);
        HandleNoLiveTransaction();
    }

    private void UpdateTransactionHistory(bool wasSuccessful)
    {
        int i = 0;
        foreach (string transaction in lastTransactions)
        {
            if (!lastTransactionsText[i].transform.parent.gameObject.activeInHierarchy) { lastTransactionsText[i].transform.parent.gameObject.SetActive(true); }
            lastTransactionsText[i].text = transaction;
            i++;
            Canvas.ForceUpdateCanvases();
        }
    }

    public bool HasActiveTransaction()
    {
        return activeTransactions.Count > 0;
    }

    public GameObject GetActiveTransactionObject()
    {
        if (activeTransactions.Count > 0)
        {
            return activeTransactions[0];
        }
        return null;
    }

    public void ToggleCellphoneUI()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        if (cellphoneUICanvasGroup.alpha == 1)
        {
            currentTween = cellphoneUI.transform.DOLocalMoveY(-Screen.height, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                cellphoneUICanvasGroup.alpha = 0;
            });
            SoundManager.instance.PlaySfx(SoundManager.instance.phoneOut);
        }
        else
        {
            cellphoneUICanvasGroup.alpha = 1;
            cellphoneUI.GetComponent<Animator>().SetTrigger("open");
            currentTween = cellphoneUI.transform.DOLocalMoveY(cellphonePosY, 0.5f).SetEase(Ease.OutBack);
            SoundManager.instance.PlaySfx(SoundManager.instance.phoneIn);
        }
    }

    private void HandleNoLiveTransaction()
    {
        if (activeTransactions.Count == 0)
        {
            noLiveTransactionSkeleton.SetActive(true);
            liveTxUiAlert.SetActive(false);
        }
        else
        {
            noLiveTransactionSkeleton.SetActive(false);
            liveTxUiAlert.SetActive(true);
        }
    }
}