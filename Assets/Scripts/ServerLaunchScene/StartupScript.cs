using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class StartupScript : MonoBehaviour
{
    public string Client;
    public string Server;
    // Start is called before the first frame update
    void Start()
    {
        if (System.Environment.GetCommandLineArgs().Any((arg) => arg == "-port"))
        {
            Debug.Log("Starting Server");
            SceneManager.LoadScene("ServerScene");

        }
        else
        {
            Debug.Log("Starting Client");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
