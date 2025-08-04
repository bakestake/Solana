using TMPro;
using UnityEngine;

namespace Gamegaard
{
    public abstract class ClockTimer : MonoBehaviour
    {
        [SerializeField] protected bool useUnscaledTime;
        [SerializeField] protected float timeScale = 1;

        [Header("ShowStyle")]
        [SerializeField] protected TimeUnit showTime = TimeUnit.Minutes;
        [SerializeField] protected bool showMilliseconds;
        [Range(1, 3)]
        [SerializeField] private int millisecondsDigits = 2;

        [Header("References")]
        [SerializeField] protected TextMeshProUGUI timerText;

        protected float _elapsedTime;

        public abstract float ElapsedTime { get; protected set; }

        protected virtual void OnValidate()
        {
            if (timerText == null)
            {
                timerText = GetComponent<TextMeshProUGUI>();
            }
            UpdateText();
        }

        protected virtual void Update()
        {
            UpdateTime();
            UpdateText();
        }

        protected abstract void UpdateTime();

        protected virtual void UpdateText()
        {
            string formattedTime = GetFormatedText();
            timerText.SetText(formattedTime);
        }

        public string GetFormatedText()
        {
            int seconds = GetSeconds();
            string formattedTime;

            switch (showTime)
            {
                case TimeUnit.Seconds:
                    formattedTime = _elapsedTime.ToString("00");
                    break;
                case TimeUnit.Minutes:
                    formattedTime = $"{GetMinutes():00}:{seconds:00}";
                    break;
                case TimeUnit.Hours:
                    formattedTime = $"{GetHours():00}:{GetMinutes():00}:{seconds:00}";
                    break;
                default:
                    formattedTime = "";
                    break;
            }

            if (showMilliseconds)
            {
                int milliseconds = GetMilliseconds();
                string miliText = milliseconds.ToString();
                string millisecondsText = miliText.PadLeft(Mathf.Max(millisecondsDigits, miliText.Length), '0');
                formattedTime += $":{millisecondsText}";
            }
            return formattedTime;
        }

        public void ResetTimer()
        {
            _elapsedTime = 0;
            UpdateText();
        }

        public virtual void SetTimeInSeconds(float timeInSeconds)
        {
            _elapsedTime = timeInSeconds;
        }

        public virtual void AddTime(float timeInSeconds)
        {
            _elapsedTime += timeInSeconds;
        }

        protected float GetElapsedTime()
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            return deltaTime * timeScale;
        }

        public void Pause()
        {
            enabled = false;
        }

        public void UnPause()
        {
            enabled = true;
        }

        public void SetTimescale(float timeScale)
        {
            this.timeScale = timeScale;
        }

        public int GetSeconds()
        {
            return (int)(_elapsedTime % 60);
        }

        public int GetMinutes()
        {
            return (int)(_elapsedTime / 60);
        }

        public int GetHours()
        {
            return (int)(_elapsedTime / 3600);
        }

        public int GetMilliseconds()
        {
            return (int)(_elapsedTime * 1000 % 1000);
        }
    }
}