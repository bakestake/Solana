using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class PVC_GameStateController : MonoBehaviour
{
    enum GameState
    {
        Starting,
        Running,
        Ending
    }

    //[SerializeField] private float _startDelay = 4.0f;
    [SerializeField] private float _endDelay = 4.0f;
    //[SerializeField] private float _gameSessionLength = 180.0f;

    [SerializeField] private TextMeshProUGUI _startEndDisplay = null;
    //[SerializeField] private TextMeshProUGUI _ingameTimerDisplay = null;

    private GameState _gameState { get; set; }

    private string _winner { get; set; }

    private List<PvC_Health_Playerr> _playerDataNetworkedIds = new List<PvC_Health_Playerr>();

    public void Start()
    {
        // --- This section is for all information which has to be locally initialized based on the networked game state
        // --- when a CLIENT joins a game

        //_startEndDisplay.gameObject.SetActive(true);
        //_ingameTimerDisplay.gameObject.SetActive(false);

        // If the game has already started, find all currently active players' PlayerDataNetworked component Ids
        if (_gameState != GameState.Starting)
        {
            
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player &&
                    _playerDataNetworkedIds.Contains(player.GetComponent<PvC_Health_Playerr>())) continue;
                TrackNewPlayer(player.GetComponent<PvC_Health_Playerr>());
            }
        }


        // Initialize the game state on the host
        _gameState = GameState.Starting;
        //_timer = TickTimer.CreateFromSeconds(Runner, _startDelay);
    }

    public void FixedUpdate()
    {
        // Update the game display with the information relevant to the current game state
        switch (_gameState)
        {
            case GameState.Starting:

                break;
            case GameState.Running:
                //UpdateRunningDisplay();
                // Ends the game if the game session length has been exceeded
                //if (_timer.ExpiredOrNotRunning(Runner))
                //{
                //GameHasEnded();
                //}

                break;
            case GameState.Ending:
                UpdateEndingDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateStartingDisplay()
    {
        // --- Host & Client
        // Display the remaining time until the game starts in seconds (rounded down to the closest full second)

        //_startEndDisplay.text = $"Game Starts In {Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0)}";

        FindObjectOfType<PVC_PlayerSpawner>().StartplayerSpawner(this);
        //FindObjectOfType<AsteroidSpawner>().StartAsteroidSpawner();

        // Switches to the Running GameState and sets the time to the length of a game session
        _gameState = GameState.Running;
        //_timer = TickTimer.CreateFromSeconds(Runner, _gameSessionLength);
    }

    /*private void UpdateRunningDisplay()
    {
        // --- Host & Client
        // Display the remaining time until the game ends in seconds (rounded down to the closest full second)
        _startEndDisplay.gameObject.SetActive(false);
        _ingameTimerDisplay.gameObject.SetActive(true);
        _ingameTimerDisplay.text =
            $"{Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0).ToString("000")} seconds left";
    }*/

    private void UpdateEndingDisplay()
    {
        // --- Host & Client
        // Display the results and
        // the remaining time until the current game session is shutdown

        _startEndDisplay.gameObject.SetActive(true);
        //_ingameTimerDisplay.gameObject.SetActive(false);
        _startEndDisplay.text =
            $"{_winner} won with {_playerDataNetworkedIds[0].currentHealth} points.";

        StartCoroutine(WinnerCoolDown());
    }

    // Called from the ShipController when it hits an asteroid
    public void CheckIfGameHasEnded()
    {
        int playersAlive = 0;

        for (int i = 0; i < _playerDataNetworkedIds.Count; i++)
        {
            if (_playerDataNetworkedIds[i].currentHealth <= 0)
            {
                _playerDataNetworkedIds.RemoveAt(i);
                i--;
                continue;
            }

            if (_playerDataNetworkedIds[i].currentHealth > 0) playersAlive++;
        }

        // If more than 1 player is left alive, the game continues.
        // If only 1 player is left, the game ends immediately.
        if (playersAlive > 1) return;

        foreach (var playerDataNetworkedId in _playerDataNetworkedIds)
        {
            _winner = playerDataNetworkedId.GetPlayerNickname();
        }

        GameHasEnded();
    }

    public void GameHasEnded()
    {
        _gameState = GameState.Ending;
    }

    public void TrackNewPlayer(PvC_Health_Playerr playerHealthClass)
    {
        _playerDataNetworkedIds.Add(playerHealthClass);
    }

    private IEnumerator WinnerCoolDown()
    {
        yield return new WaitForSeconds(_endDelay);
        SceneManager.LoadScene(1); // 1 is the SinglePlayer Main Scene

    }
}
