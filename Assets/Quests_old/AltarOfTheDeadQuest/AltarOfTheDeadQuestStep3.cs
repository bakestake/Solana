using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AltarOfTheDeadQuestStep3 : QuestStep
{
    [Header("Wave Settings")]
    [SerializeField] private int requiredZombieKills = 15;
    [SerializeField] private int[] zombiesPerWaveArray = new int[] { 3, 5, 7 };

    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float minDistanceBetweenZombies = 1.5f;

    private Transform[] spawnPoints;
    private List<Vector3> usedSpawnPositions = new List<Vector3>();

    private int defeatedZombies;
    private int currentWave;
    private int totalWaves;
    private bool isYeetioInsideCave = true;
    public bool IsInCaveLevel2 { get; private set; }

    private void Start()
    {
        defeatedZombies = 0;
        currentWave = 0;
        totalWaves = zombiesPerWaveArray.Length;
        UpdateState();
        InitializeSpawnPoints();
        SpawnNextWave();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted += OnInteracted;
        GameEventsManager.Instance.combatEvents.onEnemyDefeated += OnZombieDefeated;
        SceneManager.sceneLoaded += (x, y) => OnSceneLoaded(x);
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted -= OnInteracted;
        GameEventsManager.Instance.combatEvents.onEnemyDefeated -= OnZombieDefeated;
        SceneManager.sceneLoaded -= (x, y) => OnSceneLoaded(x);
    }

    private void OnInteracted(Interact go)
    {
        if (go.name.Contains("CaveLevel2"))
        {
            IsInCaveLevel2 = true;
            InitializeSpawnPoints();
            if (defeatedZombies < requiredZombieKills)
            {
                SpawnNextWave();
            }
        }
    }

    private void OnSceneLoaded(Scene scene)
    {
        if (scene.name.Contains("Cave"))
        {
            var yeetio = scene.GetRootGameObjects().FirstOrDefault(x => x.name.Contains("Yeetio"));
            if (isYeetioInsideCave) yeetio.SetActive(true);
        }
    }

    public void InitializeSpawnPoints()
    {
        GameObject spawnPointsParent = GameObject.Find("ZombieSpawnPoints");
        if (spawnPointsParent != null)
        {
            spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>()
                .Where(t => t != spawnPointsParent.transform)
                .ToArray();

            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points found in CaveLevel2!");
            }
        }
        else
        {
            Debug.LogError("ZombieSpawnPoints not found in CaveLevel2!");
        }
    }

    private void SpawnNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("All waves completed!");
            return;
        }

        int zombiesToSpawn = zombiesPerWaveArray[currentWave];
        Debug.Log($"Spawning wave {currentWave + 1} with {zombiesToSpawn} zombies.");

        usedSpawnPositions.Clear();

        BoxCollider2D zombieBounds = GameObject.Find("ZombieBounds")?.GetComponent<BoxCollider2D>();
        if (zombieBounds == null)
        {
            Debug.LogError("ZombieBounds not found in scene!");
            return;
        }

        GameObject waveParent = GameObject.Find("ZombieSpawnPoints");
        if (waveParent == null)
        {
            Debug.LogError("ZombieSpawnPoints not found in scene!");
            return;
        }

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition(zombieBounds);

            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity, waveParent.transform);
            zombie.name = $"Zombie_Wave{currentWave + 1}_{i + 1}";

            usedSpawnPositions.Add(spawnPos);

            if (zombie.TryGetComponent(out Enemy npcBehavior))
            {
                if (zombie.TryGetComponent(out WanderingMovementHandler movementHandler))
                {
                    movementHandler.SetBounds(zombieBounds);
                }
                npcBehavior.CanAttack = true;
            }
        }
    }

    private Vector3 GetValidSpawnPosition(BoxCollider2D bounds)
    {
        const int MAX_ATTEMPTS = 30;
        int attempts = 0;

        while (attempts < MAX_ATTEMPTS)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 potentialPos = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);

            if (IsValidSpawnPosition(potentialPos, bounds))
            {
                return potentialPos;
            }

            attempts++;
        }

        Debug.LogWarning("Could not find valid spawn position, using spawn point directly.");
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    private bool IsValidSpawnPosition(Vector3 position, BoxCollider2D bounds)
    {
        if (!bounds.bounds.Contains(position))
        {
            return false;
        }

        foreach (Vector3 usedPos in usedSpawnPositions)
        {
            if (Vector3.Distance(position, usedPos) < minDistanceBetweenZombies)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateState()
    {
        string stateText;
        string statusText;

        if (currentWave >= totalWaves)
        {
            stateText = $"{defeatedZombies}/{requiredZombieKills}";
            statusText = "All waves completed!";
            ChangeState(stateText, statusText);
            return;
        }

        int currentWaveZombies = zombiesPerWaveArray[currentWave];
        int waveProgress = defeatedZombies - (currentWave > 0 ?
            zombiesPerWaveArray.Take(currentWave).Sum() : 0);

        stateText = $"{defeatedZombies}/{requiredZombieKills}";
        statusText = $"Wave {currentWave + 1}/{totalWaves}: Defeat {waveProgress}/{currentWaveZombies} zombies";
        ChangeState(stateText, statusText);
    }

    private void OnZombieDefeated(GameObject enemy)
    {
        Debug.Log($"Enemy defeated: {enemy.name}");

        if (enemy.CompareTag("Enemy") && enemy.name.Contains("Zombie"))
        {
            defeatedZombies++;
            Debug.Log($"Zombie defeated! Progress: {defeatedZombies}/{requiredZombieKills}");

            int zombiesDefeatedInCurrentWave = defeatedZombies;
            if (currentWave > 0)
            {
                zombiesDefeatedInCurrentWave -= zombiesPerWaveArray.Take(currentWave).Sum();
            }

            if (zombiesDefeatedInCurrentWave >= zombiesPerWaveArray[currentWave])
            {
                currentWave++;
                if (currentWave < totalWaves)
                {
                    Debug.Log($"Wave {currentWave} completed! Starting next wave...");
                    SpawnNextWave();
                }
                else if (defeatedZombies >= requiredZombieKills)
                {
                    Debug.Log("All waves completed!");
                    FinishQuestStep();
                }
            }

            UpdateState();
        }
    }

    protected override void SetQuestStepState(string state)
    {
        if (state.Contains("/"))
        {
            string[] parts = state.Split('/');
            if (parts.Length == 2 && int.TryParse(parts[0], out int defeated))
            {
                defeatedZombies = defeated;
                currentWave = 0;
                int sum = 0;
                for (int i = 0; i < zombiesPerWaveArray.Length; i++)
                {
                    sum += zombiesPerWaveArray[i];
                    if (defeatedZombies >= sum)
                    {
                        currentWave = i + 1;
                    }
                }
            }
        }
    }
}