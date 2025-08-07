using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Fusion;
using UnityEngine.SocialPlatforms.Impl;

public class Health_Playerr : NetworkBehaviour
{
    [Header("Health")]
    private float maxHealth = 100.0f;

    [HideInInspector]
    [Networked]
    public float currentHealth { get; private set; }

    [HideInInspector]
    [Networked]
    public float currentDodge { get; private set; }
    public float maxDodge = 10.0f;

    private NetworkBool isDie { get; set; }

    private PVP_PlayerOverPanel playerOverviewPanel = null;
    private ChangeDetector _changeDetector;

    [HideInInspector]
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    //[Header("Health Bar")]
    //public GameObject healthBar;
    //public Image backgroundImg, healthImg;

    //[Header("Ultimate Bar")]
    //public Image backgrounUltidImg, UltimateImg;

    //[Header("Componnets")]
    //public GameObject canvas;

    private PvPGameManager GameManager;
    public Animator anim;

    //[Header("Damage Color")]
    //public Color damageColor = Color.red; // Color when damaged
    //public float damageFlashDuration = 0.1f; //Sprite color change duration when taking damage
    //private SpriteRenderer spriteRenderer;

    /*void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //GameManager = FindAnyObjectByType<PvPGameManager>();
        anim=GetComponent<Animator>();

        currentHealth = maxHealth;
        currentDodge = 0;
        isDie = false;
        //healthBar.SetActive(false);
        //canvas.SetActive(true);
    }
    private void FixedUpdate()
    {
        if (CharacterSelectionManager.isSelectedPlayer2)
        {
            healthBar.SetActive(true);
        }
    }*/

    public override void Spawned()
    {
        // --- Client
        // Find the local non-networked PlayerData to read the data and communicate it to the Host via a single RPC 
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<Mult_playerData>().GetNickName();
            RpcSetNickName(nickName);
        }

        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            isDie = false;
            currentHealth = maxHealth;
            currentDodge = maxDodge;
        }

        // --- Host & Client
        // Set the local runtime references.
        playerOverviewPanel = FindObjectOfType<PVP_PlayerOverPanel>();
        // Add an entry to the local Overview panel with the information of this spaceship
        playerOverviewPanel.AddEntry(Object.InputAuthority, this);

        // Refresh panel visuals in Spawned to set to initial values.
        playerOverviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());
        playerOverviewPanel.UpdateLives(Object.InputAuthority, currentHealth);
        playerOverviewPanel.UpdateScore(Object.InputAuthority, currentDodge);

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        if (_changeDetector == null)
        {
            Debug.Log("Change Detector is null");
        }
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NickName):
                    playerOverviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());
                    break;
                case nameof(currentDodge):
                    playerOverviewPanel.UpdateScore(Object.InputAuthority, currentDodge);
                    break;
                case nameof(currentHealth):
                    playerOverviewPanel.UpdateLives(Object.InputAuthority, currentHealth);
                    break;
            }
        }
    }

    // Remove the entry in the local Overview panel for this spaceship
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        playerOverviewPanel.RemoveEntry(Object.InputAuthority);
    }

    // Increase the score by X amount of points
    public void AddToDodge(float points)
    {
        currentDodge += points;
    }

    public void ResetDodge(float points)
    {
        currentDodge = points;
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth > 0 && !isDie) //If not dead and health is higher than 0
        {
            //Audio_Manager.instance.PlaySound(Audio_Manager.instance.hurt);
            //StartCoroutine(FlashDamageColor());

            currentHealth -= dmg; //Take damage
            /*HealthBar();
            if (gameObject.GetComponentInChildren<Player_Movements>() != null)
            {
                gameObject.GetComponentInChildren<Player_Movements>().ComboReset(); //Reset combo
                gameObject.GetComponentInChildren<Player_Movements>().comboText.text = (""); //Combo text
            }*/
           
        }
        if ((currentHealth == 0 || currentHealth <= 0) && !isDie && Object.HasStateAuthority) //If health is 0 and he is not dead, let it die.
        {
            isDie = true;
            anim.SetTrigger("isDef"); //Def animation
            currentHealth = 0;
            FindObjectOfType<PVP_GameStateController>().CheckIfGameHasEnded();
            // End Screen
            //GameManager.KOScreen(); //KO Screen start
        }
    }
    /*public void HealthBar() //health bar animation
    {
        healthImg.DOFillAmount(currentHealth / maxHealth, 0.4f);
        StartCoroutine(BarAnims(currentHealth, maxHealth, backgroundImg));
    }
    public void UltiBar() //ulti bar animation
    {
        UltimateImg.DOFillAmount(currentDodge / maxDodge, 0.4f);
        StartCoroutine(BarAnims(currentDodge, maxDodge, backgrounUltidImg));
    }
    public IEnumerator BarAnims(float current, float max, Image background) //Animation
    {
        yield return new WaitForSeconds(0.2f);
        background.DOFillAmount(current / max, 0.4f);
    }

    private IEnumerator FlashDamageColor()
    {
        // Damage color
        spriteRenderer.color = damageColor;

        // Get colors back after a certain period of time
        yield return new WaitForSeconds(damageFlashDuration);

        //Revert original sprite color
        spriteRenderer.color = Color.white; // Default color is white or the sprite's original color
    }*/
}
