using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipper : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;

    // A tiny buffer to prevent flipping due to floating point rounding errors
    [SerializeField] private float flipThreshold = 0.01f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Record the starting position so we have a baseline
        lastPosition = transform.position;
    }

    /// LateUpdate runs after the Movement script has finished its work for the frame.
    /// This ensures we are comparing the position AFTER the move has happened.

    void LateUpdate()
    {
        // Calculate how much we moved on the X axis this frame
        float movementX = transform.position.x - lastPosition.x;

        // Determine flipping based on the difference
        if (movementX > flipThreshold)
        {
            // Player moved East (Right)
            spriteRenderer.flipX = false;
        }
        else if (movementX < -flipThreshold)
        {
            // Player moved West (Left)
            spriteRenderer.flipX = true;
        }

        // Store the current position to compare against in the next frame
        lastPosition = transform.position;
    }
}