using DG.Tweening;
using System;
using UnityEngine;

namespace Gamegaard
{
    [DisallowMultipleComponent]
    public class ObjectScroll<T> : MonoBehaviour where T : ObjectScrollElement
    {
        [Header("Values")]
        [Min(0)]
        [SerializeField] private float animTime = 0.3f;
        [SerializeField] private SingleObjectScrollPhaseValue<T> fullPhase = new SingleObjectScrollPhaseValue<T>(1, Color.white);
        [SerializeField] private TwoObjectScrollPhaseValue<T> midPhase = new TwoObjectScrollPhaseValue<T>(0.75f, Color.gray);
        [SerializeField] private TwoObjectScrollPhaseValue<T> minPhase = new TwoObjectScrollPhaseValue<T>(0, Color.black);

        private Vector3 previousItemOriginalPos;
        private Vector3 actualItemOriginalPos;
        private Vector3 nextItemOriginalPos;

        private Sequence sequence;

        public T PreviousItem02 => minPhase.previousElement;
        public T PreviousItem => midPhase.previousElement;
        public T ActualItem => fullPhase.element;
        public T NextItem => midPhase.nextElement;
        public T NextItem02 => minPhase.nextElement;

        public event Action OnNextFinished;
        public event Action OnPreviousFinished;

        private void Awake()
        {
            previousItemOriginalPos = PreviousItem.transform.localPosition;
            actualItemOriginalPos = ActualItem.transform.localPosition;
            nextItemOriginalPos = NextItem.transform.localPosition;
            fullPhase.SetToInitialScale();
            midPhase.SetToInitialScale();
            minPhase.SetToInitialScale();
        }

        public virtual void TransitionToNextItem()
        {
            if (sequence != null && sequence.IsPlaying()) return;

            SetOrder(previousItemOriginalPos, NextItem, NextItem02, ActualItem, PreviousItem, PreviousItem02);
            sequence.Play().OnComplete(OnFinishNext);
        }

        public virtual void TransitionToPreviousItem()
        {
            if (sequence != null && sequence.IsPlaying()) return;

            SetOrder(nextItemOriginalPos, PreviousItem, PreviousItem02, ActualItem, NextItem, NextItem02);
            sequence.Play().OnComplete(OnFinishPrevious);
        }

        private void SetOrder(Vector3 nextAcualPosition, params ObjectScrollElement[] elements)
        {
            foreach (ObjectScrollElement element in elements)
            {
                element.transform.SetSiblingIndex(0);
            }
            if (sequence != null)
            {
                sequence.Kill();
            }
            sequence = DOTween.Sequence().Pause().SetAutoKill(false);

            MoveAndRescale(elements[1], Color.gray, midPhase.Size);
            MoveAndRescaleAndRecolor(elements[0], Color.white, fullPhase.Size, actualItemOriginalPos);
            MoveAndRescaleAndRecolor(elements[2], Color.gray, midPhase.Size, nextAcualPosition);
            MoveAndRescale(elements[3], Color.black, minPhase.Size);
        }

        private void MoveAndRescaleAndRecolor(ObjectScrollElement element, Color color, float size, Vector3 position)
        {
            MoveAndRescale(element, color, size);
            sequence.Join(element.transform.DOLocalMove(position, animTime));
        }

        private void MoveAndRescale(ObjectScrollElement element, Color color, float size)
        {
            sequence.Join(element.transform.DOScale(size, animTime));
            sequence.Join(element.DoColor(color, animTime));
        }

        private void OnFinishNext()
        {
            PreviousItem02.transform.localPosition = nextItemOriginalPos;
            SwapItems(ref minPhase.previousElement, ref minPhase.nextElement);
            SwapItems(ref minPhase.previousElement, ref midPhase.previousElement);
            SwapItems(ref midPhase.previousElement, ref fullPhase.element);
            SwapItems(ref fullPhase.element, ref midPhase.nextElement);
            OnNextFinished?.Invoke();
        }

        private void OnFinishPrevious()
        {
            NextItem02.transform.localPosition = previousItemOriginalPos;
            SwapItems(ref minPhase.previousElement, ref minPhase.nextElement);
            SwapItems(ref minPhase.nextElement, ref midPhase.nextElement);
            SwapItems(ref midPhase.nextElement, ref fullPhase.element);
            SwapItems(ref fullPhase.element, ref midPhase.previousElement);
            OnPreviousFinished?.Invoke();
        }

        private void SwapItems(ref T item1, ref T item2)
        {
            (item2, item1) = (item1, item2);
        }
    }
}