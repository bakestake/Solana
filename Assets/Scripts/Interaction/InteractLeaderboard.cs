using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class InteractLeaderboard : MonoBehaviour
    {
        public void ShowLeaderboard()
        {
            LeaderboardManager.Instance.Show();
        }
    }
}