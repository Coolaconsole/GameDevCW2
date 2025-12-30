using UnityEngine;

// This attribute allows the script to run in the Scene view without pressing Play
[ExecuteInEditMode]
public class WallConnector : MonoBehaviour
{
    [Header("Sprite Configuration")]
    public Sprite[] wallVariants; // Array of 16 sprites ordered by bitmask
    
    [Header("Detection Settings")]
    public LayerMask wallLayer;   // Set this to the layer the walls are on
    public float checkDistance = 1.0f;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame (and when objects move in Editor)
    void Update()
    {
        UpdateVisuals();
    }
    /// Checks the 4 cardinal directions for neighbors and updates the sprite.
    public void UpdateVisuals()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (wallVariants == null || wallVariants.Length < 16) return;

        int mask = 0;

        // Bitmask calculation: Top(1), Right(2), Bottom(4), Left(8)
        if (HasNeighbor(Vector2.up))    mask += 1;
        if (HasNeighbor(Vector2.right)) mask += 2;
        if (HasNeighbor(Vector2.down))  mask += 4;
        if (HasNeighbor(Vector2.left))  mask += 8;

        // Apply the sprite from the array based on the calculated neighbor mask
        _spriteRenderer.sprite = wallVariants[mask];
    }

    
    /// Uses Physics2D to check if another wall exists at the specified direction.
    private bool HasNeighbor(Vector2 direction)
    {

        Vector2 checkPos = (Vector2)transform.position + (direction * checkDistance);
        
        // Check if there's a collider at the neighbor position
        Collider2D hit = Physics2D.OverlapPoint(checkPos, wallLayer);
        
        return hit != null && hit.gameObject != gameObject;
    }
}