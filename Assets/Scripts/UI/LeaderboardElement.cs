using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Bakeland
{
    public class LeaderboardElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text username;
        [SerializeField] private TMP_Text score;

        public void Initialize(string username, int score)
        {
            this.username.text = username;
            this.score.text = score.ToString();
        }
    }
}
