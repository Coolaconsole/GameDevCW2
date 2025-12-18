using UnityEngine;
using UnityEngine.InputSystem;
using static StoryManager;

[RequireComponent(typeof(GridbasedMovementScript))]
[RequireComponent(typeof(Hammer))]
[RequireComponent(typeof(Fire))]
public class PlayerInput : MonoBehaviour
{
    //forced delay between two movement inputs
    public float inputCooldownTime = 1;

    //Player needs a movement system
    GridbasedMovementScript movement;
    //Input for player movement
    InputAction moveAction;
    //Input for player ablity (hammer or fire)
    InputAction ablityAction;
    //How long until movement input can occur again
    float inputCooldown = 0;
    //The direction the character is facing
    Direction facingDirection = Direction.North;
    //The component that handles the hammer ablity
    Hammer hammer;
    //The component that handles the fire ablity
    Fire fire;

    void Start()
    {
        movement = GetComponent<GridbasedMovementScript>();
        hammer = GetComponent<Hammer>();
        fire = GetComponent<Fire>();

        //only activate ablity components if have the correct flag
        StoryManager manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        if (manager != null)
        {
            //bind delegate
            hammer.enabled = manager.getEvent("RecivedHammer");
            fire.enabled = manager.getEvent("RecivedFire");
        }
        else
        {
            hammer.enabled = false;
            fire.enabled = false;
        }

        moveAction = InputSystem.actions.FindAction("Move");
        ablityAction = InputSystem.actions.FindAction("UseAblity");
    }

    void Update()
    {
        //check not on cooldown
        if (inputCooldown <= 0)
        {
            if (ablityAction.triggered) 
            {
                if (hammer.enabled) 
                { 
                    hammer.Use(facingDirection, movement);
                    inputCooldown = inputCooldownTime;
                }
                else if (fire.enabled) 
                {
                    fire.Use(facingDirection, movement);
                    inputCooldown = inputCooldownTime;
                }
            }
            else
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
                    facingDirection = moveDirection;
                    movement.Move(moveDirection);
                    //start movement input cooldown
                    inputCooldown = inputCooldownTime;
                }
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
