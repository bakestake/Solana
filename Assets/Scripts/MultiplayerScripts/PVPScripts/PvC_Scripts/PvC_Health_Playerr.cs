using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PvC_Health_Playerr : MonoBehaviour
{
    
    [Header("Health")]
    private float maxHealth = 100.0f;

    [HideInInspector]
    public float currentHealth { get; private set; }

    [HideInInspector]
    public float currentDodge { get; private set; }
    public float maxDodge = 10.0f;

    private bool isDie { get; set; }

    private PVC_PlayerOverPanel playerOverviewPanel = null;

    public Animator anim;
    private string nickName;

    public void DelayStart()
    {

        //nickName = FindObjectOfType<Mult_playerData>().GetNickName();
        //SetNickName(nickName);

        // --- Host
        // Initialized game specific settings 
        isDie = false;
        currentHealth = maxHealth;
        currentDodge = maxDodge;

        // --- Host & Client
        // Set the local runtime references.
        playerOverviewPanel = FindObjectOfType<PVC_PlayerOverPanel>();
        // Add an entry to the local Overview panel with the information of this spaceship
        playerOverviewPanel.AddEntry(nickName, this);

        //// Refresh panel visuals in Spawned to set to initial values.
        playerOverviewPanel.UpdateNickName(nickName, nickName);
        playerOverviewPanel.UpdateLives(nickName, currentHealth);
        playerOverviewPanel.UpdateScore(nickName, currentDodge);

        //_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    //// Remove the entry in the local Overview panel for this spaceship
    public void Despawned()
    {
        playerOverviewPanel.RemoveEntry(nickName);
    }

    // Increase the score by X amount of points
    public void AddToDodge(float points)
    {
        currentDodge += points;
        playerOverviewPanel.UpdateScore(nickName, currentDodge);
    }

    public void ResetDodge(float points)
    {
        currentDodge = points;
        playerOverviewPanel.UpdateScore(nickName, currentDodge);
    }

    //// RPC used to send player information to the Host
    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void SetNickName(string InputnickName)
    {
        nickName = InputnickName;
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth > 0 && !isDie) //If not dead and health is higher than 0
        {
            //Audio_Manager.instance.PlaySound(Audio_Manager.instance.hurt);
            //StartCoroutine(FlashDamageColor());

            currentHealth -= dmg; //Take damage
            playerOverviewPanel.UpdateLives(nickName, currentHealth);
            /*HealthBar();
            if (gameObject.GetComponentInChildren<Player_Movements>() != null)
            {
                gameObject.GetComponentInChildren<Player_Movements>().ComboReset(); //Reset combo
                gameObject.GetComponentInChildren<Player_Movements>().comboText.text = (""); //Combo text
            }*/

        }
        if ((currentHealth == 0 || currentHealth <= 0) && !isDie) //If health is 0 and he is not dead, let it die.
        {
            isDie = true;
            anim.SetTrigger("isDef"); //Def animation
            currentHealth = 0;
            FindObjectOfType<PVP_GameStateController>().CheckIfGameHasEnded();
            // End Screen
            //GameManager.KOScreen(); //KO Screen start
        }
    }

    public string GetPlayerNickname()
    {
        return nickName;
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
