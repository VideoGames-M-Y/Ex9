using Fusion;
using UnityEngine;

public class ShieldPowerUp : NetworkBehaviour
{
    private Collider shieldCollider;

    public override void Spawned()
    {
        // Ensure a collider is attached to the shield in the editor
        shieldCollider = GetComponent<Collider>();
    }

    // When the player collides with the shield, the shield will disappear
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that collided with the shield is a player
        if (other.CompareTag("Player"))
        {
            // Call the method to make the shield disappear and notify the player
            CollectShield(other.GetComponent<Player>());
        }
    }

    // Make the shield disappear by deactivating it (or despawning it in networked games)
    private void CollectShield(Player player)
    {
        if (player != null)
        {
            player.CollectShield();

            if (HasStateAuthority)
            {
                Runner.Despawn(Object);  // Despawn the shield object
                Debug.Log("Shield collected and despawned!");
            }
        }
    }
}
