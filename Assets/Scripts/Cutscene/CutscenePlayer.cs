using UnityEngine;

namespace Bakeland
{
    public enum CutsceneMovementState
    {
        Idle,
        Walking,
        Riding
    }

    public class CutscenePlayer : MonoBehaviour
    {
        [SerializeField] private Animator skinAnimator;
        [SerializeField] private bool useAutoAnimation;

        private readonly static int speedXHash = Animator.StringToHash("SpeedX");
        private readonly static int speedYHash = Animator.StringToHash("SpeedY");
        private readonly static int isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly static int isRidingHash = Animator.StringToHash("IsRiding");

        public bool UseAutoAnimation => useAutoAnimation;

        private Vector3 lastPosition;
        private Vector2 lastDirection = Vector2.down;

        private void Start()
        {
            lastPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (!useAutoAnimation) return;

            Vector3 currentPosition = transform.position;
            Vector3 delta = currentPosition - lastPosition;

            if (delta.sqrMagnitude > 0.0001f)
            {
                Vector2 direction = delta.normalized;
                lastDirection = direction;

                SetAnimHorizontalValue(direction.x);
                SetAnimVerticalValue(direction.y);
                SetAnimationState(CutsceneMovementState.Walking);
            }
            else
            {
                SetAnimHorizontalValue(lastDirection.x);
                SetAnimVerticalValue(lastDirection.y);
                SetAnimationState(CutsceneMovementState.Idle);
            }

            lastPosition = currentPosition;
        }

        public void SetAutomaticMode(int useAuto)
        {
            useAutoAnimation = useAuto == 1;
            if (useAutoAnimation)
            {
                lastPosition = transform.position;
            }
        }

        public void SetAnimHorizontalValue(float direction)
        {
            lastDirection = new Vector2(direction, lastDirection.y);
            skinAnimator.SetFloat(speedXHash, direction);
        }

        public void SetAnimVerticalValue(float direction)
        {
            lastDirection = new Vector2(lastDirection.x, direction);
            skinAnimator.SetFloat(speedYHash, direction);
        }

        public void SetAnimationState(CutsceneMovementState cutsceneMovementState)
        {
            skinAnimator.SetBool(isWalkingHash, cutsceneMovementState == CutsceneMovementState.Walking);
            skinAnimator.SetBool(isRidingHash, cutsceneMovementState == CutsceneMovementState.Riding);
        }
    }
}
