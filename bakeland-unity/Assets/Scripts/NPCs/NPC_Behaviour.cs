using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    public float NPC_Speed;
    public Collider2D NPC_collider2D;

    private Vector3 currentDirection;
    private bool isWalking = false;
    private Animator NPC_animator;
    private Rigidbody2D NPC_rb;

    // Start is called before the first frame update
    void Start()
    {
        //Animator
        NPC_animator = GetComponent<Animator>();
        NPC_rb = GetComponent<Rigidbody2D>();
        isWalking = true;
        ChangeDirection();
    }
    private void Update()
    {
        if (!isWalking)
            return;
        Movement();
    }
    private void Movement()
    {
        Vector3 point = transform.position + currentDirection * NPC_Speed * Time.deltaTime;
        if (NPC_collider2D.bounds.Contains(point))
        {
            NPC_rb.MovePosition(point);
        }
        else
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        int randomInt = Random.Range(1, 40) % 4;
        randomInt = randomInt == 0 ? 4 : randomInt;
        switch (randomInt)
        {
            case 1:
                currentDirection = Vector2.down;
                break;
            case 2:
                currentDirection = Vector2.up;
                break;
            case 3:
                currentDirection = Vector2.left;
                break;
            case 4:
                currentDirection = Vector2.right;
                break;
        }
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        NPC_animator.SetBool("IsWalking", isWalking);
        NPC_animator.SetFloat("SpeedX", currentDirection.x);
        NPC_animator.SetFloat("SpeedY", currentDirection.y);
    }

    public void Talking()
    {
        isWalking = false;
        UpdateAnimation();
    }

    public void EndTalking()
    {
        isWalking = true;
        UpdateAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDirection();
    }
}


