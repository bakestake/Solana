using UnityEngine;

public class KillPlayerCaster : MonoBehaviour
{
    public void Trigger()
    {
        PlayerController player = LocalGameManager.Instance.PlayerController;
        Health health = player.GetComponent<Health>();
        health.Die();
    }
}
