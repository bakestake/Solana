using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDying = false;
    public bool canGetHit = true;

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged;

    public delegate void OnDamageTaken(Vector3 damageSourcePosition);
    public event OnDamageTaken onDamageTaken;

    void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, Vector3 damageSourcePosition)
    {
        if (!canGetHit) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Trigger visual hit effect
        Material mat = GetComponent<Renderer>().material;
        mat.SetFloat("_HitEffectBlend", 1);
        mat.DOComplete();
        mat.DOFloat(0, "_HitEffectBlend", 0.25f);

        // Notify subscribers about damage taken
        onDamageTaken?.Invoke(damageSourcePosition);

        if (currentHealth <= 0)
        {
            if (gameObject.CompareTag("Enemy"))
            {
                GameEventsManager.instance.combatEvents.EnemyDefeated(gameObject);
                SoundManager.instance.PlayRandomFromList(SoundManager.instance.zombieDeathSounds);
                GetComponent<NPCBehavior>().masterBoolEnabled = false;
            }

            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (isDying) yield break;
        isDying = true;

        // Immediately disable collisions and combat
        if (gameObject.CompareTag("Player"))
        {
            PlayerController.canMove = false;
            GetComponent<Collider2D>().enabled = false;
            canGetHit = false;
            // Disable player from being targeted/hit
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
            canGetHit = false;
            if (TryGetComponent<NPCBehavior>(out var npc))
            {
                npc.masterBoolEnabled = false;
            }
        }

        // Visual fade effect
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            float fadeTime = 1.5f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float fadeAmount = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
                mat.SetFloat("_FadeAmount", fadeAmount);
                yield return null;
            }

            mat.SetFloat("_FadeAmount", 1f);
            yield return new WaitForSeconds(0.1f);
        }

        if (gameObject.CompareTag("Player"))
        {
            LoadingWheel.instance.EnableFader();
            yield return new WaitForSeconds(1f);

            // Get all loaded scenes and unload them except the main scene
            int sceneCount = SceneManager.sceneCount;
            for (int i = sceneCount - 1; i >= 0; i--)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Game-Singleplayer")
                {
                    yield return SceneManager.UnloadSceneAsync(scene);
                }
            }

            // Reset player state
            transform.position = LocalGameManager.Instance.playerSpawnPoint.position;
            currentHealth = maxHealth;
            onHealthChanged?.Invoke(currentHealth, maxHealth);
            GetComponent<Collider2D>().enabled = true;
            canGetHit = true;
            PlayerController.canMove = true;
            gameObject.layer = LayerMask.NameToLayer("Player"); // Reset layer

            // Reset visual effects
            if (renderer != null)
            {
                Material mat = GetComponent<Renderer>().material;
                mat.SetFloat("_FadeAmount", 0f);
            }

            yield return new WaitForSeconds(1f);
            LoadingWheel.instance.DisableFader();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
