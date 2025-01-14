using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] NumberField HealthDisplay;

    [Networked] public int NetworkedHealth { get; set; } = 100;

    private ChangeDetector _changes;
    private Player _player;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        HealthDisplay.SetNumber(NetworkedHealth);
        _player = GetComponent<Player>();
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NetworkedHealth):
                    HealthDisplay.SetNumber(NetworkedHealth);
                    break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(int damage)
    {
        // Check if the player has collected the shield
        if (_player != null && _player._hasShield)  // Using the flag from the Player class
        {
            // If the player has a shield, it absorbs damage, but doesn't consume the shield
            _player.OnShieldHit();  // Shield absorbs damage, but stays active
            Debug.Log("Shield absorbed damage, no health lost.");
        }
        else
        {
            // If the player doesn't have a shield, apply damage to health
            NetworkedHealth -= damage;
            Debug.Log("Health decreased.");
        }
    }
}