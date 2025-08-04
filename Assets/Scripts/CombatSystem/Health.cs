using Cinemachine;
using DG.Tweening;
using Gamegaard.UI.PopupText;
using System.Collections;
using UnityEngine;

public delegate void OnHealthChanged(int currentHealth, int maxHealth);
public delegate void OnDamageTaken(Vector3 damageSourcePosition);

public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject enemyHitParticle;
    [SerializeField] private GameObject enemyDamageParticle;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private HealthBar enemyHealthBar;
    [SerializeField] private PoupUpTextSpawner poupUpTextSpawner;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool canGetHit = true;

    public const string TagEnemy = "Enemy";
    public const string TagPlayer = "Player";
    public const string Shader_HitEffectBlend = "_HitEffectBlend";
    public const string Shader_Alpha = "_Alpha";
    public const string Shader_FadeAmount = "_FadeAmount";

    private DamageFlash damageFlash;
    private Collider2D cachedCollider;
    private Renderer cachedRenderer;
    private Material material;
    private bool isDying = false;
    private bool isEnemy;
    private bool isPlayer;

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (currentHealth == value) return;

            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
    public int MaxHealth => maxHealth;
    public bool CanGetHit => canGetHit;

    public event OnHealthChanged OnHealthChanged;
    public event OnDamageTaken OnDamageTaken;

    private void Awake()
    {
        damageFlash = GetComponent<DamageFlash>();

        if (!TryGetComponent(out cachedRenderer))
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
        }

        cachedCollider = GetComponent<Collider2D>();
        material = cachedRenderer.material;

        isEnemy = CompareTag(TagEnemy);
        isPlayer = CompareTag(TagPlayer);
    }

    private void Start()
    {
        CurrentHealth = maxHealth;

        if (isEnemy && enemyHealthBar != null)
        {
            enemyHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage, Vector3 damageSourcePosition)
    {
        if (!canGetHit || damage <= 0) return;

        poupUpTextSpawner.InstantiateText(damage);

        CurrentHealth -= damage;

        if (cachedRenderer != null)
        {
            Material mat = cachedRenderer.material;
            mat.SetFloat(Shader_HitEffectBlend, 1);
            mat.DOComplete();
            mat.DOFloat(0, Shader_HitEffectBlend, 0.25f);
        }

        OnDamageTaken?.Invoke(damageSourcePosition);

        if (currentHealth <= 0)
        {
            if (isEnemy)
            {
                GameEventsManager.Instance.combatEvents.EnemyDefeated(gameObject);
                SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.zombieDeathSounds);
                GetComponent<Enemy>().enabled = false;

                if (TryGetComponent(out WanderingMovementHandler movement))
                {
                    movement.enabled = false;
                }
            }

            StartCoroutine(OnDie());
        }

        if (isEnemy)
        {
            Vector2 damageDirection = (transform.position - damageSourcePosition).normalized;
            SpawnParticles(damageDirection);
            TriggerCameraShake(-damageDirection);

            if (damageFlash != null)
            {
                damageFlash.FlashSprite();
            }
            if (enemyHealthBar != null)
            {
                enemyHealthBar.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        if (isPlayer)
        {
            SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.playerHitSounds);

            if (damageFlash != null)
            {
                damageFlash.FlashSprite();
            }
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        CurrentHealth += amount;

        if (isEnemy && enemyHealthBar != null)
        {
            enemyHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    private void SpawnParticles(Vector2 damageDirection)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, damageDirection);

        if (enemyDamageParticle != null)
        {
            Instantiate(enemyDamageParticle, transform.position, spawnRotation);
        }

        if (enemyHitParticle != null)
        {
            Instantiate(enemyHitParticle, transform.position, Quaternion.identity);
        }
    }

    private void TriggerCameraShake(Vector2 shakeDirection)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(shakeDirection);
        }
        else
        {
            Debug.LogWarning("No CinemachineImpulseSource assigned to this object!");
        }
    }

    [ContextMenu("Force Die")]
    public void Die()
    {
        StartCoroutine(OnDie());
    }

#if UNITY_EDITOR
    [ContextMenu("Force Die", true)]
    private bool TestDieCondition()
    {
        return UnityEditor.EditorApplication.isPlaying;
    }
#endif

    private IEnumerator OnDie()
    {
        if (isDying) yield break;
        isDying = true;
        SoundManager.Instance.PlaySfx(deathSound);
        if (isPlayer)
        {
            PlayerController.canMove = false;
            cachedCollider.enabled = false;
            canGetHit = false;
        }
        else
        {
            cachedCollider.enabled = false;
            canGetHit = false;
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Die();
            }
        }

        if (cachedRenderer != null)
        {
            float fadeTime = 1.5f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float fadeAmount = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                material.SetFloat(Shader_Alpha, fadeAmount);
                yield return null;
            }

            material.SetFloat(Shader_Alpha, 0f);
            yield return new WaitForSeconds(0.1f);
        }

        if (isPlayer)
        {
            LoadingWheel.instance.EnableFader();
            yield return new WaitForSeconds(1f);
            LocalGameManager.Instance.FracturedRealmsTimer.ResetTimer();
            LocalGameManager.Instance.FracturedRealmsTimer.gameObject.SetActive(false);
            SceneLoader sceneLoader = GetComponent<SceneLoader>();
            sceneLoader.LoadSceneInterior("Void");

            transform.position = LocalGameManager.Instance.playerSpawnPoint.position;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            cachedCollider.enabled = true;
            canGetHit = true;
            PlayerController.canMove = true;

            if (cachedRenderer != null)
            {
                cachedRenderer.material.SetFloat(Shader_FadeAmount, 0f);
                material.SetFloat(Shader_Alpha, 1);
            }

            yield return new WaitForSeconds(1f);
            LoadingWheel.instance.DisableFader();
            isDying = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}