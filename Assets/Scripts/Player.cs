using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    [SerializeField] private Material shieldMaterial;  // Reference to the shield material
    private HashSet<BadTile> interactedTiles = new HashSet<BadTile>();
    private Material normalMaterial;  // Original material
    private Renderer playerRenderer;  // The player's renderer
    private CharacterController _cc;

    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject ballPrefab;
     [Networked] public int Score { get; private set; } = 0;
    [Networked] public bool _hasShield { get; private set; } = false;

    private Camera firstPersonCamera;

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();

        // Initialize the playerRenderer and normalMaterial
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            normalMaterial = playerRenderer.material;  // Store the original material
        }

        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
                firstPersonCameraComponent.SetTarget(this.transform);
        }
    }

    private Vector3 moveDirection;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.moveActionValue.magnitude > 0)
            {
                inputData.moveActionValue.Normalize();
                moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
                Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
                _cc.Move(DeltaX);
            }

            if (HasStateAuthority)
            {
                if (inputData.shootActionValue)
                {
                    Debug.Log("SHOOT!");
                    var ball = Runner.Spawn(ballPrefab, transform.position + moveDirection, Quaternion.LookRotation(moveDirection), Object.InputAuthority);

                    // Pass the Player reference to the Ball
                    var ballScript = ball.GetComponent<Ball>();
                    if (ballScript != null)
                    {
                        ballScript.SetOwner(this); // Pass the current Player
                    }
                }
            }
        }
    }

    // This method will be called by the shield to indicate it's been collected
    public void CollectShield()
    {
        if (HasStateAuthority)
        {
            RpcSetShield(true);  // Use an RPC to synchronize the shield across the network
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcSetShield(bool value)
    {
        _hasShield = value;

        // // Update the player's material for the shield
        // if (playerRenderer != null && shieldMaterial != null)
        // {
        //     playerRenderer.material = value ? shieldMaterial : normalMaterial;
        // }

        // Debug.Log(value ? "Shield activated!" : "Shield deactivated!");
    }

    // This method will be called when the player takes damage
    public void OnShieldHit()
    {
        if (_hasShield)
        {
            // Log the shield absorbing damage without consuming it
            Debug.Log("Shield absorbed damage, health not affected.");
        }
        else
        {
            Debug.Log("No shield to absorb damage.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void IncreaseScoreRpc(int amount)
    {
        Score += amount;
        Debug.Log($"Score updated via RPC: {Score}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DecreaseScoreRpc(int amount)
    {
        Score -= amount;
        Debug.Log($"Score decreased by {amount}. New Score: {Score}");
    }

    // Check if the tile has been interacted with
    public bool HasInteractedWithTile(BadTile tile)
    {
        return interactedTiles.Contains(tile);
    }

    // Mark the tile as interacted
    public void MarkTileAsInteracted(BadTile tile)
    {
        interactedTiles.Add(tile);
    }

    public void ClearTileInteraction(BadTile tile)
    {
        interactedTiles.Remove(tile);
    }
}
