using UnityEngine;

// This attribute allows the script to run in the Scene view without pressing Play
[ExecuteInEditMode]
public class WallConnector : MonoBehaviour
{
    [Header("Behavior")]
    // When checked, the connection logic is skipped
    public bool isStatic = false; 
    // The sprite to use when 'isStatic' is true 
    public Sprite staticSprite; 

    [Header("Sprite Configuration")]
    public Sprite[] wallVariants; // Array of 16 sprites ordered by bitmask
    
    [Header("Detection Settings")]
    public LayerMask wallLayer;   // Set this to the layer the walls/rivers are on
    public float checkDistance = 1.0f;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // If it's static, force the static sprite and stop
        if (isStatic)
        {
            if (_spriteRenderer != null && staticSprite != null)
            {
                _spriteRenderer.sprite = staticSprite;
            }
            return;
        }

        UpdateVisuals();
    }

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

        _spriteRenderer.sprite = wallVariants[mask];
    }

    private bool HasNeighbor(Vector2 direction)
    {
        Vector2 checkPos = (Vector2)transform.position + (direction * checkDistance);
        Collider2D hit = Physics2D.OverlapPoint(checkPos, wallLayer);
        
        return hit != null && hit.gameObject != gameObject;
    }
}