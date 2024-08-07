using UnityEngine;
using Photon.Pun;
using System.Collections;



public class LaserProjectile : MonoBehaviourPun
{
   public float speed = 150f;
   public int damage = 35;




   private Rigidbody rb;




   private void Awake()
   {
       rb = GetComponent<Rigidbody>();
   }




   [PunRPC]
   public void RPC_SetVelocity(Vector3 initialVelocity)
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
        if (other.CompareTag("Player"))
        {
            PhotonView targetPV = other.GetComponent<PhotonView>();
            if (targetPV != null && !targetPV.IsMine)
            {
                targetPV.RPC("TakeDamage", RpcTarget.All, damage);
            }

            Debug.Log($"OnTriggerEnter called by: {PhotonNetwork.NickName}, IsMine: {photonView.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");

            // Check if the current client owns the projectile or is the MasterClient
            if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                // Transfer ownership to the MasterClient if it's not already the owner
                if (!photonView.IsMine && PhotonNetwork.IsMasterClient)
                {
                    photonView.TransferOwnership(PhotonNetwork.MasterClient);
                    StartCoroutine(DestroyAfterOwnershipTransfer());
                }
                else
                {
                    Debug.Log($"Destroying projectile by: {PhotonNetwork.NickName}, IsMine: {photonView.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    private IEnumerator DestroyAfterOwnershipTransfer()
    {
        // Wait for a short period to ensure the ownership transfer is processed
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"Destroying projectile after ownership transfer by: {PhotonNetwork.NickName}, IsMine: {photonView.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
        PhotonNetwork.Destroy(gameObject);
    }
}

