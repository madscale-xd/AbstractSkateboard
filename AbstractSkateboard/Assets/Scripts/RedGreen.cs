using System.Collections;
using UnityEngine;

public class RedGreen : MonoBehaviour
{
    [SerializeField] private Material normalMaterial;  // Reference to the normal material
    [SerializeField] private Material redMaterial;  // Reference to the red material
    [SerializeField] private float flashDuration = 0.3f;  // Duration of each flash
    [SerializeField] private float redDuration = 1f;  // Duration to stay red
    [SerializeField] private float normalDuration = 1f;  // Duration to stay normal

    [SerializeField] private GameObject targetObject;  // The GameObject to rotate (e.g., another object that should turn 180 degrees)

    private Renderer objectRenderer;  // Reference to the Renderer component
    private Coroutine materialCycleCoroutine;  // Reference to the coroutine
    private PlayerMovement player;
    
    // Store the original rotation and the target rotation when in red
    private Quaternion originalRotation;
    private Quaternion redRotation;

    public bool canMove = true;

    [SerializeField] private SFXManager sfx;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material = normalMaterial;  // Ensure the material is set to normal initially
        
        originalRotation = targetObject.transform.rotation;  // Store the original rotation of the target object
        redRotation = Quaternion.Euler(targetObject.transform.eulerAngles + new Vector3(0, 180, 0)); // 180 degrees on the Y-axis
    }

    void Update(){
        player = GameObject.Find("skateboard(Clone)").GetComponent<PlayerMovement>();
    }

   private void OnTriggerEnter(Collider other)
    {
        // Ensure that player is not null before accessing its groundTouch property
        if ((other.CompareTag("Player") && player != null) || (player != null && player.groundTouch == true))  
        {
            Debug.Log("RedGreen!");
            player.groundTouch = false;

            // Start the cycle only if it's not already running
            if (materialCycleCoroutine == null)
            {
                materialCycleCoroutine = StartCoroutine(MaterialCycle());
            }
        }
    }

    private IEnumerator MaterialCycle()
    {
        while (true)
        {
            // Flash red three times
            for (int i = 0; i < 3; i++)
            {
                objectRenderer.material = redMaterial;
                yield return new WaitForSeconds(flashDuration);
                objectRenderer.material = normalMaterial;
                yield return new WaitForSeconds(flashDuration);
            }

            // Stay red for a set amount of time
            sfx.PlaySFX(2);
            canMove = false;
            objectRenderer.material = redMaterial;
            targetObject.transform.rotation = redRotation;  // Rotate the target object 180 degrees on red
            yield return new WaitForSeconds(redDuration);

            // Flash red three times again
            for (int i = 0; i < 3; i++)
            {
                objectRenderer.material = redMaterial;
                yield return new WaitForSeconds(flashDuration);
                objectRenderer.material = normalMaterial;
                yield return new WaitForSeconds(flashDuration);
            }

            // Stay normal for a set amount of time
            sfx.PlaySFX(1);
            canMove = true;
            objectRenderer.material = normalMaterial;
            targetObject.transform.rotation = originalRotation;  // Ensure it's back to its original rotation
            yield return new WaitForSeconds(normalDuration);
        }
    }

    public void StopRedGreen(){
        Debug.Log("Stop RedGreen.");
        if (materialCycleCoroutine != null)
        {
            StopCoroutine(materialCycleCoroutine);
            materialCycleCoroutine = null;
            objectRenderer.material = normalMaterial;  // Reset to normal material
            targetObject.transform.rotation = originalRotation;  // Reset to the original rotation
            canMove = true;
        }
    }
}
