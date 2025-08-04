using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class TwoStateObject : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; private set; }

        public virtual void Active()
        {
            IsActive = true;
        }

        public virtual void Desactive()
        {
            IsActive = false;
        }

        public void ChangeState()
        {
            if (IsActive)
            {
                Desactive();
            }
            else
            {
                Active();
            }
        }
    }
}