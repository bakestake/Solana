using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float waitBeforeFade = 1.5f;

    [SerializeField] private bool useInteriorScenePoint;
    [SerializeField] private int scenePoint;

    private GlobalLightManager globalLightManager;
    private static string currentInteriorScene;

    public static bool IsInteriorLoaded { get; private set; }

    private void Start()
    {
        globalLightManager = FindObjectOfType<GlobalLightManager>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private void OnSceneLoaded(AsyncOperation obj)
    {
        LoadingWheel.instance.DisableLoading();
    }

    public void LoadSceneInterior(string sceneName)
    {
        LoadingWheel.instance.StartCoroutine(HandleInteriorScene(sceneName));
    }

    public void ForceUnloadCurrentInterior()
    {
        if (IsInteriorLoaded && !string.IsNullOrEmpty(currentInteriorScene))
        {
            if (SceneManager.GetSceneByName(currentInteriorScene).isLoaded)
            {
                SceneManager.UnloadSceneAsync(currentInteriorScene);
            }

            IsInteriorLoaded = false;
            currentInteriorScene = null;
        }
    }

    private void OnSceneLoadedInterior(AsyncOperation obj)
    {
        LoadingWheel.instance.DisableFader();
        IsInteriorLoaded = true;
    }

    public void UnloadCurrentInterior()
    {
        if (IsInteriorLoaded && !string.IsNullOrEmpty(currentInteriorScene))
        {
            StartCoroutine(UnloadSceneInteriorAsync(currentInteriorScene));
        }
    }

    private void OnSceneUnloadedInterior(AsyncOperation obj)
    {
        LoadingWheel.instance.DisableFader();
        IsInteriorLoaded = false;
        currentInteriorScene = null;
    }

    public void PlaySound(AudioClip clip)
    {
        SoundManager.Instance.PlaySfx(clip);
    }

    public void EnableReverb()
    {
        SoundManager.Instance.EnableReverb();
    }

    public void DisableReverb()
    {
        SoundManager.Instance.DisableReverb();
    }

    public void Legacy_UnloadSceneInterior(string sceneName)
    {
        StartCoroutine(Legacy_UnloadSceneInteriorAsync(sceneName));
    }

    private IEnumerator HandleInteriorScene(string newScene)
    {
        LoadingWheel.instance.EnableFader();
        yield return new WaitForSeconds(waitBeforeFade);

        if (!string.IsNullOrEmpty(currentInteriorScene) && SceneManager.GetSceneByName(currentInteriorScene).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentInteriorScene);
            yield return asyncUnload;
        }

        yield return LoadSceneInteriorAsync(newScene);
    }

    private IEnumerator LoadSceneInteriorAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.completed += OnSceneLoadedInterior;

        if (globalLightManager)
        {
            globalLightManager.ChangeLight(sceneName);
        }

        currentInteriorScene = sceneName;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid() && loadedScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedScene);

            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                InteriorManager interiorManager = obj.GetComponentInChildren<InteriorManager>();
                if (interiorManager != null)
                {
                    if (useInteriorScenePoint && scenePoint > 0 && scenePoint < interiorManager.SpawnPositions.Length)
                    {
                        PlayerController.Instance.transform.position = interiorManager.SpawnPositions[scenePoint].position;
                    }

                    break;
                }
            }
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        LoadingWheel.instance.EnableLoading();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.completed += OnSceneLoaded;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadSceneInteriorAsync(string sceneName)
    {
        LoadingWheel.instance.EnableFader();
        yield return new WaitForSeconds(waitBeforeFade);

        InteriorManager interiorManager = FindObjectOfType<InteriorManager>();
        if (interiorManager != null && interiorManager.ExitPosition != null)
        {
            PlayerController.Instance.transform.position = interiorManager.ExitPosition.position;
        }

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        asyncUnload.completed += OnSceneUnloadedInterior;

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator Legacy_UnloadSceneInteriorAsync(string sceneName)
    {
        LoadingWheel.instance.EnableFader();
        yield return new WaitForSeconds(waitBeforeFade);

        InteriorManager interiorManager = FindObjectOfType<InteriorManager>();
        if (interiorManager.ExitPosition != null)
        {
            PlayerController.Instance.transform.position = interiorManager.ExitPosition.position;
        }

        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
        asyncLoad.completed += OnSceneUnloadedInterior;
    }
}