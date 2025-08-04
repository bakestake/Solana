using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class FillBarUI : MonoBehaviour, IFillBar
    {
        [Header("Local References")]
        [SerializeField] protected Image attrFillBar;

        public virtual void SetValue(float actualPercentage) => attrFillBar.fillAmount = actualPercentage;
    }
}