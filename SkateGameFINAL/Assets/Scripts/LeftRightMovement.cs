using UnityEngine;

public class LeftRightMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;  // Speed of movement
    [SerializeField] private float interval = 2.0f;  // Time in seconds before changing direction

    private Vector3 startPosition;  // Store the initial position
    private bool movingRight = true;  // Direction of movement

    void Start()
    {
        startPosition = transform.position;  // Store the initial position of the object
        InvokeRepeating(nameof(ChangeDirection), interval, interval);  // Call ChangeDirection at regular intervals
    }

    void Update()
    {
        // Move in the current direction
        if (movingRight)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void ChangeDirection()
    {
        movingRight = !movingRight;  // Toggle the direction
    }
}
