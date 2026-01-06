using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class BirdMovement : MonoBehaviour
{

    Rigidbody2D rb;

    public float speed = 5;
    public Vector2 Direction = new Vector2(0.5f, 0.5f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
