using UnityEngine;

public class PlayerSkinCloner : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (!LocalGameManager.HasInstance || LocalGameManager.Instance.PlayerController == null) return;
        animator.runtimeAnimatorController = LocalGameManager.Instance.PlayerController.Animator.runtimeAnimatorController;
    }
}
