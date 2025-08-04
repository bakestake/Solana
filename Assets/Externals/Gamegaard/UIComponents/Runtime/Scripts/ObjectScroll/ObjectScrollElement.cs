using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public abstract class ObjectScrollElement<T> : ObjectScrollElement
    {
        public abstract void Initialize(T value);
    }

    public abstract class ObjectScrollElement : MonoBehaviour
    {
        private Graphic[] images;

        private void Awake()
        {
            images = GetComponentsInChildren<Graphic>();
        }

        public Sequence DoColor(Color color, float animTime)
        {
            Debug.LogWarning("Docolor dont work. Please create a new assembly for dotween.");
            Sequence sequence = DOTween.Sequence();
            //foreach (Graphic image in images)
            //{
            //    sequence.Join(image.DOColor(color, animTime));
            //}
            return sequence;
        }
    }
}