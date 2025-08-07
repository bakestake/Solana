using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameButton : MonoBehaviour
{
    [Header("Settings")]
    public int cooldownDuration = 3;
    [Header("References")]
    public Button saveGameButton;
    public Animator saveButtonAnimator;

    public void SaveGame()
    {
        LocalSaveManager.instance.SaveEverything();
        StartCoroutine(HandleButtonRoutine());

        LocalSaveManager.instance.CloudSave(false);
    }

    public void SaveSuccessful()
    {
        // saveButtonAnimator.SetTrigger("save");
        SoundManager.instance.PlaySfx(SoundManager.instance.saveGame);
    }

    public void SaveFailed()
    {
        // saveButtonAnimator.SetTrigger("fail");
        SoundManager.instance.PlaySfx(SoundManager.instance.saveGameFail);
    }

    private IEnumerator HandleButtonRoutine()
    {
        saveGameButton.interactable = false;
        yield return new WaitForSeconds(cooldownDuration);
        saveGameButton.interactable = true;
    }
}
