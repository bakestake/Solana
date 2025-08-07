
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public GameObject police, farmer, stoner;
    public GameObject police2, farmer2, stoner2;
    public GameObject player1Buttons, player2buttons;
    public GameObject characterSeleectPanel;
    private GameObject Player1, Player2;
    private PvPGameManager gameManager;
    int buttoncount = 0;
    public static bool isSelectedPlayer2;
    private void Start()
    {
        Player1 = GameObject.FindGameObjectWithTag("Player");
        Player2 = GameObject.FindGameObjectWithTag("Player2");

        buttoncount = 0;
        gameManager = FindAnyObjectByType<PvPGameManager>();
        //PvPGameManager.managerInstance.pause();
        gameManager._pauseButton.SetActive(false);
        isSelectedPlayer2 = false;
        player1Buttons.SetActive(true);
        player2buttons.SetActive(false);

    }
    public void CharacterSelectButton(GameObject character) // When CharacterSelectButton is pressed, set the character parameter in the button to true,
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.click);

        // and set the remaining characters to false.    
        buttoncount++;

        if (buttoncount == 1)
        {
            character.SetActive(true); 
            Player1 = character;
            player1Buttons.SetActive(false);
            player2buttons.SetActive(true);

            //if (character.GetComponent<Damage2_Player>() != null && character.GetComponent<Player2_Movements>() != null)
            //{
            //    Player2.GetComponent<Damage2_Player>().SetPlayerReference(character); // **** Assign the selected character to the player reference in the Damage2_Player script !!!!!
            //    Player2.GetComponent<Player2_Movements>().SetPlayerReference(character); // **** Assign the selected character to the player reference in the Player2_Movements script !!!!!
            //}


        }
        if (buttoncount == 2)
        {
            character.SetActive(true); 
            Player2 = character;
            isSelectedPlayer2 = true;


            //if (character.GetComponent<Damage_Player>() != null && character.GetComponent<Player_Movements>() != null)
            //{
            //    character.GetComponent<Damage_Player>().SetPlayerReference(character); // **** Assign the selected character to the player reference in the Damage2_Player script !!!!!
            //    character.GetComponent<Player_Movements>().SetPlayerReference(character); // **** Assign the selected character to the player reference in the Player2_Movements script !!!!!
            //}
            AssignPlayerReferences();

            characterSeleectPanel.SetActive(false); //characterSeleectPanel close
            gameManager._pauseButton.SetActive(true);
            PvPGameManager.managerInstance.resume(); // Continue the game after character is selected

        }
    }
    private void AssignPlayerReferences()
    {
        //if (Player1 != null && Player2 != null)
        //{
        //    var player1Damage = Player1.GetComponent<Damage_Player>();
        //    var player1Movement = Player1.GetComponent<Player_Movements>();

        //    var player2Damage = Player2.GetComponent<Damage2_Player>();
        //    var player2Movement = Player2.GetComponent<Player2_Movements>();

        //    //if (player1Damage != null)
        //        //player1Damage.SetPlayerReference(Player2);

        //    if (player1Movement != null)
        //        player1Movement.SetPlayerReference(Player2);

        //    if (player2Damage != null)
        //        player2Damage.SetPlayerReference(Player1);

        //    if (player2Movement != null)
        //        player2Movement.SetPlayerReference(Player1);
        //}
        //else
        //{
        //    Debug.LogError("Players not set correctly.");
        //}
    }
}

