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
                if(this.transform.parent.parent.CompareTag("LocalPlayer"))
                {
                    ClientManager.Instance.StateManager.Actions.PlayerAttack(other.gameObject);
                }
                
            }

            // A Enemy is hitting a player
            if (this.CompareTag("Enemy") && other.CompareTag("Player"))
            {
                if (this.transform.parent.parent.CompareTag("LocalPlayer"))
                {
                    ClientManager.Instance.StateManager.Actions.EnemyAttack(other.gameObject);
                }
            }

        }

    }
}