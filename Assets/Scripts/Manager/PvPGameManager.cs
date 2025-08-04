using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PvPGameManager : MonoBehaviour
{
    public string scene;
    public GameObject pausePanel, _pauseButton, tutorialPanel;
    public Text KOText;
    public bool isPaused, isEnd;
    [SerializeField] Transform pos1, pos2;
    public static PvPGameManager managerInstance;
    public GameObject spotLight;
    private void Awake()
    {
        managerInstance = this;
    }
    private void Start()
    {
        tutorialPanel.SetActive(true);
        isEnd = false;
        KOText.enabled = false;
        pausePanel.SetActive(false);
        spotLight.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            tutorialPanel.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (Time.timeScale == 1)
        {
            pausePanel.SetActive(false);
            _pauseButton.SetActive(true);
        }
        if(CharacterSelectionManager.isSelectedPlayer2)
            spotLight.SetActive(true);

    }
    public void KOScreen() //slow down time on KO Screen
    {
        isEnd = true;
        Time.timeScale = 0.5f;
        KOText.enabled = true;
        StartCoroutine(end());
    }
    public void pauseButton() 
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.click);
        if (isPaused) //If it stopped when the pauseButton was pressed
        {
            pausePanel.SetActive(false);
            isPaused = false;
            Time.timeScale = 1; //continue
        }
        else //If it not stopped when the pauseButton was pressed
        {
            pausePanel.SetActive(true);
            isPaused = true;
            Time.timeScale = 0; //stop
        }
    }
    public void pause()
    {

        isPaused = true;
        Time.timeScale = 0;
    }
    public void resume()
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.click);

        isPaused = false;
        Time.timeScale = 1;
    }

    IEnumerator end()
    {
         yield return new WaitForSeconds(2.5f);
         SceneManager.LoadScene(scene);
    }
    public void quit()
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.click);

        Application.Quit();
    }
}
