using System;
using TMPro;
using UnityEngine;

namespace Gamegaard
{
    public class BossLifeBarUI : FillBarUI
    {
        [SerializeField] private TextMeshProUGUI bossName;
        [SerializeField] private GameObject bar;

        public static Action<string, float> OnUpdateBar;

        private void Start()
        {
            UpdateBarData("", 0);
        }

        private void OnEnable()
        {
            OnUpdateBar += UpdateBarData;
        }

        private void OnDisable()
        {
            OnUpdateBar -= UpdateBarData;
        }

        private void UpdateBarData(string name, float lifeValue)
        {
            bossName.SetText(name);
            SetValue(lifeValue);
            bar.SetActive(lifeValue > 0);
        }
    }
}