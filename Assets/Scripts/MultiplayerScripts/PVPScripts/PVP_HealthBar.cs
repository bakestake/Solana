using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PVP_HealthBar : MonoBehaviour
{
    public Image healthImage;
    public Image ultimateImage;
    public TextMeshProUGUI nickname;

    public void UpdateEntry(float healthScore, float ultimateScore, string name)
    {
        nickname.text = name;
        healthImage.DOFillAmount(healthScore / 100.0f, 0.4f);
        ultimateImage.DOFillAmount(ultimateScore / 10.0f, 0.4f);
    }
}
