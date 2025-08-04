using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingWheel : MonoBehaviour
{
    public static LoadingWheel instance;

    [Header("References")]
    public GameObject loadingScreen;
    public Image loadingWheel;
    public TMP_Text loadingText;
    public TMP_Text infoText;
    public Animator fader;

    [Header("Settings")]
    public List<Color> colors;
    public List<string> lines;
    public List<string> infoLines;

    public Action onFadeFinished;
    public Action onLoadingFinished;

    private string CurrentScene;

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

    // Start is called before the first frame update
    void Start()
    {
        DisableLoading();
        CurrentScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N) && PlayerController.canInteract && CurrentScene != "PvP-Menu")
        {
            EnableLoading();
        }

        if (Input.GetKeyDown(KeyCode.M) && PlayerController.canInteract && CurrentScene != "PvP-Menu")
        {
            DisableLoading();
        }
#endif
    }

    public void EnableLoading()
    {
        if (loadingScreen.activeInHierarchy) { return; }
        SetRandomColor();
        SetRandomText();
        SetRandomInfoText();
        loadingScreen.SetActive(true);
        //SoundManager.instance.PlayRandomFromList(SoundManager.instance.popSounds);
    }

    public void DisableLoading()
    {
        loadingScreen.SetActive(false);
        onLoadingFinished?.Invoke();
    }

    public void FadeInOut(System.Action callback = null)
    {
        StartCoroutine(Coroutine_FadeInOut(callback));
    }

    private IEnumerator Coroutine_FadeInOut(System.Action callback)
    {
        PlayerController.canMove = false;
        PlayerController.canInteract = false;
        fader.SetTrigger("in");

        yield return new WaitForSeconds(1f);
        callback?.Invoke();

        fader.SetTrigger("out");
        PlayerController.canInteract = true;
        PlayerController.canMove = true;

        yield return new WaitForSeconds(1f);
        onFadeFinished?.Invoke();
    }

    public void EnableFader()
    {
        if (loadingScreen.activeInHierarchy) { return; }
        PlayerController.canMove = false;
        PlayerController.canInteract = false;
        fader.SetTrigger("in");
    }

    public void DisableFader()
    {
        fader.SetTrigger("out");
        PlayerController.canMove = true;
        PlayerController.canInteract = true;
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.popSounds);
        onFadeFinished?.Invoke();
    }

    private void SetRandomColor()
    {
        int i = UnityEngine.Random.Range(0, colors.Count);
        loadingWheel.color = colors[i];
    }

    private void SetRandomText()
    {
        int i = UnityEngine.Random.Range(0, lines.Count);
        loadingText.text = lines[i];
    }

    private void SetRandomInfoText()
    {
        int i = UnityEngine.Random.Range(0, infoLines.Count);
        infoText.text = infoLines[i];
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        EnableLoading();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.2f);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        DisableLoading();
    }
}
