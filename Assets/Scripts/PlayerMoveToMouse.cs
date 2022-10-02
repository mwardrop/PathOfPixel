using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.CullingGroup;

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

public class PlayerMoveToMouse : MonoBehaviour
{

    public float playerSpeed = (float)2.5;
    public Direction playerDIrection = Direction.Down;
    public State playerState = State.Idle;

    private State previousState = State.Idle;
    private Vector3 target;
    private Animator animator;
    private bool stateLocked;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.position;
        animator = GetComponent<Animator>();
        SetDirection(playerDIrection);
        SetState(playerState);
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

            transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.fixedDeltaTime);
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


    void FixedUpdate()
    {

    }

    // Used to ensure the state doesn't change before an animation is completed.
    private void LockState(string resetParameter)
    {
        StartCoroutine(LockStateCoroutine(resetParameter));

        IEnumerator LockStateCoroutine( string resetParameter)
        {
            stateLocked = true;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            yield return null;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            float seconds = animator.runtimeAnimatorController.animationClips
                .Where<AnimationClip>((x) => x.name == "down" + char.ToUpper(resetParameter[0]) + resetParameter.Substring(1))
                .First()
                .length;

            yield return new WaitForSeconds(seconds);      
            stateLocked = false;
            
        }
    }

    private void SetState(State state)
    {

        if (!stateLocked && playerState != state) {

            // Capture previous state and current for locking purposes
            previousState = playerState;
            playerState = state;

            // Set Animator to default state
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(parameter.name, false);
                }
            }

            // Set Animator to Current State
            switch (playerState)
            {
                case State.Walk:
                    animator.SetBool("walk", true);
                    break;
                case State.Attack1:
                    LockState("attack1");
                    break;
                case State.Attack2:
                    LockState("attack2");
                    break;
                case State.Attack3:
                    LockState("attack3");
                    break;
            }

            
        }
    }

    private void SetDirection(Direction direction)
    {

        playerDIrection = direction;

        switch (playerDIrection)
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
