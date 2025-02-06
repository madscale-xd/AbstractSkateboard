using UnityEngine;
using UnityEngine.SceneManagement;  // To load the menu scene
using UnityEngine.UI;              // For UI button functionality

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;  // Reference to the Pause Menu Canvas (assign in inspector)
    [SerializeField] private Button resumeButton;         // Reference to the Resume Button (assign in inspector)
    [SerializeField] private Button menuButton;           // Reference to the Menu Button (assign in inspector)

    private PlayerMovement playerMovement;
    private CameraFollow cameraFollow;

    private bool isPaused = false;

    void Start()
    {
        // Initially, set the pause menu canvas to inactive
        pauseMenuCanvas.SetActive(false);

        // Set up button listeners
        resumeButton.onClick.AddListener(ResumeGame);
        menuButton.onClick.AddListener(LoadMenu);
        
        // Hide the cursor initially
        Cursor.visible = false;
    }

    void Update()
    {
        playerMovement = GameObject.Find("skateboard(Clone)").GetComponent<PlayerMovement>();
        cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();

        // Listen for Escape key to toggle pause state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();  // If paused, resume the game
            }
            else
            {
                PauseGame();   // If not paused, pause the game
            }
        }
    }

    void PauseGame()
    {
        // Activate the pause menu canvas and make it visible
        pauseMenuCanvas.SetActive(true);

        // Pause the game
        Time.timeScale = 0;

        // Disable PlayerMovement and CameraFollow
        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraFollow != null) cameraFollow.enabled = false;

        // Show the cursor
        Cursor.visible = true;

        // Set isPaused to true
        isPaused = true;
    }

    void ResumeGame()
    {
        // Deactivate the pause menu canvas
        pauseMenuCanvas.SetActive(false);

        // Resume the game
        Time.timeScale = 1;

        // Enable PlayerMovement and CameraFollow
        if (playerMovement != null) playerMovement.enabled = true;
        if (cameraFollow != null) cameraFollow.enabled = true;

        // Hide the cursor
        Cursor.visible = false;

        // Set isPaused to false
        isPaused = false;
    }

    void LoadMenu()
    {
        // Resume the game time before loading the scene
        Time.timeScale = 1;

        // Load the menu scene
        SceneManager.LoadScene("MainMenu");  // Replace "MainMenu" with your actual menu scene name
    }
}
