using UnityEngine;

public class CarouselRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up;  // Axis to rotate around, default is the Y-axis
    [SerializeField] private float rotationSpeed = 20f;  // Speed of rotation in degrees per second

    // Update is called once per frame
    void Update()
    {
        // Rotate the GameObject around the specified axis at the specified speed
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
