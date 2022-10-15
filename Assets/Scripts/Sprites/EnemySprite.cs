using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;



public class EnemySprite : BaseSprite
{
    private GameObject target;
    private Vector3 homePosition;

    public string enemyName = "Enemy";
    public int baseDamage = 1;
    public float chaseRadius = 6;
    public float attackRadius = 1.2f;
    public float idleRadius = 2;
    public bool canAttack = true;
    public bool canMove = true;

    void Start()
    {
        target = GameObject.FindWithTag("Player");
        homePosition = new Vector3(transform.position.x, transform.position.y);
        

        SetDirection(direction);
        SetState(state);
    }

    new void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            if (state != State.Death && this.GetComponent<CapsuleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                target.GetComponent<PlayerSprite>().enemy = this;
                GameObject.FindWithTag("MovementIndicator").GetComponent<MovementIndicator>().enemyClicked = true;
            }
        } 

        // Chase Player
        if (canMove &&
            state != State.Death &&
            Vector3.Distance(target.transform.position, transform.position) <= chaseRadius && 
            Vector3.Distance(target.transform.position, transform.position) > attackRadius) 
        {         
            SetDirection(target.transform.position);
            SetState(State.Walk);
            MoveToDestination(target.transform.position);
            return;

        }
        // Attack Player
        if (canAttack &&
            state != State.Death && 
            state != State.Attack1 && 
            Vector3.Distance(target.transform.position, transform.position) < attackRadius)
        {
            SetState(State.Attack1);
            return;

        }
        // Stop Chasing Player and return home
        if (canMove &&
            state != State.Death &&
            Vector3.Distance(target.transform.position, transform.position) > chaseRadius + 2 && 
            Vector3.Distance(homePosition, transform.position) > 0)
        {
            SetDirection(homePosition);
            SetState(State.Walk);
            MoveToDestination(homePosition);
            return;
        }

        SetState(State.Idle);
    }


}
