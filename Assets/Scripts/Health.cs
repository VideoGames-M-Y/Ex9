using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private NumberField HealthDisplay;

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
        if (_player != null && _player._hasShield)
        {
            _player.OnShieldHit();
            Debug.Log("Shield absorbed damage, no health lost.");
        }
        else
        {
            NetworkedHealth -= damage;
            Debug.Log("Health decreased.");
        }
    }
}
