using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class YeetRoundContentManager : MonoBehaviour
{
    public TextMeshProUGUI RoundsText;
    public TextMeshProUGUI RewardsText;

    public void SetText(int roundsText, int rewardsText)
    {
        RoundsText.text = roundsText.ToString();
        RewardsText.text = rewardsText.ToString();
    }
}
