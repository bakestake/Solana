using Gamegaard.Pooling;
using Gamegaard.Timer;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public abstract class PopupTextBase : MonoBehaviour, IPauseable, IPoolable
    {
        [SerializeField] private float lifetimeInSeconds = 3;
        [field: SerializeField] protected bool UseUnscaledTime { get; set; }
        [SerializeReference] protected List<PopupTextEffectBase> effects = new List<PopupTextEffectBase>();

        public PopupTextManager PopupTextManager { get; protected set; }
        public Transform TargetToFollow { get; protected set; }
        public Waiter Lifetime { get; private set; }
        public string Text { get; private set; }
        public float TimeScale { get; set; }
        public float ElapsedLifeTime { get; private set; }
        public float ValueAsNumber { get; private set; }
        public bool HasTarget => TargetToFollow != null;
        public bool IsPermanent => Lifetime == null;
        public bool IsPaused { get; private set; }
        public bool IsNumber { get; private set; }

        protected virtual void Awake()
        {
            Lifetime = new Waiter(lifetimeInSeconds);
        }

        private void OnDisable()
        {
            if (PopupTextManager == null) return;
            PopupTextManager.RemovePopup(this);
        }

        public void UpdateBehaviours()
        {
            if (!enabled || IsPaused) return;

            ElapsedLifeTime += GetElapsedTime();

            foreach (PopupTextEffectBase effect in effects)
            {
                effect.Evaluate(this);
            }

            if (!IsPermanent && Lifetime.IsCompleted)
            {
                OnLifeTimeCompleted();
            }
        }

        public void Initialize(string text, PopupTextManager popupTextManager)
        {
            PopupTextManager = popupTextManager;
            SetText(text);

            foreach (PopupTextEffectBase effect in effects)
            {
                effect.Initialize(this);
            }
            gameObject.SetActive(true);
        }

        public void SetText(float value, string format = "")
        {
            SetText(value.ToString(format));
        }

        public virtual void SetText(string text)
        {
            Text = text;
            if (float.TryParse(text, out float value))
            {
                IsNumber = true;
                ValueAsNumber = value;
            }
        }

        public void AddEffect(PopupTextEffectBase effect)
        {
            if (!effects.Contains(effect))
                effects.Add(effect);
        }

        public bool RemoveEffect(PopupTextEffectBase effect)
        {
            return effects.Remove(effect);
        }

        public void SetTimeScaleMode(bool useUnscaledTime)
        {
            UseUnscaledTime = useUnscaledTime;
        }

        public void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            Lifetime.Pause();
        }

        public void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Lifetime.Resume();
        }

        public void ResetElapsedTime()
        {
            Lifetime.Reset();
            ElapsedLifeTime = 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetElapsedTime()
        {
            return (UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * TimeScale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnLifeTimeCompleted()
        {
            PopupTextManager.RemovePopup(this);
        }

        public abstract void SetColor(Color color);

        public void SetRandomColor(float minHue = 0, float maxHue = 1f, float minSaturation = 0, float maxSaturation = 1, float minValue = 0, float maxValue = 1, float minAlpha = 0, float maxAlpha = 1)
        {
            Color color = Random.ColorHSV(minHue, maxHue, minSaturation, maxSaturation, minValue, maxValue, minAlpha, maxAlpha);
            SetColor(color);
        }

        public void SetRandomColor(Color from, Color to)
        {
            SetColor(Color.Lerp(from, to, Random.Range(0f, 1f)));
        }

        public void SetRandomColor(Gradient gradient)
        {
            SetColor(gradient.Evaluate(Random.value));
        }

        public void OnTakenFromPool()
        {
            ResetElapsedTime();
            Resume();
            foreach (PopupTextEffectBase effect in effects)
            {
                effect.Reset();
            }
        }

        public void OnReturnedToPool()
        {

        }
    }
}