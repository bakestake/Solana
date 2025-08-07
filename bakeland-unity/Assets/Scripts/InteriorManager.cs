using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public Transform spawnPosition;
    public Transform exitPosition;

    void Start()
    {
        GameObject player = GameObject.FindAnyObjectByType<PlayerController>().gameObject;
        player.transform.position = spawnPosition.position;
    }
}
