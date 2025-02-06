using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;  // Speed at which the object moves
    [SerializeField] private float sprintSpeed = 5.5f;  // Speed when sprinting
    [SerializeField] private float rotationSpeed = 4f;  // Speed of rotation (higher value = faster rotation)
    [SerializeField] private Transform cameraTransform;  // Reference to the camera's transform
    [SerializeField] private float jumpForce = 5f;  // Force applied when jumping

    private Rigidbody rb;  // Reference to the player's Rigidbody
    private Quaternion targetRotation;  // Store the target rotation
    private bool isGrounded;  // Flag to check if the player is grounded

    public bool groundTouch;
    private DeathAndRespawn spawner;
    private Coroutine resetSpeedCoroutine;
    private RedGreen rg;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        // Initialize the target rotation to the current rotation
        targetRotation = transform.rotation;

        spawner = GameObject.Find("Skatepark").GetComponent<DeathAndRespawn>();

        rg = GameObject.Find("RedGreen").GetComponent<RedGreen>();
    }

    void Update()
    {
        if (rg.canMove == false && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space)))
        {
            spawner.RespawnPlayer();
        }
        // Check for sprinting (Shift key) and adjust moveSpeed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // Handle input for movement direction
        Vector3 direction = GetMovementDirection();

        // If there's any movement input, update the target rotation
        if (direction != Vector3.zero)
        {
            // Calculate the target rotation based on the direction relative to the camera
            targetRotation = Quaternion.LookRotation(direction);

            // Move the object forward
            MoveCharacter(currentSpeed);
        }

        // Apply rotation smoothly towards the target rotation
        RotateCharacter();

        // Jump logic (when spacebar is pressed and player is grounded)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 direction = Vector3.zero;

        // Get the forward and right directions relative to the camera's rotation
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f; // Ignore vertical component to keep movement on the X-Z plane
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f; // Ignore vertical component
        right.Normalize();

        // Check for input and calculate movement direction based on the camera's orientation
        if (Input.GetKey(KeyCode.W)) direction += forward;  // Move forward relative to the camera
        if (Input.GetKey(KeyCode.S)) direction -= forward;  // Move backward relative to the camera
        if (Input.GetKey(KeyCode.A)) direction -= right;    // Move left relative to the camera
        if (Input.GetKey(KeyCode.D)) direction += right;    // Move right relative to the camera
        return direction;
    }

    private void RotateCharacter()
    {
        // Rotate smoothly towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveCharacter(float speed)
    {
        // Move the player in the direction it's facing (using Rigidbody for physics-based movement)
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void Jump()
    {
        // Apply an upward force to the Rigidbody to make the player jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Detect collision with objects to reset the jump when touching a specific tag
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player has collided with an object that has the specified tag
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Carousel") || collision.gameObject.CompareTag("RedGreen"))
        {
            isGrounded = true; // Reset the grounded flag when touching the ground
            Debug.Log("grounded");
            if(collision.gameObject.CompareTag("Ground")){
                groundTouch = true;
                if (rg != null)
                {
                    rg.StopRedGreen();
                }
                else
                {
                }
            }
        }
        // Check if the player collides with an object tagged as "Death"
        else if (collision.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            spawner.RespawnPlayer();
        }
        else if (collision.gameObject.CompareTag("FinishLine"))
        {
            SceneManager.LoadScene("EndMenu");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carousel"))
        {
            moveSpeed = 2.5f;
            sprintSpeed = 4f;

            // If there's a coroutine running, stop it to prevent premature reset
            if (resetSpeedCoroutine != null)
            {
                StopCoroutine(resetSpeedCoroutine);
                resetSpeedCoroutine = null;
            }
        }
        else if (collision.gameObject.CompareTag("RedGreen"))
        {
            moveSpeed = 1.5f;
            sprintSpeed = 3f;

            // If there's a coroutine running, stop it to prevent premature reset
            if (resetSpeedCoroutine != null)
            {
                StopCoroutine(resetSpeedCoroutine);
                resetSpeedCoroutine = null;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carousel"))
        {
            resetSpeedCoroutine = StartCoroutine(ResetSpeedAfterDelay());
        }
        else if (collision.gameObject.CompareTag("RedGreen"))
        {
            // Start the coroutine to reset the speed after the delay
            resetSpeedCoroutine = StartCoroutine(ResetSpeedAfterDelay());
        }
    }

    private IEnumerator ResetSpeedAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(2f);

        // Reset the move and sprint speeds to their defaults
        moveSpeed = 3.5f;
        sprintSpeed = 5.5f;

        // Clear the coroutine reference
        resetSpeedCoroutine = null;
    }
}