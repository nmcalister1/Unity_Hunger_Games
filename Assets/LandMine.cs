using UnityEngine;
using Photon.Pun;
using RPG.Character;

public class LandMine : MonoBehaviourPun
{
    public int damage = 40;
    public GameObject explosionEffectPrefab; // Assign the explosion effect prefab in the inspector

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name); // Debug statement

        if (other.CompareTag("Player"))
        {
            //gameObject.SetActive(false);
            Debug.Log("Player entered trigger: " + other.gameObject.name); // Debug statement

            PhotonView targetPV = other.GetComponent<PhotonView>();
            if (targetPV != null)
            {
                Debug.Log("Player is not mine, triggering explosion and damage"); // Debug statement

                // Deal damage to the player who stepped on the mine
                targetPV.RPC("TakeDamage", RpcTarget.All, damage);

                // Show explosion effect
                photonView.RPC("RPC_ShowExplosionEffect", RpcTarget.All, transform.position);

                Debug.Log("Destroying land mine"); // Debug statement
                // Destroy the land mine
                // Destroy the land mine
                photonView.RPC("RPC_DestroyLandMine", RpcTarget.All);
            }

            // PlayerController targetPV = other.GetComponent<PlayerController>();
            // if (targetPV != null)
            // {
            //     targetPV.TakeDamageWithoutRPC(damage);
            //     //PhotonNetwork.Destroy(gameObject);
            //     targetPV.photonView.RPC("RPC_ShowExplosionEffect", RpcTarget.All, transform.position);
            // }
        }
    }

    [PunRPC]
    private void RPC_ShowExplosionEffect(Vector3 position)
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        Destroy(explosionEffect, 0.5f); // Remove the effect after 0.5 seconds
    }

    [PunRPC]
    private void RPC_DestroyLandMine()
    {
        if (PhotonNetwork.IsMasterClient || photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    
        //PhotonNetwork.Destroy(gameObject);
        
    }
}
