using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Make sure the Attack Collider is Colliding with Damage Hitbox (CapsuleCollider2D)
        if (other.GetType() == typeof(CapsuleCollider2D))
        {
            // A Player is hitting an Enemy
            if (this.CompareTag("PlayerAttackCollider") && other.CompareTag("Enemy"))
            {
                GameObject player = this.gameObject.transform.parent.parent.gameObject;
                GameObject enemy = other.gameObject;
                ClientManager.Instance.StateManager.Actions.PlayerHitEnemy(player, enemy);

            }

            // A Enemy is hitting a player
            if (this.CompareTag("EnemyAttackCollider") && (other.CompareTag("LocalPlayer") || other.CompareTag("NetworkPlayer")))
            {
                ClientManager.Instance.StateManager.Actions.EnemyHitPlayer(other.gameObject, this.gameObject.transform.parent.parent.gameObject);
            }

        }

    }
}