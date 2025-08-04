using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class FPSText : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private float interpolation = 1;

        private TMP_Text text;
        private float deltaTime;
        private float timer;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * interpolation;
            timer += Time.unscaledDeltaTime;

            if (timer >= updateInterval)
            {
                float fps = 1.0f / deltaTime;
                text.text = $"{fps:0.}<size=-15>FPS</size>";
                timer = 0f;
            }
        }
    }
}