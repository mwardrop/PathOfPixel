using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveToMouse : MonoBehaviour
{
    enum Direction
    {
        Down,
        Left,
        Right,
        Up
    }

    public float speed = (float)2.5;
    private Vector3 target;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;

        }  

    }

    void FixedUpdate()
    {
        if(transform.position != target)
        {
            if (target.y > transform.position.y)
            {
                SetDirection(Direction.Up);

                if (Math.Abs((target.y - transform.position.y)) < Math.Abs((target.x - transform.position.x)))
                {
                    if (target.x - transform.position.x > 0) { SetDirection(Direction.Right); }
                    else { SetDirection(Direction.Left); }
                }
            }
            else
            {
                SetDirection(Direction.Down);

                if (Math.Abs((transform.position.y - target.y)) < Math.Abs((transform.position.x - target.x)))
                {
                    if (target.x - transform.position.x > 0) { SetDirection(Direction.Right); }
                    else { SetDirection(Direction.Left); }
                } 
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            animator.SetBool("moving", true);

        } else
        {
            animator.SetBool("moving", false);
        }
        
    }

    private void SetDirection(Direction direction)
    {

        Debug.Log(direction.ToString());
        switch (direction)
        {
            case Direction.Down:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", -1);
                break;
            case Direction.Left:
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
                break;
            case Direction.Right:
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
                break;
            case Direction.Up:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 1);
                break;
        }
    }
}
