using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetType() == typeof(CapsuleCollider2D))
        {
            if (other.CompareTag("canTakeDamage"))
            {

                PlayerSprite player = GameObject.FindWithTag("Player").GetComponent<PlayerSprite>();

                switch (player.selectedAttack)
                {
                    case State.Attack1:
                        other.GetComponent<EnemySprite>().Damage(5, 0);
                        break;
                    case State.Attack2:
                        other.GetComponent<EnemySprite>().Damage(1, 150);
                        break;
                }
            }
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerSprite>().Damage(3, 0);
            }

        }

    }
}
