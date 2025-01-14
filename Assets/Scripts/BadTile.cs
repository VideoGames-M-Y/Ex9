using Fusion;
using UnityEngine;

public class BadTile : NetworkBehaviour
{
    [SerializeField] private int pointsToDeduct = 10;
    private Collider tileCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && !player.HasInteractedWithTile(this))
            {
                Debug.Log("Player hit BadTile.");
                player.MarkTileAsInteracted(this); // Mark tile as interacted
                decrease(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Player left BadTile.");
                player.ClearTileInteraction(this); // Allow interaction with this tile again
            }
        }
    }

    private void decrease(Player player)
    {
        if (player != null)
        {
            Debug.Log("Calling DecreaseScoreRpc");
            player.DecreaseScoreRpc(pointsToDeduct);
        }
    }
}
