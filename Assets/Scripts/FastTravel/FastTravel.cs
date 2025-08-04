using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FastTravel : MonoBehaviour
{
    [SerializeField] private string stationName;
    [SerializeField] private Button fastTravelButton;
    [SerializeField] private Animator fastTravelPanel;
    [SerializeField] private GameObject fastTravelPanelObject;

    private string currentSceneName;
    private GameObject player;
    private Transform fastTravelPosition;
    private LocalGameManager localGameManager;
    private Animator anim;

    public string StationName => stationName;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        currentSceneName = SceneManager.GetActiveScene().name;
        fastTravelPosition = GameObject.FindGameObjectWithTag(stationName).transform;
        fastTravelButton.onClick.AddListener(Travel);
    }

    private void Start()
    {
        localGameManager = LocalGameManager.NotNullInstance;
        player = localGameManager.PlayerController.gameObject;
    }

    public void Travel()
    {
        if (currentSceneName == "Game-Singleplayer")
        {
            StartCoroutine(TravelRoutine());
        }
    }

    private IEnumerator TravelRoutine()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.transition);
        localGameManager.transitionAnimator.SetTrigger("in");
        try
        {
            fastTravelPanel.SetTrigger("out");
        }
        catch
        {
            if (anim != null)
            {
                anim.SetTrigger("out");
            }
        }

        yield return new WaitForSeconds(.5f);

        player.transform.position = fastTravelPosition.position;
        PlayerController.direction = new Vector2(0, -1);
        PlayerController.canMove = true;

        localGameManager.transitionAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.fastTravelOut);
    }
}
