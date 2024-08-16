using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    public float bounceForce = 15f; // Adjust this value to control how high the player bounces
    public float speedUpForce = 10f;
    private float speedUpBounceForce = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BounceTile"))
        {
            // Apply an upward force to the player
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("SpeedUpTile"))
        {
            // Apply a forward force to the player
            // Apply a forward and upward force to the player
            Vector3 forwardDirection = transform.forward; // This assumes the forward direction is the desired launch direction
            Vector3 speedUpBounce = forwardDirection * speedUpForce + Vector3.up * speedUpBounceForce;
            rb.AddForce(speedUpBounce, ForceMode.Impulse);
        }
    }
}
