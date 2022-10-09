using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementIndicator : MonoBehaviour
{
    public Sprite move;
    public Sprite attack;

    public Boolean enemyClicked = false;

    // Update is called once per frame
    void Update()
    {

        PlayerSprite player = GameObject.FindWithTag("Player").GetComponent<PlayerSprite>();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            transform.position = target;
            this.GetComponent<SpriteRenderer>().enabled = true;
            if (enemyClicked)
            {
                this.GetComponent<SpriteRenderer>().sprite = attack;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = move;
            }
            enemyClicked = false;
        }      

        if (Vector3.Distance(player.transform.position, transform.position) < 0.5)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        if(enemyClicked)
        {
            this.GetComponent<SpriteRenderer>().sprite = attack;
        }
    }


}
