using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform exitPosition;
    [SerializeField] private Transform[] spawnPositions;

    public Transform ExitPosition => exitPosition;
    public Transform[] SpawnPositions => spawnPositions;

    private void Start()
    {
        Transform player = LocalGameManager.NotNullInstance.PlayerController.transform;
        player.position = spawnPosition.position;
    }
}