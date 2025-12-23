using UnityEngine;

public class Hammer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //Called to use the hammer ablity
    public void Use(Direction direction, GridbasedMovementScript movement)
    {
        Vector2 postion = new Vector2(transform.position.x, transform.position.y) + movement.DirectionToVector(direction);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        Collider2D[] results = new Collider2D[5];
        Physics2D.OverlapCircle(postion, movement.gridSquareDist / 2, contactFilter, results);

        foreach (Collider2D collider in results)
        {
            if (collider != null)
            {
                Breakable breakable = collider.GetComponentInParent<Breakable>();
                if (breakable != null)
                {
                    breakable.Break();
                }
            }
        }
    }
}
