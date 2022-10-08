using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum State
{
    Idle,
    Walk,
    Attack1,
    Attack2,
    Attack3,
    Hurt,
    Death
}

public enum Direction
{
    Down,
    Left,
    Right,
    Up
}

public class SpriteState : MonoBehaviour
{
    protected Boolean stateLocked = false;
    protected Animator animator;

    public Direction direction = Direction.Down;
    public State state = State.Idle;
    public Boolean IsStateLocked { get { return stateLocked; } }


    public void LockState(string resetParameter, String animation, Boolean permanent = false)
    {

        StartCoroutine(LockStateCoroutine(resetParameter));

        IEnumerator LockStateCoroutine(string resetParameter)
        {
            stateLocked = true;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            yield return null;
            if (!permanent) { animator.SetBool(resetParameter, !animator.GetBool(resetParameter)); }

            //TODO : Will need to account for Animation Speed and Animator Transition properties 
            float seconds = animator.runtimeAnimatorController.animationClips
                .Where<AnimationClip>((x) => x.name == animation)
                .First()
                .length;

            yield return new WaitForSeconds(seconds);

            if (!permanent) { stateLocked = false; }
            

        }
    }

    protected void SetState(State _state)
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }

        if (!IsStateLocked && this.state != _state)
        {
            state = _state;

            // Set Animator to default state
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(parameter.name, false);
                }
            }

            // Set Animator to Current State
            switch (state)
            {
                // Interruptible Animations
                case State.Walk:
                case State.Idle:
                    animator.SetBool(state.ToString(), true);
                    break;
                // Uninterruptible Animations
                case State.Attack1:
                case State.Attack2:
                case State.Attack3:
                case State.Hurt:
                    LockState(
                        state.ToString(),
                        direction.ToString().ToLower() + this.state);
                    break;
                // Stopping Animations
                case State.Death:
                    LockState(
                        state.ToString(),
                        direction.ToString().ToLower() + this.state, true);
                    break;
            }

        }
    }

    protected Direction SetDirection(Vector3 location, Vector3 target)
    {
        Direction _direction;

        if (location.y > target.y)
        {
            _direction = Direction.Up;

            if (Math.Abs((location.y - target.y)) < Math.Abs((location.x - target.x)))
            {
                if (location.x - target.x > 0) { _direction = Direction.Right; }
                else { _direction = Direction.Left; }
            }
        }
        else
        {
            _direction = Direction.Down;

            if (Math.Abs((target.y - location.y)) < Math.Abs((target.x - location.x)))
            {
                if (location.x - target.x > 0) { _direction = Direction.Right; }
                else { _direction = Direction.Left;  }
            }
        }

        this.SetDirection(_direction);
        return _direction;
    }

    protected void SetDirection(Direction _direction)
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }

        this.direction = _direction;

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
