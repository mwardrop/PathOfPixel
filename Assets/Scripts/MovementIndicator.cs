using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class MovementIndicator : MonoBehaviour
{
    public Sprite move;
    public Sprite attack;

    private PlayerSprite playerSprite;
    private SpriteRenderer spriteRenderer;

    public Boolean enemyClicked = false;
    public Boolean interfaceClicked = false;
    public GameObject playerSpriteObject;

    private IEnumerator coroutine;
    private bool coroutineRunning = false;

    void Start()
    {
        playerSprite = playerSpriteObject.GetComponent<PlayerSprite>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        coroutine = hideIndicatorCoroutine();
    }

    // Update is called once per frame
    void Update()
    {
    
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            transform.position = target;
            spriteRenderer.sortingOrder = playerSprite.spriteRenderer.sortingOrder + 1;

            showIndicator();

            if (enemyClicked)
            {
                if(Vector3.Distance(playerSprite.transform.position, transform.position) < playerSprite.attackRadius)
                {
                    spriteRenderer.sprite = attack;
                } else
                {
                    //enemyClicked = false;
                }
            }
            else
            {
                spriteRenderer.sprite = move;
            }
        }      

        if (spriteRenderer.enabled)
        {
            if (!coroutineRunning)
            {
                if (Vector3.Distance(playerSprite.transform.position, transform.position) < 1)
                {
                    hideIndicator();
                }
            }
        }

        if(enemyClicked)
        {
            spriteRenderer.sprite = attack;
        }
    }

    private void hideIndicator()
    {
        coroutine = hideIndicatorCoroutine();
        StartCoroutine(coroutine);

    }

    private IEnumerator hideIndicatorCoroutine()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.enabled = false;
        coroutineRunning = false;
    }

    private void showIndicator()
    {
        
        if (coroutineRunning)
        {
            StopCoroutine(coroutine);
            
            coroutineRunning = false;
        }

        spriteRenderer.enabled = true;
    }

}
