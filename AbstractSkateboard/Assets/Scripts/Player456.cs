using UnityEngine;
using System.Collections;

public class Player456 : AbstractSkateboard
{
    public Transform WholeCharacter; // Reference to the whole character (used for flipping)

    [SerializeField]
    private float flipSpeed = 720f;  // Speed of the 360 flip in degrees per second

    void LateUpdate()
    {
        // Check if 'R' is pressed to perform a special move (360 flip + jump)
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Only allow a flip if the player has jumped and is grounded (to ensure they flip after jumping)
            if (hasJumped && !isGrounded && canFlip)
            {
                PerformSpecialMove();
                canFlip = false; // Prevent further flips until the player lands
            }
        }
    }

    // Perform a special move: 360 flip in the X direction + jump
    protected override void PerformSpecialMove()
    {
        if (WholeCharacter != null)
        {
            // Start a coroutine to smoothly rotate the player model 360 degrees around the X-axis and apply a jump
            StartCoroutine(Flip360AndJump());
        }
    }

    private IEnumerator Flip360AndJump()
    {
        // Apply the upward jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        float rotationAmount = 0f;

        // Rotate the player model in the X direction while jumping
        while (rotationAmount < 360f)
        {
            // Rotate the player model in the X direction
            WholeCharacter.Rotate(Vector3.right * flipSpeed * Time.deltaTime);
            rotationAmount += flipSpeed * Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final rotation is exactly 360 degrees
        WholeCharacter.rotation = Quaternion.Euler(WholeCharacter.rotation.eulerAngles.x + 360f, WholeCharacter.rotation.eulerAngles.y, WholeCharacter.rotation.eulerAngles.z);
    }
}
