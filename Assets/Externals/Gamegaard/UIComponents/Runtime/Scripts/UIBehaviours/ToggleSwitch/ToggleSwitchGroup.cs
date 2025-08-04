using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ToggleSwitchGroup : MonoBehaviour
{
    [Header("Start Value")]
    [SerializeField] private ToggleSwitch initialToggle;

    [Header("ToggleOptions")]
    [Min(1)]
    [SerializeField] private int maxSimultaneosOn = 1;
    [SerializeField] private bool allCanBeToggledOff;

    private readonly List<ToggleSwitch> toggleSwitches = new List<ToggleSwitch>();
    private readonly List<ToggleSwitch> toggleOnSwitches = new List<ToggleSwitch>();

    private void Awake()
    {
        ToggleSwitch[] childToggles = GetComponentsInChildren<ToggleSwitch>();
        foreach (var toggleSwitch in childToggles)
        {
            RegisterToggleSwitch(toggleSwitch);
        }
    }

    private void Start()
    {
        if (toggleSwitches.Any(x => x.IsToggleEnabled) || allCanBeToggledOff) return;

        ToggleSwitch switchToEnable = initialToggle != null ? initialToggle : toggleSwitches[0];
        ToggleGroup(switchToEnable);
    }

    private void RegisterToggleSwitch(ToggleSwitch toggleSwitch)
    {
        if (toggleSwitches.Contains(toggleSwitch)) return;

        toggleSwitches.Add(toggleSwitch);
        toggleSwitch.SetupForGroup(this);
    }

    public void ToggleGroup(ToggleSwitch toggleSwitch)
    {
        if (toggleSwitch.IsToggleEnabled)
        {
            if (allCanBeToggledOff || toggleOnSwitches.Count > 1)
            {
                DisableToggle(toggleSwitch);
            }
        }
        else
        {
            EnableToggle(toggleSwitch);
            if (toggleOnSwitches.Count > maxSimultaneosOn)
            {
                DisableToggle(toggleOnSwitches[0]);
            }
        }
    }

    private void EnableToggle(ToggleSwitch toggleSwitch)
    {
        toggleOnSwitches.Add(toggleSwitch);
        toggleSwitch.ToggleByGroup(true);
    }

    private void DisableToggle(ToggleSwitch toggleSwitch)
    {
        toggleSwitch.ToggleByGroup(false);
        toggleOnSwitches.Remove(toggleSwitch);
    }
}
