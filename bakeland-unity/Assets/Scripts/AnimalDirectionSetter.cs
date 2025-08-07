using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDirectionSetter : MonoBehaviour
{
    public Animator animator;
    public animDirection direction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetDirection(direction);
    }

    private void SetDirection(animDirection dir)
    {
        switch (dir)
        {
            case animDirection.Up:
                animator.SetTrigger("up");
                break;
            case animDirection.Down:
                animator.SetTrigger("down");
                break;
            case animDirection.Left:
                animator.SetTrigger("left");
                break;
            case animDirection.Right:
                animator.SetTrigger("right");
                break;
            default:
                break;
        }

    }
}

public enum animDirection
{
    Up,
    Down,
    Left,
    Right
}
