using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int countFps = 30;
    public float duration = 1f;
    public string numberFormat = "N0";
    private int _value;
    // public AudioSource audioSource;

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }

    private Coroutine countingCoroutine;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void UpdateText(int newValue)
    {
        if (countingCoroutine != null)
        {
            StopCoroutine(countingCoroutine);
        }

        countingCoroutine = StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newValue)
    {
        // audioSource.Play();
        WaitForSeconds wait = new WaitForSeconds(1f / countFps);
        int previousValue = _value;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (countFps * duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (countFps * duration));
        }

        if (previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }
                text.SetText(previousValue.ToString(numberFormat));
                yield return wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount;
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }
                text.SetText(previousValue.ToString(numberFormat));
                yield return wait;
            }
        }
        // audioSource.Stop();
        // SoundManager.instance.PlaySfx(SoundManager.instance.bankIn);
    }
}
