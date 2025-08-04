using UnityEngine;

namespace Gamegaard
{
    public abstract class AnimatorParameterSetter<T> : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected string paramName;

        protected int hash;

        protected virtual void Reset()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Awake()
        {
            hash = Animator.StringToHash(paramName);
        }

        public abstract void SetValue(T value);
    }
}