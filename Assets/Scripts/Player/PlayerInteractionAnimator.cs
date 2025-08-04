using UnityEngine;
using System.Collections;

public class PlayerInteractionAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator popUpAnimator;
    [SerializeField] private Transform playerTransform;

    public void PlayPopup()
    {
        popUpAnimator.gameObject.SetActive(true);
        popUpAnimator.SetTrigger("OnPlay");
    }

    public void PlayInteractionAnimation(Vector3 targetPosition, InteractionType interactionType)
    {
        Vector2 direction = targetPosition - playerTransform.position;

        string directionSuffix = GetDirectionSuffix(direction);
        string animationName = $"{interactionType}{directionSuffix}";

        animator.Play(animationName);
        PlayerController.canMove = false;
        PlayerController.canInteract = false;

        StartCoroutine(WaitForAnimationEnd());
    }

    private string GetDirectionSuffix(Vector2 direction)
    {
        direction.Normalize();

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x < 0f ? "Left" : "Right";
        }
        else
        {
            return direction.y < 0f ? "Down" : "Top";
        }
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length / stateInfo.speed;

        yield return new WaitForSeconds(animationDuration);

        PlayerController.canMove = true;
        PlayerController.canInteract = true;
    }
}
