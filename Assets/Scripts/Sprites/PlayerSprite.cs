using System.Linq;
using UnityEngine;


public class PlayerSprite : CharacterSprite
{
    public int NetworkClientId;

    public PlayerState PlayerState
    {
        get
        {
            try
            {
                return ClientManager.Instance.ClientConnection.WorldState.Players.First(x => x.ClientId == NetworkClientId);
            } catch {
                return null;
            }
        }
    }

    public Vector3 target { get
        {
            return new Vector3(PlayerState.TargetLocation.x, PlayerState.TargetLocation.y);
        } 
    }
    //private MovementIndicator movementIndicator;
    
    public SpriteState selectedAttack = SpriteState.Attack1;
    public float attackRadius = 1.2f;
    public float moveRadius = 1;
    public EnemySprite enemy;

    private bool shouldMove = false;

    void Start()
    {
        //movementIndicator = GameObject.FindWithTag("MovementIndicator").GetComponent<MovementIndicator>();

        SetDirection(direction);
        SetState(state);
        SetAttack(SpriteState.Attack1);
    }

    protected override void Update()
    {      
        if(PlayerState == null) { Destroy(this.gameObject); }

        if (Vector3.Distance(transform.position, target) >= moveRadius)
        {
            if(canMove && Vector3.Distance(transform.position, target) > moveRadius) { shouldMove = true; }
            SetDirection(target);
        }

        base.Update();

        //if (Input.GetButtonDown("Attack1"))
        //{
        //    SetAttack(SpriteState.Attack1);
        //}

        //if (Input.GetButtonDown("Attack2"))
        //{
        //    SetAttack(SpriteState.Attack2);
        //}

        //if (Input.GetButtonDown("Attack3"))
        //{
        //    SetAttack(SpriteState.Attack3);
        //}

        // Attack Enemy if in range
        //if (canAttack && 
        //    movementIndicator.enemyClicked && 
        //    Vector3.Distance(transform.position, enemy.transform.position) < attackRadius)
        //{
        //    SetState(selectedAttack);
        //    movementIndicator.enemyClicked = false;
        //    return;
        //}

        if(Vector3.Distance(transform.position, target) < moveRadius)
        {
            shouldMove = false;
        }

        // Move to target
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
