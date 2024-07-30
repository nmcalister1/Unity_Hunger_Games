using UnityEngine;
using Photon.Pun;

public class RocketLauncher : MonoBehaviourPun
{
    public float speed = 150f;
    public int damage = 35;
    public float damageRadius = 4f;
    public GameObject explosionPrefab; // Add this to reference the explosion effect

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void RPC_SetVelocityRocketProjectile(Vector3 initialVelocity)
    {
        if (rb != null)
        {
            rb.velocity = initialVelocity;
        }
    }

    public void Initialize(Vector3 shootDirection)
    {
        rb.velocity = shootDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("GroundTile"))
        {
            // PhotonView targetPV = other.GetComponent<PhotonView>();
            // if (targetPV != null && !targetPV.IsMine)
            // {
            //     targetPV.RPC("TakeDamage", RpcTarget.All, damage);
            // }

            // Trigger the explosion effect
            photonView.RPC("RPC_TriggerExplosion", RpcTarget.All, transform.position);

            // Destroy the projectile
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void RPC_TriggerExplosion(Vector3 position)
    {
        // Instantiate the explosion effect at the given position
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);

        // Apply area damage to players within the radius
        ApplyAreaDamage(position);

        // Optionally, destroy the explosion effect after a short delay
        Destroy(explosion, 0.5f);
    }

    private void ApplyAreaDamage(Vector3 explosionPosition)
    {
        Collider[] colliders = Physics.OverlapBox(explosionPosition, new Vector3(damageRadius, damageRadius, damageRadius), Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PhotonView targetPV = collider.GetComponent<PhotonView>();
                if (targetPV != null && !targetPV.IsMine)
                {
                    targetPV.RPC("TakeDamage", RpcTarget.All, damage);
                }
            }
        }
    }
}


