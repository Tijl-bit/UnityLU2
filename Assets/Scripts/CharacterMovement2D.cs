using UnityEngine;

public class CharacterMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get movement input
        movement.x = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        movement.y = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow
    }

    void FixedUpdate()
    {
        // Move character using Rigidbody2D.MovePosition to respect collisions
        Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        // Debug the movement position to check if it's working correctly
        //Debug.Log("New Position: " + newPosition);

        rb.MovePosition(newPosition);
    }
}
