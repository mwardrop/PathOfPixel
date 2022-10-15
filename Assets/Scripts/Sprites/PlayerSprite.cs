using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class PlayerSprite : BaseSprite
{
    private EnemySprite _enemy;
    private Boolean hasAttackClick;
    private Vector3 target;
    
    public State selectedAttack = State.Attack1;
    public float attackRadius = 1.2f;
    public float moveRadius = 1;
    public GameObject movementIndicator;
    private bool shouldMove = false;


    public EnemySprite enemy { 
        get { return _enemy; }
        set { _enemy = value; hasAttackClick = true; }
    }
    // TODO :   hasAttackClick should have a 1sec reset after set in order to
    //          ensure the player doesn't attack the enemy after deselecting the enemy

    void Start()
    {
        if(movementIndicator == null) { throw new Exception("Player Movement Indicator not attached to Player.");}
        target = transform.position;
        hasAttackClick = false;
        

        SetDirection(direction);
        SetState(state);
        SetAttack(State.Attack1);
    }

    new void Update()
    {      
        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            if(Vector3.Distance(transform.position, target) > moveRadius) { shouldMove = true; }
            SetDirection(target);
        }

        base.Update();

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
        if (hasAttackClick && Vector3.Distance(transform.position, enemy.transform.position) < attackRadius)
        {
            SetState(selectedAttack);
            hasAttackClick = false;
            return;
        }

        if(transform.position.x == target.x && transform.position.y == target.y)
        {
            shouldMove = false;
        }

        // Move to mouse click
        if (shouldMove)
        {      
            SetState(State.Walk);
            MoveToDestination(target);
            return;
        }

        SetState(State.Idle);
    }


    private void SetAttack(State attack)
    {
        selectedAttack = attack;
    }

}
