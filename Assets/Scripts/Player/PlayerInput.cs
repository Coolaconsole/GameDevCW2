using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GridbasedMovementScript))]
public class PlayerInput : MonoBehaviour
{
    //forced delay between two movement inputs
    public float inputCooldownTime = 1;

    //Player needs a movement system
    GridbasedMovementScript movement;
    //Input for player movement
    InputAction moveAction;
    //How long until movement input can occur again
    float inputCooldown = 0;

    void Start()
    {
        movement = GetComponent<GridbasedMovementScript>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        //check not on cooldown
        if (inputCooldown <= 0)
        {
            //get movement input
            Vector2 moveValue = moveAction.ReadValue<Vector2>();

            Direction moveDirection = Direction.None;

            //break into one caridnal direction
            if (moveValue.x > 0)
            {
                moveDirection = Direction.East;
            }
            else if (moveValue.x < 0)
            {
                moveDirection = Direction.West;
            }
            else if (moveValue.y > 0)
            {
                moveDirection = Direction.North;
            }
            else if (moveValue.y < 0)
            {
                moveDirection = Direction.South;
            }

            //if actually moving get movement to try to perform the move
            if (moveDirection != Direction.None)
            {
                movement.Move(moveDirection);
                //start movement input cooldown
                inputCooldown = inputCooldownTime;
            }
        }
        //if still on cooldown, reduce the timer
        else
        {
            inputCooldown -= Time.deltaTime;
            if (inputCooldown < 0) { inputCooldown = 0; }
        }
    }
}
