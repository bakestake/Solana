using UnityEngine;

public class MusicChanger : MusicSetter
{
    protected const string PlayerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            Apply();
        }
    }
}