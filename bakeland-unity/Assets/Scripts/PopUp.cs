using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField] GameObject popupPanel;
    bool isPlayerNear;

    void Start()
    {
        isPlayerNear = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPlayerNear)
        {
            popupPanel.SetActive(true);
            PlayerController.canMove = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPlayerNear)
        {
            Close();
        }
    }

    public void Close()
    {
        popupPanel.SetActive(false);
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
