using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] private Transform[] playerTransforms;
    private Camera minimapCamera;

    [Header("Camera Settings")]
    [SerializeField] private float smoothTime = 0.3f;  // Camera movement smoothness
    [SerializeField] private float fixedZPosition = -50f;

    private Vector3 velocity = Vector3.zero;

    [Header("Marker Settings")]
    [SerializeField] private GameObject markerPrefab;  // Prefab for the offscreen marker
    [SerializeField] private Color[] markerColors;       // One color per player
    [SerializeField] private float markerOffset = 0.1f;    // Optional: extra offset from edge

    // Dictionary to track markers per player
    private Dictionary<Transform, GameObject> playerMarkers = new Dictionary<Transform, GameObject>();

    void Start()
    {
        // Get all player transforms (if not assigned via Inspector, for example)
        if (playerTransforms == null || playerTransforms.Length == 0)
        {
            playerTransforms = GameObject.FindGameObjectsWithTag("Player")
                .Select(p => p.transform)
                .ToArray();
        }

        minimapCamera = GetComponent<Camera>();

        // Lock Z position for the minimap camera
        transform.position = new Vector3(transform.position.x, transform.position.y, fixedZPosition);
    }

    public void UpdateNumOfPlayers()
    {
        playerTransforms = GameObject.FindGameObjectsWithTag("Player")
            .Select(p => p.transform)
            .ToArray();
    }

    void LateUpdate()
    {
        if (playerTransforms.Length == 0) return;

        // Calculate the center point of all players and move the camera
        Vector3 centerPoint = CalculatePlayersCenter();
        MoveCamera(centerPoint);

        // For each player, update or create marker if off-screen
        foreach (Transform player in playerTransforms)
        {
            // Convert player's position to viewport coordinates
            Vector3 viewportPos = minimapCamera.WorldToViewportPoint(player.position);
            bool onScreen = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;

            // If a marker doesn't exist yet for this player, create it
            if (!playerMarkers.TryGetValue(player, out GameObject marker))
            {
                marker = Instantiate(markerPrefab);
                // Optionally, set its parent so it stays on top of the minimap (e.g., a dedicated UI canvas)
                // marker.transform.SetParent(yourCanvasTransform, false);
                playerMarkers.Add(player, marker);
            }

            if (onScreen)
            {
                // Hide marker when the player is on screen
                marker.SetActive(false);
            }
            else
            {
                // Show marker and position it at the edge of the viewport
                marker.SetActive(true);

                // Clamp the viewport coordinate to [0, 1]
                float clampedX = Mathf.Clamp(viewportPos.x, 0f, 1f);
                float clampedY = Mathf.Clamp(viewportPos.y, 0f, 1f);
                Vector3 clampedViewportPos = new Vector3(clampedX, clampedY, viewportPos.z);

                // Optionally, add a small offset so the marker isn't exactly at the border
                if (clampedX == 0f || clampedX == 1f)
                    clampedViewportPos.x = (clampedX == 0f) ? 0f + markerOffset : 1f - markerOffset;
                if (clampedY == 0f || clampedY == 1f)
                    clampedViewportPos.y = (clampedY == 0f) ? 0f + markerOffset : 1f - markerOffset;

                // Convert back to world space
                Vector3 markerWorldPos = minimapCamera.ViewportToWorldPoint(clampedViewportPos);
                // Ensure the marker stays on the same Z plane as the minimap camera's UI elements (or adjust accordingly)
                markerWorldPos.z = fixedZPosition;

                marker.transform.position = markerWorldPos;

                // Set marker color based on player index (if markerColors is set)
                int playerIndex = System.Array.IndexOf(playerTransforms, player);
                if (playerIndex >= 0 && playerIndex < markerColors.Length)
                {
                    // If marker has an Image component, set its color
                    Image img = marker.GetComponent<Image>();
                    if (img != null)
                        img.color = markerColors[playerIndex];
                }
            }
        }
    }

    Vector3 CalculatePlayersCenter()
    {
        Vector3 combined = Vector3.zero;
        foreach (Transform player in playerTransforms)
        {
            combined += player.position;
        }
        return combined / playerTransforms.Length;
    }

    void MoveCamera(Vector3 targetCenter)
    {
        // Keep fixed Z position, move only X/Y
        Vector3 targetPosition = new Vector3(targetCenter.x, targetCenter.y, fixedZPosition);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
