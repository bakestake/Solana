using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float flashTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private Coroutine damageFlashCoroutine;
    private const string FlashAmountParam = "_FlashAmount";
    private bool canFlash;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        canFlash = material.HasFloat(FlashAmountParam);
    }

    public void FlashSprite()
    {
        if (!canFlash) return;
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }
        damageFlashCoroutine = StartCoroutine(ChangeFlashAmount(material, 1f));
    }

    private IEnumerator ChangeFlashAmount(Material mat, float targetValue)
    {
        float currentValue = mat.GetFloat(FlashAmountParam);
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(currentValue, targetValue, elapsedTime / flashTime);
            mat.SetFloat(FlashAmountParam, newValue);
            yield return null;
        }

        mat.SetFloat(FlashAmountParam, targetValue);

        if (targetValue == 1f)
        {
            yield return new WaitForSeconds(0.1f);
            mat.SetFloat(FlashAmountParam, 0f);
        }
    }
}
