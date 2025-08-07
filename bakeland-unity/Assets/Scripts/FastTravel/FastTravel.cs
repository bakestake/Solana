using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FastTravel : MonoBehaviour
{
    public Button FastTravelButton;
    //public Bus Bus;
    public string stationName;
    public GameObject fastTravelPanel;

    private GameObject player;
    private string currentSceneName;
    private Transform FastTravelPosition;

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (FastTravelPosition == null)
        {
            FastTravelPosition = GameObject.FindGameObjectWithTag(stationName).transform;
            // Debug.Log("This is set" + FastTravelPosition);
        }
        if (FastTravelButton == null)
        {
            FastTravelButton = GetComponent<Button>();
            // Debug.Log("This is also set" + FastTravelButton);
        }
        FastTravelButton.onClick.AddListener(Travel);
    }

    public void Travel()
    {
        // Debug.Log("This is working");
        if (currentSceneName == "Game-Singleplayer")
        {
            StartCoroutine(TravelRoutine());
        }
        else
        {
            //Bus.StartFastTravel(FastTravelPosition.position);
        }
    }

    private IEnumerator TravelRoutine()
    {
        // Debug.Log("It is here");
        SoundManager.instance.PlaySfx(SoundManager.instance.transition);
        LocalGameManager.Instance.transitionAnimator.SetTrigger("in");
        fastTravelPanel.GetComponent<Animator>().SetTrigger("out");
        yield return new WaitForSeconds(.5f);

        player.transform.position = FastTravelPosition.position;
        PlayerController.direction = new Vector2(0, -1);
        PlayerController.canMove = true;
        Debug.Log("Can Move true");

        LocalGameManager.Instance.transitionAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.fastTravelOut);
    }
}
