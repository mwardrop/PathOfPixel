using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class PlayerSprite : SpriteState
{
    public float moveSpeed = (float)2.5;
    public int health = 3000;

    private int incomingDamage = 0;

    private Vector3 target;
    private Boolean hurt;

    private State selectedAttack = State.Attack1;
    public float attackRadius = 1.2f;

    private Boolean _isAttacking = false;
    public Boolean isAttacking { get { return _isAttacking; } private set { _isAttacking = value; } }
    public EnemySprite _enemy;
    public EnemySprite enemy { 
        get { return _enemy; }
        set { _enemy = value; isAttacking = true; }
    }


    void Start()
    {
        target = transform.position;
        SetDirection(direction);
        SetState(state);
    }

    void Update()
    {
        if (hurt && !IsStateLocked)
        {
            SetState(State.Hurt);
            hurt = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
        }

        if (health <= 0)
        {
            SetState(State.Death);
            return;

        }

        if (incomingDamage > 0)
        {
            health -= incomingDamage;
            incomingDamage = 0;
            hurt = true;
            return;

        }

        if (Input.GetButtonDown("Attack1"))
        {
            SetAttack(State.Attack1);
        }

        if (Input.GetButtonDown("Attack2"))
        {
            SetAttack(State.Attack2);
        }

        if (Input.GetButtonDown("Attack3"))
        {
            SetAttack(State.Attack3);
        }

        // Attack Enemy if in range
        if (isAttacking && Vector3.Distance(transform.position, enemy.transform.position) < attackRadius)
        {
            SetState(selectedAttack);
            isAttacking = false;
            return;
        }

        // Move to mouse click
        if (Vector3.Distance(transform.position, target) > 0.5)
        {     
            SetDirection(target, transform.position);
            SetState(State.Walk);
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            return;
        }

        SetState(State.Idle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("canTakeDamage"))
        {
            other.GetComponent<EnemySprite>().Damage();
        }

    }

    private void SetAttack(State attack)
    {
        selectedAttack = attack;
    }

    public void Damage()
    {
        incomingDamage++;

    }

}
