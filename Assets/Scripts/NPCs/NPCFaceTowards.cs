using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public enum NPCDirection { LEFT, RIGHT, UP, DOWN }

    public class NPCFaceTowards : MonoBehaviour
    {
        [SerializeField] private NPCDirection direction;

        private void OnValidate()
        {
            ChangeDirection(direction);
        }

        private void Start()
        {
            ChangeDirection(direction);
        }

        public void ChangeDirection(NPCDirection direction)
        {
            this.direction = direction;

            float horizontal = 0f;
            if (direction == NPCDirection.RIGHT) horizontal = 1f;
            else if (direction == NPCDirection.LEFT) horizontal = -1f;

            float vertical = 0f;
            if (direction == NPCDirection.UP) vertical = 1f;
            else if (direction == NPCDirection.DOWN) vertical = -1f;

            var animator = GetComponent<Animator>();
            var cullingMode = animator.cullingMode;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.SetFloat("SpeedX", horizontal);
            animator.SetFloat("SpeedY", vertical);
            animator.Update(1f);
            animator.cullingMode = cullingMode;
        }
    }
}