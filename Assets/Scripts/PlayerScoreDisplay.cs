using UnityEngine;
using Fusion;

public class PlayerScoreDisplay : NetworkBehaviour
{
    [SerializeField] private NumberField PointsDisplay; // Reference to the score display UI

    private Player _player;

    void Start()
    {
        // Get the Player component from the same GameObject
        _player = GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player component not found on this GameObject!");
        }

        if (PointsDisplay == null)
        {
            Debug.LogError("PointsDisplay (NumberField) not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // Update the points display if the player and display exist
        if (_player != null && PointsDisplay != null)
        {
            PointsDisplay.SetNumber(_player.Score); // Update the NumberField with the player's score
        }
    }
}