using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            try
            {
                return ClientManager.Instance.StateManager.WorldState
                    .GetEnemyState(StateGuid, SceneManager.GetActiveScene().name);
            }
            catch
            {
                return null;
            }
        }
    }

    public float chaseRadius = 6;
    public float attackRadius = 1.2f;
    public float idleRadius = 2;

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

            if (canMove &&
                state != SpriteState.Death &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) <= chaseRadius &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) > attackRadius)
            {
                SetState(SpriteState.Walk);
                MoveToDestination(TargetPlayer.transform.position);
                return;

            }

            // Attack Player
            if (canAttack &&
                state != SpriteState.Death &&
                state != SpriteState.Attack1 &&
                Vector3.Distance(TargetPlayer.transform.position, transform.position) < attackRadius)
            {
                ClientManager.Instance.StateManager.Actions.EnemyAttack(this);
                //SetState(SpriteState.Attack1);
                return;

            }

        }

        //// Stop Chasing Player and return home
        //if (canMove &&
        //    state != SpriteState.Death &&
        //    Vector3.Distance(target.transform.position, transform.position) > chaseRadius + 2 && 
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
