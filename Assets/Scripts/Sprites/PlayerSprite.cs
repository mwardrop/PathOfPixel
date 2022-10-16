using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class PlayerSprite : CharacterSprite
{
    private Vector3 target;
    private MovementIndicator movementIndicator;
    
    public SpriteState selectedAttack = SpriteState.Attack1;
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
        SetAttack(SpriteState.Attack1);
    }

    protected override void Update()
    {      
        if (Input.GetMouseButtonDown(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            if(canMove && Vector3.Distance(transform.position, target) > moveRadius) { shouldMove = true; }
            SetDirection(target);
        }

        base.Update();

        if (Input.GetButtonDown("Attack1"))
        {
            SetAttack(SpriteState.Attack1);
        }

        if (Input.GetButtonDown("Attack2"))
        {
            SetAttack(SpriteState.Attack2);
        }

        if (Input.GetButtonDown("Attack3"))
        {
            SetAttack(SpriteState.Attack3);
        }

        // Attack Enemy if in range
        if (canAttack && 
            movementIndicator.enemyClicked && 
            Vector3.Distance(transform.position, enemy.transform.position) < attackRadius)
        {
            SetState(selectedAttack);
            movementIndicator.enemyClicked = false;
            return;
        }

        if(Vector3.Distance(transform.position, target) < moveRadius)
        {
            shouldMove = false;
        }

        // Move to mouse click
        if (shouldMove)
        {      
            SetState(SpriteState.Walk);
            MoveToDestination(target);
            return;
        }

        SetState(SpriteState.Idle);
    }


    private void SetAttack(SpriteState attack)
    {
        selectedAttack = attack;
    }

}
