using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{

    [Header("Settings")] public float normalCaveLightIntensity = 0.2f;
    
    private GameObject player;
    private GameObject playerLightSource;
    private Light2D globalLight;
    
    void Start()
    {
        GameObject globalLightObject = GameObject.FindWithTag("GlobalLight");
        globalLight = globalLightObject.GetComponent<Light2D>();
        
        player = LocalGameManager.Instance.PlayerController.gameObject;
        Transform childTransform = player.transform.Find("LightSource");

        playerLightSource = childTransform.gameObject;
    }

    public void ChangeLight(string sceneName)
    {
        if (globalLight != null)
        {
            if (sceneName == "Cave")
            {
                globalLight.intensity = normalCaveLightIntensity;
                playerLightSource.SetActive(true);
            }
            else if (sceneName == "DarkCave")
            {
                globalLight.intensity = 0f;
                playerLightSource.SetActive(true);
            }
            else if (sceneName == "Singleplayer")
            {
                globalLight.intensity = 1f;
                playerLightSource.SetActive(true);
            }
        }
    }

    public void DarkCaveEntered()
    {
        StartCoroutine(ChangeLightWithDelay("DarkCave"));
    }

    public void NormalCaveEntered()
    {
        StartCoroutine(ChangeLightWithDelay("Cave"));
    }
    
    public void OuterWorldEntered()
    {
        StartCoroutine(ChangeLightWithDelay("Singleplayer"));
    }

    private IEnumerator ChangeLightWithDelay(string lightType)
    {
        yield return new WaitForSeconds(0.5f);
        ChangeLight(lightType);               
    }
}
