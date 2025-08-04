using System.Collections;
using UnityEngine;

namespace Bakeland
{
    public class AndroidFpsFix : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5f);

#if PLATFORM_ANDROID || PLATFORM_IOS
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            QualitySettings.vSyncCount = 0;
            GameObject.Find("FpsText").GetComponent<TMPro.TMP_Text>().color = Color.cyan;
#endif
        }
    }
}
