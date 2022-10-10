using System;
using System.Collections;
using System.Linq;
using UnityEngine;


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

public abstract class BaseSprite : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidBody2D;
    private SpriteRenderer _spriteRenderer;
    
    protected float incomingDamage = 0;
    protected Boolean stateLocked = false;
    protected Boolean hurt = false;

    public Animator animator
    {
        get
        {
            if (!_animator) { _animator = GetComponent<Animator>(); }
            return _animator;
        }
        set { _animator = value; }
    }
    public Rigidbody2D rigidBody2D
    {
        get
        {
            if (!_rigidBody2D) { _rigidBody2D = GetComponent<Rigidbody2D>(); }
            return _rigidBody2D;
        }
        set { _rigidBody2D = value; }
    }
    public SpriteRenderer spriteRenderer
    {
        get
        {
            if (!_spriteRenderer) { _spriteRenderer = GetComponent<SpriteRenderer>(); }
            return _spriteRenderer;
        }
        set { _spriteRenderer = value; }
    }

    public float moveSpeed = (float)2.5;
    public Direction direction = Direction.Down;
    public State state = State.Idle;
    public float health = 100f;
    public Boolean IsStateLocked { get { return stateLocked; } }

    protected void Update()
    {

        spriteRenderer.sortingOrder = (int)((-transform.position.y) * 100f);

        // Show hurt interruption if State is not locked
        if (hurt && !IsStateLocked)
        {
            SetState(State.Hurt);
            hurt = false;
            return;
        }
        // Die
        if (health <= 0)
        {
            SetState(State.Death);
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.Destroy();
            return;

        }
        // Take Damage
        if (incomingDamage > 0)
        {
            health -= incomingDamage;
            incomingDamage = 0;
            hurt = true;
            return;

        }
    }

    public void LockState(string resetParameter, String animation, Boolean permanent = false)
    {

        StartCoroutine(LockStateCoroutine(resetParameter));

        IEnumerator LockStateCoroutine(string resetParameter)
        {
            stateLocked = true;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            yield return null;
            if (!permanent) { animator.SetBool(resetParameter, !animator.GetBool(resetParameter)); }

            // TODO : Will need to account for Animation Speed and Animator Transition properties when implementing attack speed
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

    protected Direction SetDirection(Vector3 target)
    {
        Direction _direction;

        if (target.y > transform.position.y)
        {
            _direction = Direction.Up;

            if (Math.Abs((target.y - transform.position.y)) < Math.Abs((target.x - transform.position.x)))
            {
                if (target.x - transform.position.x > 0) { _direction = Direction.Right; }
                else { _direction = Direction.Left; }
            }
        }
        else
        {
            _direction = Direction.Down;

            if (Math.Abs((transform.position.y - target.y)) < Math.Abs((transform.position.x - target.x)))
            {
                if (target.x - transform.position.x > 0) { _direction = Direction.Right; }
                else { _direction = Direction.Left;  }
            }
        }

        this.SetDirection(_direction);
        return _direction;
    }

    protected void SetDirection(Direction _direction)
    {

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

    protected void MoveToDestination(Vector3 destination, float speed = 0)
    {
        if(speed == 0) { speed = moveSpeed;  }
        rigidBody2D.MovePosition(Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime));
    }

    public int debugHitCount = 0;
    public void Damage(float damageAmount, float knockback = 0)
    {
        debugHitCount++;
        incomingDamage += damageAmount;

        if (knockback > 0)
        {

            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;
            switch (direction)
            {
                case Direction.Up:
                    y -= knockback;
                    break;
                case Direction.Down:
                    y += knockback;
                    break;
                case Direction.Right:
                    x -= knockback;
                    break;
                case Direction.Left:
                    x += knockback;
                    break;
            }

            MoveToDestination(new Vector3(x, y, z), knockback);
        }
    }

    public void Destroy(int seconds = 60)
    {

        StartCoroutine(DestroyCoroutine());

        IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(seconds);
            spriteRenderer.enabled = false;
            Destroy(this);
        }
    }
}
