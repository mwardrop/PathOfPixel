using System.Collections;
using UnityEngine;

public enum ChestState
{
    Idle,
    Open,
    Looted
}

public enum ChestRarity
{
    Common,
    Magic,
    Rare,
    Legendary,
    Mythic
}

public class ChestSprite : BaseSprite
{

    public float openRadius = 1f;
    private ChestState chestState = ChestState.Idle;
    private bool clicked = false;
    private GameObject player;
    public StateManager gameState;
    public int chestLevel = 1;
    public ChestRarity chestRarity = ChestRarity.Common;

    void Start()
    {
        player = GameObject.FindWithTag("LocalPlayer");
    }

    protected override void Update()
    {
        spriteRenderer.sortingOrder = (int)((-transform.position.y) * 100f);

        if (Input.GetMouseButtonDown(0))
        {
            if (this.GetComponent<CapsuleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {    
                clicked = true;
            }
        }

        if (clicked && 
            chestState == ChestState.Idle && 
            Vector3.Distance(transform.position, player.transform.position) <= openRadius)
        {
            SetState(ChestState.Open);
        }
    }

    void SetState(ChestState state)
    {
        if (chestState != state)
        {
            chestState = state;

            animator.SetBool(state.ToString(), true);
            
            // Set Animator to Current State
            switch (state)
            {
                // Interruptible Animations
                case ChestState.Idle:
                    break;
                case ChestState.Looted:
                    //gameState.generateChestDrops(this.gameObject);
                    break;
                case ChestState.Open:
                    Loot();
                    break;

            }

        }
    }

    public void Loot()
    {

        StartCoroutine(LootCoroutine());

        IEnumerator LootCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            SetState(ChestState.Looted);


            
        }
    }
}
