using UnityEngine;
using Photon.Pun;




public class RocketLauncher : MonoBehaviourPun
{
   public float speed = 150f;
   public int damage = 35;




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
       if (other.CompareTag("Player"))
       {
           PhotonView targetPV = other.GetComponent<PhotonView>();
           if (targetPV != null && !targetPV.IsMine)
           {
               targetPV.RPC("TakeDamage", RpcTarget.All, damage);
           }
           PhotonNetwork.Destroy(gameObject); // Destroy the projectile after hitting a player
       }
   }
}

