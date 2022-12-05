using System;
using UnityEngine;

public class CurrencyDropSprite : BaseSprite
{
    public CurrencyType Type = CurrencyType.StandardSmall;

    public Guid CurrencyGuid;
    public CurrencyState CurrencyDrop;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("StandardSmall", false);
        animator.SetBool("StandardLarge", false);
        animator.SetBool("PremiumSmall", false);
        animator.SetBool("PremiumLarge", false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update();

        if(CurrencyDrop != null && CurrencyDrop.Type != Type) { Type = CurrencyDrop.Type; }

        switch (Type)
        {
            case CurrencyType.StandardSmall:
                if (!animator.GetBool("StandardSmall")) {
                    animator.SetBool("StandardSmall", true);
                }
                break;
            case CurrencyType.StandardLarge:
                if (!animator.GetBool("StandardLarge")) {
                    animator.SetBool("StandardLarge", true);
                }
                break;
            case CurrencyType.PremiumSmall:
                if (!animator.GetBool("PremiumSmall")) {
                    animator.SetBool("PremiumSmall", true);
                }
                break;
            case CurrencyType.PremiumLarge:
                if (!animator.GetBool("PremiumLarge")) {
                    animator.SetBool("PremiumLarge", true);
                }
                break;
        }

        if (Input.GetMouseButtonDown(0) && this.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            var playerObject = GameObject.FindWithTag("LocalPlayer");
            var playerSprite = playerObject.GetComponent<PlayerSprite>();

            if (Vector2.Distance(playerObject.transform.position, transform.position) <= playerSprite.PickupRadius)
            {
                ClientManager.Instance.StateManager.Actions.CurrencyPickedUp(CurrencyDrop, playerSprite.PlayerState.Scene);
            }
        }

    }
}
