using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    private float canvasY;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned in MinimapMarkerFollow.");
            enabled = false;
            return;
        }

        // Store the canvas's fixed Y position at start
        canvasY = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Update red dot to match player XZ, but keep Y from the canvas
        transform.position = new Vector3(
            player.position.x,
            canvasY,
            player.position.z


        );
    }
}
