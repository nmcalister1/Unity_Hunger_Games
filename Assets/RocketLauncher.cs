using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using RPG.Character;

public class RocketLauncher : MonoBehaviourPun
{
    public float speed = 150f;
    public int damage = 30;
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

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         PhotonView targetPV = other.GetComponent<PhotonView>();
    //         if (targetPV != null && !targetPV.IsMine)
    //         {
    //             targetPV.RPC("TakeDamage", RpcTarget.All, damage);
    //             PhotonNetwork.Destroy(gameObject);
    //         }

    //         Debug.Log($"OnTriggerEnter called by: {PhotonNetwork.NickName}, IsMine: {photonView.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");

    //     }
    // }

    // private IEnumerator DestroyAfterOwnershipTransfer()
    // {
    //     // Wait for a short period to ensure the ownership transfer is processed
    //     yield return new WaitForSeconds(0.1f);

    //     Debug.Log($"Destroying projectile after ownership transfer by: {PhotonNetwork.NickName}, IsMine: {photonView.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
    //     PhotonNetwork.Destroy(gameObject);
    // }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        
        if (other.CompareTag("Player") || other.CompareTag("GroundTile"))
        {
            // PhotonView targetPV = other.GetComponent<PhotonView>();
            // if (targetPV != null && !targetPV.IsMine)
            // {
            //     targetPV.RPC("TakeDamage", RpcTarget.All, damage);
            // }

            Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(damageRadius, damageRadius, damageRadius), Quaternion.identity);
            HashSet<PlayerController> damagedPlayers = new HashSet<PlayerController>();

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    //Debug.Log("Player being dealt damage from rocket launcher");
                    // PhotonView targetPV = collider.GetComponent<PhotonView>();
                    // //Health playerHealth = collider.GetComponent<Health>();
                    // //Debug.Log(targetPV);
                    // if (targetPV != null && !targetPV.IsMine && !damagedPlayers.Contains(targetPV))
                    // {
                    //     targetPV.RPC("TakeDamage", RpcTarget.All, damage);
                    //     targetPV.RPC("RPC_TriggerExplosion", RpcTarget.All, transform.position);
                    //     damagedPlayers.Add(targetPV);
                    // }
                    PlayerController targetPV = collider.GetComponent<PlayerController>();
                    //Health playerHealth = collider.GetComponent<Health>();
                    //Debug.Log(targetPV);
                    if (targetPV != null && !damagedPlayers.Contains(targetPV))
                    {
                        targetPV.TakeDamageWithoutRPC(damage);
                        targetPV.photonView.RPC("RPC_TriggerExplosion", RpcTarget.All, transform.position);
                        damagedPlayers.Add(targetPV);
                    }

                    // if (playerHealth != null)
                    // {
                    //     playerHealth.currentHealth -= damage;
                    // }
                }
            }

            // Trigger the explosion effect
            //photonView.RPC("RPC_TriggerExplosion", RpcTarget.All, transform.position);

            // Destroy the projectile
            //PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void RPC_TriggerExplosion(Vector3 position)
    {
        // Instantiate the explosion effect at the given position
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);

        // Apply area damage to players within the radius
        //ApplyAreaDamage(position);

        // Optionally, destroy the explosion effect after a short delay
        Destroy(explosion, 0.5f);
    }

    // private void ApplyAreaDamage(Vector3 explosionPosition)
    // {
    //     Collider[] colliders = Physics.OverlapBox(explosionPosition, new Vector3(damageRadius, damageRadius, damageRadius), Quaternion.identity);
    //     HashSet<PhotonView> damagedPlayers = new HashSet<PhotonView>();

    //     foreach (Collider collider in colliders)
    //     {
    //         if (collider.CompareTag("Player"))
    //         {
    //             //Debug.Log("Player being dealt damage from rocket launcher");
    //             PhotonView targetPV = collider.GetComponent<PhotonView>();
    //             //Health playerHealth = collider.GetComponent<Health>();
    //             //Debug.Log(targetPV);
    //             if (targetPV != null && !targetPV.IsMine && !damagedPlayers.Contains(targetPV))
    //             {
    //                 targetPV.RPC("TakeDamage", RpcTarget.All, damage);
    //                 damagedPlayers.Add(targetPV);
    //             }

    //             // if (playerHealth != null)
    //             // {
    //             //     playerHealth.currentHealth -= damage;
    //             // }
    //         }
    //     }
    // }
}


