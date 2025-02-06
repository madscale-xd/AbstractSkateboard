using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class AbstractSkateboard : MonoBehaviour
{
    // Player stats
    public string playerName;
    public float health;
    public float speed;
    public float specialMoveGauge;
    
    private DeathAndRespawn spawner;
    private RedGreen rg;
    private Coroutine resetSpeedCoroutine;

    // Core movement and jump variables
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] protected float sprintSpeed = 5.5f;
    [SerializeField] protected float rotationSpeed = 4f;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected Transform skateboardD;  // Reference to the player's skateboard model
    [SerializeField] protected Transform CharacterModel; // Reference to the character's body model

    // Movement-related fields
    protected Rigidbody rb;
    protected Quaternion targetRotation;
    protected bool isGrounded;
    protected bool groundTouch;

    protected bool hasJumped = false;

    protected bool isGrinding = false; // Track if player is grinding

    protected bool canFlip = true; // Prevent multiple flips without landing

    protected Quaternion originalRotation;

    // For saving/loading player state
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;

        spawner = GameObject.Find("Skatepark").GetComponent<DeathAndRespawn>();
        rg = GameObject.Find("RedGreen").GetComponent<RedGreen>();

        // Store the original rotation
        originalRotation = skateboardD.rotation;

        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;
        LoadPlayerData();
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetFloat("Health", health);
        PlayerPrefs.SetFloat("SpecialMoveGauge", specialMoveGauge);
        PlayerPrefs.Save();
        Debug.Log("Player data saved!");
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            health = PlayerPrefs.GetFloat("Health");
            specialMoveGauge = PlayerPrefs.GetFloat("SpecialMoveGauge");
            Debug.Log("Player data loaded!");
        }
    }

    // Core movement handling (WASD, jump)
    protected virtual void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 direction = GetMovementDirection();

        if (direction != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(direction);
            MoveCharacter(currentSpeed);
        }

        RotateCharacter();
    }

    protected virtual void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 direction = Vector3.zero;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        if (Input.GetKey(KeyCode.W)) direction += forward;
        if (Input.GetKey(KeyCode.S)) direction -= forward;
        if (Input.GetKey(KeyCode.A)) direction -= right;
        if (Input.GetKey(KeyCode.D)) direction += right;

        return direction;
    }

    private void RotateCharacter()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveCharacter(float speed)
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void Jump()
    {
        hasJumped = true;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Abstract method for collision exit logic (to be overridden in concrete class)
    

    // Override OnCollisionEnter to add specific collision behavior for PlayerMovement2
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Carousel") || collision.gameObject.CompareTag("RedGreen"))
        {
            isGrounded = true; // Reset the grounded flag when touching the ground
            Debug.Log("grounded");
            Debug.Log("can flip");

            if (collision.gameObject.CompareTag("Ground"))
            {
                groundTouch = true;
                if (rg != null)
                {
                    rg.StopRedGreen();
                }

                // Reset the flip state once grounded
                canFlip = true; // Allow the player to flip again after landing
                hasJumped = false; // Reset the jump state, ensuring another jump is required to flip
            }
        }
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

    protected void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carousel"))
        {
            moveSpeed = 2.5f;
            sprintSpeed = 4f;

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

            if (resetSpeedCoroutine != null)
            {
                StopCoroutine(resetSpeedCoroutine);
                resetSpeedCoroutine = null;
            }
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Carousel"))
        {
            resetSpeedCoroutine = StartCoroutine(ResetSpeedAfterDelay());
        }
        else if (collision.gameObject.CompareTag("RedGreen"))
        {
            resetSpeedCoroutine = StartCoroutine(ResetSpeedAfterDelay());
        }
    }

    private IEnumerator ResetSpeedAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        moveSpeed = 3.5f;
        sprintSpeed = 5.5f;

        resetSpeedCoroutine = null;
    }

    // Abstract methods for tricks and special moves
    
    protected void PerformGrind()
    {
        if (skateboardD != null)
        {
            if (isGrinding)  // While grinding, rotate relative to the target object's forward direction
            {
                // Get the target object's forward direction
                Vector3 targetForward = CharacterModel.forward;

                // Normalize to make sure the direction is uniform
                targetForward.y = 0; // Prevent vertical tilt (we only want horizontal rotation)
                targetForward.Normalize();

                // Rotate the skateboard model 90 degrees relative to the target object's forward direction
                skateboardD.rotation = Quaternion.LookRotation(targetForward) * Quaternion.Euler(0, 90, 0);
            }
            else  // When LeftControl is released, rotate back by 90 degrees from its current direction
            {
                // Get the current forward direction of the skateboard model
                Vector3 currentForward = skateboardD.forward;

                // Rotate back by 90 degrees relative to the current facing direction
                skateboardD.rotation = Quaternion.LookRotation(currentForward) * Quaternion.Euler(0, -90, 0);
            }
        }
    }

    protected abstract void PerformSpecialMove();

    protected virtual void Update()
    {
        HandleMovement();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isGrinding = true;
            PerformGrind();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isGrinding = false;
            PerformGrind(); // Reset to original rotation when LeftControl is released
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadPlayerData();
        }
    }
}
