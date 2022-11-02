using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySprite : CharacterSprite
{
    public Guid StateGuid;

    public GameObject TargetPlayer { get
        {          
            if(TargetPlayerId == -1) { return null;  }
            return ClientManager.Instance.StateManager.GetPlayerGameObject(TargetPlayerId);
        }
    }

    public int TargetPlayerId = -1;

    private ClickHandler ClickHandler;
    private PlayerSprite LocalPlayer;

    public EnemyState EnemyState
    {
        get
        {
            return ClientManager.Instance.StateManager.WorldState
                .GetEnemyState(StateGuid, SceneManager.GetActiveScene().name);

        }
    }

    public float chaseRadius = 6;
    public float attackRadius = 1.2f;
    public float idleRadius = 2;

    private bool attackPending = false;

    void Start()
    {
        SetDirection(direction);
        SetState(state);
        InvokeRepeating("UpdateState", 0, 1);

    }

    protected override void Update()
    {

        if (EnemyState == null) { Destroy(this.gameObject); }

        base.Update();

        if (LocalPlayer == null)
        {
            try { LocalPlayer = GameObject.FindWithTag("LocalPlayer").GetComponent<PlayerSprite>(); } catch { return; }
        }

        if (ClickHandler == null)
        {
            try { ClickHandler = GameObject.FindWithTag("ClickHandler").GetComponent<ClickHandler>(); } catch { return; }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (state != SpriteState.Death && this.GetComponent<CapsuleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                LocalPlayer.TargetEnemy = this;
                ClickHandler.enemyClicked = true;
            }
        }

        // Chase Player
        if (TargetPlayer != null)
        {

            if (state != SpriteState.Death &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) <= chaseRadius &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) > attackRadius)
            {
                SetState(SpriteState.Walk);
                MoveToDestination(TargetPlayer.transform.position);
                return;

            }

            // Attack Player
            if (state != SpriteState.Death &&
                state != SpriteState.Attack1 &&
                !attackPending &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) < attackRadius)
            {
                attackPending = true;

                StartCoroutine(EnemyAttackCoroutine());
                IEnumerator EnemyAttackCoroutine()
                {
                    // Attack delay so the enemy doesnt simply spam attack without interuption
                    yield return new WaitForSeconds(0.5f);
                    ClientManager.Instance.StateManager.Actions.EnemyAttack(this);
                    attackPending = false;
                }
                return;
            }

            //// Move to location?? Return Home ?? See UpdateEnemyLocation
            //if (Vector2.Distance(transform.position, CharacterState.Location) > 0.001)
            //{
            //    SetState(SpriteState.Walk);
            //    MoveToDestination(CharacterState.Location);
            //    return;
            //}

        }

        // Stop Chasing Player and return home
        //if (state != SpriteState.Death &&
        //    Vector3.Distance(TargetPlayer.transform.position, transform.position) > chaseRadius + 2 &&
        //    Vector3.Distance(homePosition, transform.position) > 0)
        //{
        //    SetDirection(homePosition);
        //    SetState(SpriteState.Walk);
        //    MoveToDestination(homePosition);
        //    return;
        //}

        //}
        //}

        SetState(SpriteState.Idle);
    }

    protected override void OnDeath()
    {
        //gameState.generateEnemyDrops(this.gameObject);
    }

    private void UpdateState()
    {
        ClientManager.Instance.StateManager.Actions.UpdateEnemyLocation(
            StateGuid,
            new Vector2(transform.position.x, transform.position.y),
            SceneManager.GetActiveScene().name);
      
    }

}
