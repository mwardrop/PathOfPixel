using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BaseSprite : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidBody2D;
    private SpriteRenderer _spriteRenderer;

    protected Boolean stateLocked = false;

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


    public Boolean IsStateLocked { get { return stateLocked; } }


    protected virtual void Update()
    {

        spriteRenderer.sortingOrder = (int)((-transform.position.y) * 100f);

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

}
