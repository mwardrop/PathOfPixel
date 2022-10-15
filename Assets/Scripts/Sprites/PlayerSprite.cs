using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class PlayerSprite : BaseSprite
{
    private Vector3 target;
    private MovementIndicator movementIndicator;
    
    public State selectedAttack = State.Attack1;
    public float attackRadius = 1.2f;
    public float moveRadius = 1;
    public GameObject movementIndicatorObject;
    public EnemySprite enemy;
    public ScriptableObject gameSate;

    private bool shouldMove = false;

    void Start()
    {
        target = transform.position;
        movementIndicator = movementIndicatorObject.GetComponent<MovementIndicator>();

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
        if (movementIndicator.enemyClicked && Vector3.Distance(transform.position, enemy.transform.position) < attackRadius)
        {
            SetState(selectedAttack);
            movementIndicator.enemyClicked = false;
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
