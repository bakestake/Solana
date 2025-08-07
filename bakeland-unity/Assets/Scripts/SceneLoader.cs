using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float waitBeforeFade = 0f;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        LoadingWheel.instance.EnableLoading();

        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.completed += OnSceneLoaded;

        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoaded(AsyncOperation obj)
    {
        Debug.Log("loaded");
        LoadingWheel.instance.DisableLoading();
    }

    public void LoadSceneInterior(string sceneName)
    {
        StartCoroutine(LoadSceneInteriorAsync(sceneName));
    }

    private IEnumerator LoadSceneInteriorAsync(string sceneName)
    {
        LoadingWheel.instance.EnableFader();
        yield return new WaitForSeconds(waitBeforeFade);

        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.completed += OnSceneLoadedInterior;

        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoadedInterior(AsyncOperation obj)
    {
        LoadingWheel.instance.DisableFader();
    }

    public void UnloadSceneInterior(string sceneName)
    {
        StartCoroutine(UnloadSceneInteriorAsync(sceneName));
    }

    private IEnumerator UnloadSceneInteriorAsync(string sceneName)
    {
        LoadingWheel.instance.EnableFader();
        yield return new WaitForSeconds(waitBeforeFade);

        InteriorManager interiorManager = GameObject.FindObjectOfType<InteriorManager>();
        if (interiorManager.exitPosition != null)
        {
            GameObject.FindObjectOfType<PlayerController>().transform.position = interiorManager.exitPosition.position;
        }

        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
        asyncLoad.completed += OnSceneUnloadedInterior;
    }

    private void OnSceneUnloadedInterior(AsyncOperation obj)
    {
        LoadingWheel.instance.DisableFader();
    }

    public void PlaySound(AudioClip clip)
    {
        SoundManager.instance.PlaySfx(clip);
    }

    public void EnableReverb()
    {
        SoundManager.instance.EnableReverb();
    }

    public void DisableReverb()
    {
        SoundManager.instance.DisableReverb();
    }
}
