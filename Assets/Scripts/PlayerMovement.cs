using System;
using UnityEngine;

public enum State
{
    Idle,
    Walk,
    Attack1,
    Attack2,
    Attack3
}

public enum Direction
{
    Down,
    Left,
    Right,
    Up
}

public class PlayerMovement : CustomMonoBehaviour
{

    public float moveSpeed = (float)2.5;
    public Direction direction = Direction.Down;

    public State state = State.Idle;

    private Vector3 target;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.position;
        animator = GetComponent<Animator>();
        SetDirection(direction);
        SetState(state);
    }

    // Update is called once per frame
    void Update()
    {

        SetState(State.Idle);

        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
        }  

        if (transform.position != target)
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

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            SetState(State.Walk);

        }

        if (Input.GetButtonDown("Attack1"))
        {
            SetState(State.Attack1);
        }

        if (Input.GetButtonDown("Attack2"))
        {
            SetState(State.Attack2);
        }

        if (Input.GetButtonDown("Attack3"))
        {
            SetState(State.Attack3);
        }


    }

    private void SetState(State state)
    {

        if (!IsAnimationStateLocked && this.state != state) {

            // Capture previous state and current for locking purposes
            this.state = state;

            // Set Animator to default state
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(parameter.name, false);
                }
            }

            // Set Animator to Current State
            switch (this.state)
            {
                case State.Walk:
                    animator.SetBool("Walk", true);
                    break;
                case State.Attack1:
                case State.Attack2:
                case State.Attack3:
                    LockAnimationState(
                        this.state.ToString(),
                        animator,
                        direction.ToString().ToLower() + this.state);
                    break;
            }
            
        }
    }

    private void SetDirection(Direction direction)
    {

        this.direction = direction;

        switch (this.direction)
        {
            case Direction.Down:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", -1);
                break;
            case Direction.Left:
                animator.SetFloat("MoveX", -1);
                animator.SetFloat("MoveY", 0);
                break;
            case Direction.Right:
                animator.SetFloat("MoveX", 1);
                animator.SetFloat("MoveY", 0);
                break;
            case Direction.Up:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 1);
                break;
        }
    }

}
