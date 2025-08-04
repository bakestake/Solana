using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public enum TextCurveMode { Constant, RandomBetweenConstants, Curve, RandomBetweenCurves }

    [System.Serializable]
    public class TextMinMaxCurve
    {
        [SerializeField] private bool isEnabled;
        [SerializeField] private TextCurveMode mode = TextCurveMode.Constant;
        [SerializeField] private float constantValue = 1f;
        [SerializeField] private Vector2 randomConstants = new Vector2(0f, 1f);
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private AnimationCurve minCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve maxCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private float randomConstantValue; // Valor randomizado entre constantes
        private float randomCurveFactor;  // Fator de interpolação entre curvas

        public bool IsEnabled => isEnabled;
        public TextCurveMode Mode => mode;
        public float ConstantValue => constantValue;
        public Vector2 RandomConstants => randomConstants;
        public AnimationCurve Curve => curve;
        public AnimationCurve MinCurve => minCurve;
        public AnimationCurve MaxCurve => maxCurve;

        /// <summary>
        /// Inicializa os valores randomizados para evitar flutuações.
        /// </summary>
        public void Initialize()
        {
            randomConstantValue = Random.Range(randomConstants.x, randomConstants.y);
            randomCurveFactor = Random.value;
        }

        /// <summary>
        /// Avalia o valor com base no modo selecionado e no progresso.
        /// </summary>
        /// <param name="time">Progresso no tempo (0 a 1).</param>
        /// <returns>Valor avaliado com base no modo.</returns>
        public float Evaluate(float time)
        {
            if (!isEnabled) return 0;

            switch (mode)
            {
                case TextCurveMode.Constant:
                    return constantValue;

                case TextCurveMode.RandomBetweenConstants:
                    return randomConstantValue;

                case TextCurveMode.Curve:
                    return curve.Evaluate(time);

                case TextCurveMode.RandomBetweenCurves:
                    float minValue = minCurve.Evaluate(time);
                    float maxValue = maxCurve.Evaluate(time);
                    return Mathf.Lerp(minValue, maxValue, randomCurveFactor);

                default:
                    return 0f;
            }
        }
    }

    [System.Serializable]
    public class ThreeAxisCurve
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private TextMinMaxCurve x = new TextMinMaxCurve();
        [SerializeField] private TextMinMaxCurve y = new TextMinMaxCurve();
        [SerializeField] private TextMinMaxCurve z = new TextMinMaxCurve();

        [SerializeField] private float multiplier = 1.0f;

        public bool IsEnabled => enabled;

        public void Initialize()
        {
            x.Initialize();
            y.Initialize();
            z.Initialize();
        }

        public Vector3 EvaluateAsVector(float progress)
        {
            return new Vector3(x.Evaluate(progress), y.Evaluate(progress), z.Evaluate(progress)) * multiplier;
        }
    }

    [System.Serializable]
    public class PopupTextEffectBase : IPopupTextEffect
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string EditorName;
#endif

        protected PopupTextManager manager;

        public virtual void Initialize(PopupTextBase popupText)
        {
            manager = popupText.PopupTextManager;
        }

        public virtual void Evaluate(PopupTextBase popupText) { }

        public virtual void Reset() { }
    }

    [System.Serializable]
    public class PopupTextEffect : PopupTextEffectBase
    {
        [SerializeField] private Gradient colorOverLifeTime;
        [SerializeField] private ThreeAxisCurve sizeOverLifeTime;
        [SerializeField] private ThreeAxisCurve rotationOverLifeTime;
        [SerializeField] private ThreeAxisCurve positionOverLifeTime;
        [SerializeField] private float gravityScale;

        private float gravity;

        public override void Initialize(PopupTextBase popupText)
        {
            base.Initialize(popupText);
            InitializeCurves();
        }

        public override void Evaluate(PopupTextBase popupText)
        {
            Transform transform = popupText.transform;
            float progress = popupText.Lifetime.Progress;

            if (sizeOverLifeTime.IsEnabled)
            {
                Vector3 size = sizeOverLifeTime.EvaluateAsVector(progress);
                transform.localScale = size;
            }

            if (rotationOverLifeTime.IsEnabled)
            {
                Vector3 rotation = rotationOverLifeTime.EvaluateAsVector(progress);
                transform.eulerAngles = rotation;
            }

            if (positionOverLifeTime.IsEnabled)
            {
                Vector3 positionOffset = positionOverLifeTime.EvaluateAsVector(progress);
                transform.position += positionOffset * Time.deltaTime;
            }

            Color color = colorOverLifeTime.Evaluate(progress);
            popupText.SetColor(color);

            gravity -= gravityScale * Time.deltaTime;
            transform.position += new Vector3(0, gravity, 0);
        }

        public override void Reset()
        {
            gravity = 0;
            InitializeCurves();
            base.Reset();
        }

        private void InitializeCurves()
        {
            sizeOverLifeTime.Initialize();
            rotationOverLifeTime.Initialize();
            positionOverLifeTime.Initialize();
        }
    }

    [System.Serializable]
    public class PushEffect : PopupTextEffectBase
    {
        [SerializeField] private float pushRadius = 1.0f;
        [SerializeField] private float pushStrength = 0.1f;

        public override void Evaluate(PopupTextBase popupText)
        {
            foreach (PopupTextBase otherText in manager.ActivePopups)
            {
                if (otherText == popupText) continue;

                Vector3 direction = (popupText.transform.position - otherText.transform.position).normalized;
                if (direction == Vector3.zero)
                {
                    direction = Vector3.up;
                }
                float distance = Vector3.Distance(popupText.transform.position, otherText.transform.position);

                if (distance < pushRadius)
                {
                    popupText.transform.position += pushStrength * Time.deltaTime * direction;
                }
            }
        }
    }

    [System.Serializable]
    public class CombineEffect : PopupTextEffectBase
    {
        [SerializeField] private float combinationDistance = 1.0f;

        public override void Evaluate(PopupTextBase popupText)
        {
            if (popupText == null) return;

            foreach (PopupTextBase other in manager.ActivePopups)
            {
                if (other != null && other == popupText || !other.IsNumber || !popupText.IsNumber) continue;
                float distance = Vector3.Distance(popupText.transform.position, other.transform.position);

                if (distance < combinationDistance)
                {
                    popupText.SetText(popupText.ValueAsNumber + other.ValueAsNumber);
                    popupText.ResetElapsedTime();
                    manager.RemovePopup(other);
                }
            }
        }
    }
}
