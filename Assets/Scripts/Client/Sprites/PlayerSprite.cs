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
            return ClientManager.Instance.StateManager.WorldState.GetPlayerState(NetworkClientId);
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
        base.Update();

        if (PlayerState == null) { Destroy(this.gameObject); }

        if(Attack()) { return; }
        if(Move()) { return; }

        SetState(SpriteState.Idle);
    }

    private bool Attack()
    {
        if (this.CompareTag("LocalPlayer") &&
            TargetEnemy != null &&
            ClickHandler.enemyClicked &&
            Vector3.Distance(transform.position, TargetEnemy.transform.position) < AttackRadius)
        {
            ClientManager.Instance.StateManager.Actions.PlayerAttack();
            ClickHandler.enemyClicked = false;
            return true;
        }
        return false;
    }

    private bool Move()
    {
        if (!PlayerState.IsDead &&
            Vector3.Distance(transform.position, Target) >= MoveRadius &&
            Vector3.Distance(transform.position, Target) > 0.001)
        {
            SetState(SpriteState.Walk);
            MoveToDestination(Target);
            return true;
        }
        return false;
    }

    private void SetAttack(SpriteState attack)
    {
        SelectedAttack = attack;
    }

}
