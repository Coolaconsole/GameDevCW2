using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pusher : MonoBehaviour
{
    //How far it should try to push
    public float gridSquareDist = 1f;
    //How often it should try to push
    public float pushCooldownTime = 1;

    float pushCooldown;

    Vector2 pushVector;
    Direction pushDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (transform.eulerAngles.z)
        {
            case 0:
                pushVector = new Vector2(0, gridSquareDist);
                pushDirection = Direction.North;
                break;
            case 270:
                pushVector = new Vector2(gridSquareDist, 0);
                pushDirection = Direction.East;
                break;
            case 180:
                pushVector = new Vector2(0, -gridSquareDist);
                pushDirection = Direction.South;
                break;
            case 90:
                pushVector = new Vector2(-gridSquareDist, 0);
                pushDirection = Direction.West;
                break;
            default:
                pushVector = new Vector2();
                pushDirection = Direction.None;
                break;
        }
        pushCooldown = pushCooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        //check not on cooldown
        if (pushCooldown <= 0)
        {
            Vector2 postion = new Vector2(transform.position.x, transform.position.y) + pushVector;
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            Collider2D[] results = new Collider2D[5];
            Physics2D.OverlapCircle(postion, gridSquareDist / 2, contactFilter, results);

            foreach (Collider2D collider in results)
            {
                if (collider != null)
                {
                    GridbasedMovementScript mover = collider.GetComponentInParent<GridbasedMovementScript>();
                    if (mover != null)
                    {
                        mover.Move(pushDirection);
                    }
                }
            }
            pushCooldown = pushCooldownTime;
        }
        //if still on cooldown, reduce the timer
        else
        {
            pushCooldown -= Time.deltaTime;
            if (pushCooldown < 0) { pushCooldown = 0; }
        }
    }
}
