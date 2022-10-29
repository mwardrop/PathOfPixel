using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Make sure the Attack Collider is Colliding with Damage Hitbox (CapsureCollider2D)
        if (other.GetType() == typeof(CapsuleCollider2D))
        {
            // A Player is hitting an Enemy
            if (this.CompareTag("Player") && other.CompareTag("Enemy"))
            {

                PlayerSprite player = GameObject.FindWithTag("LocalPlayer").GetComponent<PlayerSprite>();

                switch (player.selectedAttack)
                {
                    case SpriteState.Attack1:
                        other.GetComponent<EnemySprite>().Damage(5, 0);
                        break;
                    case SpriteState.Attack2:
                        other.GetComponent<EnemySprite>().Damage(1, 150);
                        break;
                }
            }

            // A Enemy is hitting a player
            if (this.CompareTag("Enemy") && other.CompareTag("Player"))
            {
                other.GetComponent<PlayerSprite>().Damage(3, 0);
            }

        }

    }
}