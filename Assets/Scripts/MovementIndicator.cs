using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementIndicator : MonoBehaviour
{
    public Sprite move;
    public Sprite attack;

    private PlayerSprite player;
    private SpriteRenderer spriteRenderer;

    public Boolean enemyClicked = false;

    private IEnumerator coroutine;
    private bool coroutineRunning = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerSprite>();
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
            spriteRenderer.sortingOrder = player.spriteRenderer.sortingOrder + 1;

            showIndicator();

            if (enemyClicked)
            {
                spriteRenderer.sprite = attack;
            }
            else
            {
                spriteRenderer.sprite = move;
            }
            enemyClicked = false;
        }      

        if (spriteRenderer.enabled)
        {
            if (!coroutineRunning)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < 1)
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
