using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpriteState
{
    Idle,
    Walk,
    Attack1,
    Attack2,
    Attack3,
    Hurt,
    Death
}

public enum SpriteDirection
{
    Down,
    Left,
    Right,
    Up
}

public abstract class CharacterSprite : BaseSprite
{
    public Boolean hurt = false;

    public SpriteDirection direction = SpriteDirection.Down;
    public SpriteState state = SpriteState.Idle;

    public CharacterState CharacterState
    {
        get
        {
            if (this.GetType() == typeof(PlayerSprite))
            {
                return ((PlayerSprite)this).PlayerState;
            }
            else
            {
                return ((EnemySprite)this).EnemyState;
            }
        }
    }

    public Vector3 Target
    {
        get
        { 
            if(CharacterState == null) { return transform.position; }
            return new Vector3(CharacterState.TargetLocation.x, CharacterState.TargetLocation.y);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CharacterState == null) { Destroy(this.gameObject); }

        base.Update();

        // Show hurt interruption if State is not locked
        if (hurt && !IsStateLocked)
        {
            SetState(SpriteState.Hurt);
            hurt = false;
            return;
        }
        // Die
        if (CharacterState.IsDead)
        {
            SetState(SpriteState.Death);
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.Destroy();
            return;

        }
    }

    public void SetState(SpriteState _state)
    {

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
                case SpriteState.Walk:
                case SpriteState.Idle:
                    animator.SetBool(state.ToString(), true);
                    break;
                // Uninterruptible Animations
                case SpriteState.Attack1:
                case SpriteState.Attack2:
                case SpriteState.Attack3:
                case SpriteState.Hurt:
                    LockState(
                        state.ToString(),
                        direction.ToString().ToLower() + this.state);
                    break;
                // Stopping Animations
                case SpriteState.Death:
                    OnDeath();
                    LockState(
                        state.ToString(),
                        direction.ToString().ToLower() + this.state, true);
                    break;
            }

        }
    }

    protected virtual void OnDeath()
    {
        Debug.Log("Character Death.");
    }

    public SpriteDirection SetDirection(Vector3 target)
    {
        SpriteDirection _direction;

        if (target.y > transform.position.y)
        {
            _direction = SpriteDirection.Up;

            if (Math.Abs((target.y - transform.position.y)) < Math.Abs((target.x - transform.position.x)))
            {
                if (target.x - transform.position.x > 0) { _direction = SpriteDirection.Right; }
                else { _direction = SpriteDirection.Left; }
            }
        }
        else
        {
            _direction = SpriteDirection.Down;

            if (Math.Abs((transform.position.y - target.y)) < Math.Abs((transform.position.x - target.x)))
            {
                if (target.x - transform.position.x > 0) { _direction = SpriteDirection.Right; }
                else { _direction = SpriteDirection.Left; }
            }
        }

        this.SetDirection(_direction);
        return _direction;
    }

    protected void SetDirection(SpriteDirection _direction)
    {

        this.direction = _direction;

        switch (this.direction)
        {
            case SpriteDirection.Down:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", -1);
                break;
            case SpriteDirection.Left:
                animator.SetFloat("MoveX", -1);
                animator.SetFloat("MoveY", 0);
                break;
            case SpriteDirection.Right:
                animator.SetFloat("MoveX", 1);
                animator.SetFloat("MoveY", 0);
                break;
            case SpriteDirection.Up:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 1);
                break;
        }
    }

    public void MoveToDestination(Vector3 destination, float speed = 0)
    {
        SetDirection(destination);
        if (speed == 0) { speed = CharacterState.MoveSpeed; }
        rigidBody2D.MovePosition(Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime));
    }

    //public void Damage(float damageAmount, float knockback = 0)
    //{
        //if (canTakeDamage)
        //{
        //    incomingDamage += damageAmount;

        //    if (knockback > 0)
        //    {
        //        float x = transform.position.x;
        //        float y = transform.position.y;
        //        float z = transform.position.z;
        //        switch (direction)
        //        {
        //            case SpriteDirection.Up:
        //                y -= knockback;
        //                break;
        //            case SpriteDirection.Down:
        //                y += knockback;
        //                break;
        //            case SpriteDirection.Right:
        //                x -= knockback;
        //                break;
        //            case SpriteDirection.Left:
        //                x += knockback;
        //                break;
        //        }

        //        MoveToDestination(new Vector3(x, y, z), knockback);
        //    }
        //}
    //}

    public void Destroy(int seconds = 60)
    {

        StartCoroutine(DestroyCoroutine());

        IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(seconds);
            spriteRenderer.enabled = false;
            Destroy(this.gameObject);
        }
    }
}
