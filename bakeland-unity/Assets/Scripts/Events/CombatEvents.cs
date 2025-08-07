using UnityEngine;
using UnityEngine.Events;

public class CombatEvents
{
    public UnityAction<GameObject> onEnemyDefeated;

    public void EnemyDefeated(GameObject enemy)
    {
        onEnemyDefeated?.Invoke(enemy);
    }
}