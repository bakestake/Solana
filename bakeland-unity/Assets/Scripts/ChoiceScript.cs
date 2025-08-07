using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoiceScript : MonoBehaviour
{
    public Button SingleplayerButton;

    public void OpenSinglePlayerScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
