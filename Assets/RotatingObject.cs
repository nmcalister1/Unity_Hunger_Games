using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float rotationSpeed = 90f; // Degrees per second

    void Update()
    {
        // Rotate the cube around its local x-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

