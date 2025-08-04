using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class ValueContainerUI : MonoBehaviour
    {
        [Header("Refrences")]
        [SerializeField] private Image heartFill;
        [SerializeField] private Image blackFill;

        public void SetValue(float value, float maxValue)
        {
            heartFill.fillAmount = value;
            blackFill.fillAmount = maxValue;
        }
    }
}
