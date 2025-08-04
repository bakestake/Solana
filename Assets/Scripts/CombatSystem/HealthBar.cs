using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Animator animator;

    private float lastValue;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (currentValue <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (slider != null)
            {
                slider.value = currentValue / (float)maxValue;
            }

            if (animator != null && lastValue > currentValue)
            {
                animator.Play("TakeDamage");
            }
            lastValue = currentValue;
        }
    }
}