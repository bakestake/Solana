using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class AltarOfTheDeadQuestStep3 : QuestStep
{
    [Header("Wave Settings")]
    [SerializeField] private int requiredZombieKills = 15;
    [SerializeField] private int[] zombiesPerWaveArray = new int[] { 3, 5, 7 };

    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float minDistanceBetweenZombies = 1.5f; // Minimum distance between spawned zombies
    private Transform[] spawnPoints;
    private List<Vector3> usedSpawnPositions = new List<Vector3>();

    private int defeatedZombies;
    private int currentWave;
    private int totalWaves;
    private bool isInCaveLevel2 = false;

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
        GameEventsManager.instance.miscEvents.onInteracted += OnInteracted;
        GameEventsManager.instance.combatEvents.onEnemyDefeated += OnZombieDefeated;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onInteracted -= OnInteracted;
        GameEventsManager.instance.combatEvents.onEnemyDefeated -= OnZombieDefeated;
    }

    private void OnInteracted(Interact go)
    {
        // Initialize spawn points when player enters Cave Level 2
        if (go.name.Contains("CaveLevel2") || go.gameObject.name == "CaveLevel2Door")
        {
            isInCaveLevel2 = true;
            InitializeSpawnPoints();
            if (defeatedZombies < requiredZombieKills)
            {
                SpawnNextWave();
            }
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

        // Clear used positions from previous waves
        usedSpawnPositions.Clear();

        // Find bounds before spawning zombies
        BoxCollider2D zombieBounds = GameObject.Find("ZombieBounds")?.GetComponent<BoxCollider2D>();
        if (zombieBounds == null)
        {
            Debug.LogError("ZombieBounds not found in scene!");
            return;
        }

        // Find or create a parent object for this wave's zombies
        GameObject waveParent = GameObject.Find("ZombieSpawnPoints");
        if (waveParent == null)
        {
            Debug.LogError("ZombieSpawnPoints not found in scene!");
            return;
        }

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition(zombieBounds);

            // Spawn the zombie and set parent
            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity, waveParent.transform);
            zombie.name = $"Zombie_Wave{currentWave + 1}_{i + 1}";

            // Add position to used positions
            usedSpawnPositions.Add(spawnPos);

            // Set up the NPC behavior
            NPCBehavior npcBehavior = zombie.GetComponent<NPCBehavior>();
            if (npcBehavior != null)
            {
                npcBehavior.boundsCollider2D = zombieBounds;
                npcBehavior.canAttack = true;
            }
        }
    }

    private Vector3 GetValidSpawnPosition(BoxCollider2D bounds)
    {
        const int MAX_ATTEMPTS = 30;
        int attempts = 0;

        while (attempts < MAX_ATTEMPTS)
        {
            // Pick a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Calculate random offset
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 potentialPos = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);

            // Check if position is valid
            if (IsValidSpawnPosition(potentialPos, bounds))
            {
                return potentialPos;
            }

            attempts++;
        }

        // If we couldn't find a valid position, use a spawn point position directly
        Debug.LogWarning("Could not find valid spawn position, using spawn point directly.");
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    private bool IsValidSpawnPosition(Vector3 position, BoxCollider2D bounds)
    {
        // Check if position is within bounds
        if (!bounds.bounds.Contains(position))
        {
            return false;
        }

        // Check distance from other zombies
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

        // If we've completed all waves, show final state
        if (currentWave >= totalWaves)
        {
            stateText = $"{defeatedZombies}/{requiredZombieKills}";
            statusText = "All waves completed!";
            ChangeState(stateText, statusText);
            return;
        }

        // Otherwise show current wave progress
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

            // Calculate how many zombies we've defeated in the current wave
            int zombiesDefeatedInCurrentWave = defeatedZombies;
            if (currentWave > 0)
            {
                zombiesDefeatedInCurrentWave -= zombiesPerWaveArray.Take(currentWave).Sum();
            }

            // Check if we've completed the current wave
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
                // Calculate current wave based on defeated zombies
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
