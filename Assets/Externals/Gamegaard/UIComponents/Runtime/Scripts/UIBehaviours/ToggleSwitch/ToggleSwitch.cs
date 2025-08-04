using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Setup")]
    [Range(0, 1)]
    [SerializeField] private float value;

    [Header("Animation")]
    [Min(0)]
    [SerializeField] private float durationInSeconds = 0.5f;
    [SerializeField] private AnimationCurve sliderEase = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Events")]
    [SerializeField] private UnityEvent onTogleOn;
    [SerializeField] private UnityEvent onTogleOff;

    private Slider slider;
    private Coroutine anitionSliderCoroutine;
    private ToggleSwitchGroup toggleSwitchGroup;

    public bool IsToggleEnabled { get; private set; }

    protected virtual void OnValidate()
    {
        SetupToggleComponents();
        slider.value = value;
    }

    private void Awake()
    {
        SetupSliderComponent();
    }

    private void SetupToggleComponents()
    {
        if (slider != null) return;
        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        slider = GetComponent<Slider>();
        slider.interactable = false;
        var sliderColors = slider.colors;
        sliderColors.disabledColor = Color.white;
        slider.colors = sliderColors;
        slider.transition = Selectable.Transition.None;
    }

    public void SetupForGroup(ToggleSwitchGroup group)
    {
        toggleSwitchGroup = group;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void Toggle()
    {
        if (toggleSwitchGroup != null)
        {
            toggleSwitchGroup.ToggleGroup(this);
        }
        else
        {
            SetStateAndStartAnimation(!IsToggleEnabled);
        }
    }

    public void ToggleByGroup(bool isEnabled)
    {
        SetStateAndStartAnimation(isEnabled);
    }

    protected virtual void SetStateAndStartAnimation(bool isEnabled)
    {
        if (IsToggleEnabled != isEnabled)
        {
            if (isEnabled)
            {
                onTogleOn?.Invoke();
            }
            else
            {
                onTogleOff?.Invoke();
            }
        }
        IsToggleEnabled = isEnabled;

        if (anitionSliderCoroutine != null)
        {
            StopCoroutine(anitionSliderCoroutine);
        }

        anitionSliderCoroutine = StartCoroutine(AnimationSlider());
    }

    private IEnumerator AnimationSlider()
    {
        float endValue = IsToggleEnabled ? 1 : 0;
        if (durationInSeconds > 0)
        {
            float startValue = slider.value;
            float time = 0;
            float percentage;
            float actualValue;
            while (time < durationInSeconds)
            {
                time += Time.unscaledDeltaTime;
                percentage = (float)time / durationInSeconds;
                actualValue = sliderEase.Evaluate(percentage);
                slider.value = Mathf.Lerp(startValue, endValue, actualValue);
                yield return null;
            }
        }
        slider.value = endValue;
        anitionSliderCoroutine = null;
    }
}