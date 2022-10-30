using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class PlayerSprite : CharacterSprite
{
    public int NetworkClientId;

    public PlayerState PlayerState
    {
        get
        {
            try
            {
                return ClientManager.Instance.StateManager.WorldState.Players.First(x => x.ClientId == NetworkClientId);
            } catch {
                return null;
            }
        }
    }

    private ClickHandler ClickHandler;
    
    public SpriteState SelectedAttack = SpriteState.Attack1;
    public float AttackRadius = 1.2f;
    public float MoveRadius = 1;
    public EnemySprite TargetEnemy;

    private bool shouldMove = false;

    void Start()
    {
        ClickHandler = GameObject.FindWithTag("ClickHandler").GetComponent<ClickHandler>();

        SetDirection(direction);
        SetState(state);
        SetAttack(SpriteState.Attack1);
    }

    protected override void Update()
    {      
        if(PlayerState == null) { Destroy(this.gameObject); }

        if (Vector3.Distance(transform.position, Target) >= MoveRadius)
        {
            if(canMove && Vector3.Distance(transform.position, Target) > MoveRadius) { shouldMove = true; }
            SetDirection(Target);
        }

        base.Update();

        // Attack Enemy if in range
        if (this.CompareTag("LocalPlayer") &&
            canAttack &&
            TargetEnemy != null &&
            ClickHandler.enemyClicked &&
            Vector3.Distance(transform.position, TargetEnemy.transform.position) < AttackRadius)
        {
            SetState(SelectedAttack);
            // TODO : resetting the click handler is needed so you can repeated click attack but messes up the attack indicator
            ClickHandler.enemyClicked = false;
            return;
        }

        if (Vector3.Distance(transform.position, Target) < 0.001)
        {
            shouldMove = false;
        } 

        // Move to target
        if (shouldMove)
        {      
            SetState(SpriteState.Walk);
            MoveToDestination(Target);
            return;
        }

        SetState(SpriteState.Idle);
    }


    private void SetAttack(SpriteState attack)
    {
        SelectedAttack = attack;
    }

}
