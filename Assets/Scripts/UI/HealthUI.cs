using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Find the player's Health component and subscribe to its event
        Health playerHealth = LocalGameManager.Instance.PlayerController.GetComponent<Health>();
        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }

        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
        
        animator.Play("ANIM_HealthUI_TakeDamage", 0, 0f);
    }
}