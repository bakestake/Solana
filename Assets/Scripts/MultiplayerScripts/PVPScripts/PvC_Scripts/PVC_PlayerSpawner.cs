using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PVC_PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> PlayerPrefab;

    public List<Transform> _spawnPoints;

    public  TMP_InputField nicknameInputField;

    private GameObject _prefabCharacter;

    //private bool _gameIsReady = false;
    private PVC_GameStateController _gameStateController = null;

    public void Start()
    {
    }

    public void SetPrefab(int CharacterNumber)
    {
        if (CharacterNumber == 0)
        {
            _prefabCharacter = PlayerPrefab[0];
        }
        else if (CharacterNumber == 1)
        {
            _prefabCharacter = PlayerPrefab[1];
        }
        else
        {
            _prefabCharacter = PlayerPrefab[2];
        }

    }

    // The spawner is started when the GameStateController switches to GameState.Running.
    public void StartplayerSpawner(PVC_GameStateController gameStateController)
    {
        //_gameIsReady = true;
        _gameStateController = gameStateController;
        Spawnplayer();
    }

    // Spawns a player for a player.
    // The spawn point is chosen in the _spawnPoints array using the implicit playerRef to int conversion 
    private void Spawnplayer()
    {
        var spawnPosition = _spawnPoints[0];

        var playerObject = Instantiate(_prefabCharacter, spawnPosition.position, Quaternion.identity);
        //Setting it for Player Inputs
        if (playerObject != null)
        {
            playerObject.GetComponent<PvC_PlayerMovement>().isPlayer = true;
            playerObject.GetComponent<PvC_Health_Playerr>().SetNickName(nicknameInputField.text);
            playerObject.GetComponent<PvC_Health_Playerr>().DelayStart();
        }
        // Add the new player to the players to be tracked for the game end check.
        _gameStateController.TrackNewPlayer(playerObject.GetComponent<PvC_Health_Playerr>());

        //NPC
        int randomNumber = Random.Range(0, 2);

        var NPCObject = Instantiate(PlayerPrefab[randomNumber], _spawnPoints[1].position, Quaternion.identity);

        if (NPCObject != null)
        {
            NPCObject.GetComponent<PvC_PlayerMovement>().isNPC = true;
            NPCObject.GetComponent<PvC_Health_Playerr>().SetNickName("Terminator");
            NPCObject.GetComponent<PvC_Health_Playerr>().DelayStart();
            NPCObject.gameObject.tag = "NPC";
        }

        _gameStateController.TrackNewPlayer(NPCObject.GetComponent<PvC_Health_Playerr>());
    }

}
