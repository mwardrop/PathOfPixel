using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;



public class EnemySprite : SpriteState
{
    public float moveSpeed = (float)1;
    public int health = 3;
    public string enemyName;
    public int baseDamage;
    public float chaseRadius;
    public float attackRadius = 1.2f;
    public Vector3 homePosition;

    private GameObject target;
    private int incomingDamage = 0;
    private Boolean hurt = false;

    void Start()
    {
        target = GameObject.FindWithTag("Player");
        homePosition = new Vector3(transform.position.x, transform.position.y);

    }

    void Update()
    {
        if(hurt && !IsStateLocked)
        {
            SetState(State.Hurt);
            hurt = false;
            return;
        }

        if (health <= 0)
        {
            SetState(State.Death);
            this.GetComponent<BoxCollider2D>().enabled = false;
            return;

        }

        if (incomingDamage > 0)
        {
            health -= incomingDamage;
            incomingDamage = 0;
            hurt = true;
            return;

        }
        // Chase Player
        if (Vector3.Distance(target.transform.position, transform.position) <= chaseRadius && Vector3.Distance(target.transform.position, transform.position) > attackRadius) 
        {         
            SetDirection(target.transform.position, transform.position);
            SetState(State.Walk);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            return;

        }
        // Attack Player
        if (Vector3.Distance(target.transform.position, transform.position) < attackRadius && state != State.Attack1)
        {
            SetState(State.Attack1);
            return;

        }


        // Stop Chasing Player and return home
        if (Vector3.Distance(target.transform.position, transform.position) > chaseRadius + 2 && Vector3.Distance(homePosition, transform.position) > 0)
        {
            SetDirection(homePosition, transform.position);
            SetState(State.Walk);
            transform.position = Vector3.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
            return;
        }

        SetState(State.Idle);
    }

    public void Damage()
    {
        incomingDamage++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerSprite>().Damage();

        }
    }
    private void OnMouseDown()
    {
        target.GetComponent<PlayerSprite>().enemy = this;
    }
}
