using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Target")]
    // The object the camera will focus on (the player)
    public Transform playerTransform;

    [Header("Camera Settings")]
    // How much the mouse position influences the camera's position.
    // 0 = camera is always on player. 1 = camera is always on mouse.
    [Range(0, 1)]
    public float mouseInfluence = 0.2f;

    // How quickly the camera moves to its target position. Higher values are faster.
    public float smoothing = 5f;

    // The Z-axis position of the camera. For a 2D game, this should be negative.
    public float cameraZOffset = -10f;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();

        // Error handling in case the playerTransform is not assigned
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned on the CameraFollow script. Please assign it in the Inspector.");
            // Disable the script to prevent further errors
            this.enabled = false;
        }
    }

    // LateUpdate is called after all Update functions have been called.
    // This is the best place to move a camera, as it ensures the player has
    // already moved for the current frame, preventing camera jitter.
    void LateUpdate()
    {
        // 1. Find the target position
        // ---------------------------

        // Get the mouse position in world coordinates
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // We only care about the X and Y for a 2D game, so set Z to the player's Z
        mouseWorldPosition.z = playerTransform.position.z;

        // Calculate the target position. This is the key part.
        // We use Lerp (Linear Interpolation) to find a point between the player and the mouse.
        // The 'mouseInfluence' value determines how far towards the mouse the camera will look.
        Vector3 targetPosition = Vector3.Lerp(playerTransform.position, mouseWorldPosition, mouseInfluence);

        // 2. Set the camera's final position
        // ------------------------------------

        // Keep the camera's Z position constant using the offset
        targetPosition.z = cameraZOffset;

        // Smoothly move the camera from its current position to the target position.
        // Time.deltaTime makes the movement frame-rate independent.
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}
