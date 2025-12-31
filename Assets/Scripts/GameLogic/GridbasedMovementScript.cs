using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GridbasedMovementScript : MonoBehaviour
{
    //Distance the object moves - needs to be consitent in map design and all objects using this
    public float gridSquareDist = 1f;

    //Ridgid body responsible for movement
    Rigidbody2D rb;
    //Collision to prevent moving into objects
    Collider2D collision;

    public bool canTalkTo = false;
    public bool isImmovable = false;
    public List<string> dialogueLines = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collider2D>();
    }

    //Move the parent one grid space in the specified direction - returns if the space was able to be moved into
    public bool Move(Direction direction) {
        if (isImmovable) {
            return false;
        }
        //Work out the change in position
        Vector2 movementVector = DirectionToVector(direction);

        //check if can move - propogating to those it is pushing
        if (CanMove(direction))
        {
            //get the items being puhsed
            RaycastHit2D[] testResults = new RaycastHit2D[5];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            collision.Cast(movementVector, contactFilter, testResults, gridSquareDist);
            foreach (RaycastHit2D raycastHit2D in testResults)
            {
                if (raycastHit2D.collider != null)
                {
                    //know can call this as will always be valid
                    raycastHit2D.collider.GetComponentInParent<GridbasedMovementScript>().Move(direction);
                }
            }

            //actually move
            rb.MovePosition(rb.position + movementVector);
            return true;
        }
        return false;
    }

    public bool CanMove(Direction direction) {
        //Work out the change in position
        Vector2 movementVector = DirectionToVector(direction);

        //Check if movement is blocked
        RaycastHit2D[] testResults = new RaycastHit2D[5];
        bool canMove = true;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        collision.Cast(movementVector, contactFilter, testResults, gridSquareDist);
        foreach (RaycastHit2D raycastHit2D in testResults)
        {
            //if blocked check if blocking object can also move
            if (raycastHit2D.collider != null)
            {
                GridbasedMovementScript otherMovement = raycastHit2D.collider.GetComponentInParent<GridbasedMovementScript>();
                if (otherMovement != null)
                {
                    if (otherMovement.CanTalkTo() && otherMovement.GetDialogueLines().Count > 0)
                    {
                        TextManager.Instance.QueuePrompt(otherMovement.GetDialogueLines()[0]);
                        otherMovement.DecrementDialogueLines();
                        Debug.Log("Talking to " + otherMovement.gameObject.name);
                        //cannot move into characters
                        canMove = false;
                        break;
                    }

                    //if movable try to move it and if success then this can move as well
                    if (otherMovement.CanMove(direction))
                    {
                        canMove = true;
                        continue;
                    }
                }
                canMove = false;
                break;
            }
        }
        return canMove;
    }

    //whether this object can be talked to
    public bool CanTalkTo() {
        return canTalkTo;
    }

    public List<string> GetDialogueLines() {
        return dialogueLines;
    }
    public void DecrementDialogueLines() {
        if (dialogueLines.Count > 1) {
            dialogueLines.RemoveAt(0); // Repeat last line if no more lines
        }
    }

    //convert direction into a vector - used for moving rb and traces while mainating only cardinal movement
    public Vector2 DirectionToVector(Direction direction) {
        switch (direction)
        {
            case Direction.North:
                return new Vector2(0, gridSquareDist);
            case Direction.East:
                return new Vector2(gridSquareDist, 0);
            case Direction.South:
                return new Vector2(0, -gridSquareDist);
            case Direction.West:
                return new Vector2(-gridSquareDist, 0);
            case Direction.None:
                return new Vector2();
            default:
                return new Vector2();
        }
    }
}

//Defines the different directions a grid object can move on a given frame
public enum Direction { North, East, South, West, None };
