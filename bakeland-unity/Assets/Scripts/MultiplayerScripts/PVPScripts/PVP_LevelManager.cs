using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PVP_LevelManager : NetworkSceneManagerDefault
{
    [HideInInspector]
    public Fusion_Launcher Launcher;
    //[SerializeField] private LoadingManager _loadingManager;
    private Scene _loadedScene;

    public void ResetLoadedScene()
    {
        //_loadingManager.ResetLastLevelsIndex();
        _loadedScene = default;
    }

    protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
    {
        //GameManager.Instance.SetGameState(GameManager.GameState.Loading);
        Launcher.SetConnectionStatus(Fusion_Launcher.ConnectionStatus.Loading, "");
        yield return new WaitForSeconds(1.0f);
        yield return base.LoadSceneCoroutine(sceneRef, sceneParams);
        Launcher.SetConnectionStatus(Fusion_Launcher.ConnectionStatus.Loaded, "");
        yield return new WaitForSeconds(1f);

    }
}
