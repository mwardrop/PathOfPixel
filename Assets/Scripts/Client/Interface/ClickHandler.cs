using System;
using System.Collections;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public Sprite move;
    public Sprite attack;

    private PlayerSprite playerSprite;
    private SpriteRenderer spriteRenderer;

    public Boolean enemyClicked = false;
    public Boolean interfaceClicked = false;

    private IEnumerator coroutine;
    private bool coroutineRunning = false;

    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        coroutine = hideIndicatorCoroutine();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerSprite == null) {

            GameObject localPlayer = GameObject.FindWithTag("LocalPlayer");
            if(localPlayer != null)
            {
                playerSprite = localPlayer.GetComponent<PlayerSprite>();
            }

            return;
        }

        if (enemyClicked) {
            spriteRenderer.sprite = attack;
        } else {
            spriteRenderer.sprite = move;
        }

        if (Input.GetMouseButtonDown(0))
        {

            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;

            ClientManager.Instance.StateManager.Actions.RequestMove(target);

            transform.position = target;

            spriteRenderer.sortingOrder = playerSprite.spriteRenderer.sortingOrder + 1;

            showIndicator();

            enemyClicked = false;
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
