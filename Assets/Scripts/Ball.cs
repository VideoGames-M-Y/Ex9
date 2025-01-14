using Fusion;
using UnityEngine;

/**
 * This component represents a ball moving at a constant speed.
 */
public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }

    [SerializeField] float lifeTime = 5.0f;
    [SerializeField] float speed = 5.0f;
    [SerializeField] int damagePerHit = 1;
    [SerializeField] int scorePerHit = 5;
    private Player owner;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += speed * transform.forward * Runner.DeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        Player hitPlayer = other.GetComponent<Player>();

        if (health != null)
        {
            health.DealDamageRpc(damagePerHit);
        }

        if (owner != null)
        {
            if (hitPlayer != null && hitPlayer._hasShield)
            {
                Debug.Log($"Player {hitPlayer.name} has a shield, no points awarded.");
                hitPlayer.OnShieldHit();
                return;
            }
            else
            {
                owner.IncreaseScoreRpc(scorePerHit);
                Debug.Log($"Score increased by {scorePerHit} for {owner.name}");
            }
        }
        else
        {
            Debug.LogWarning("Owner not set for the ball!");
        }
    }

    public void SetOwner(Player player)
    {
        owner = player;
    }
}
