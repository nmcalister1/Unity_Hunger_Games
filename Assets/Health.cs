using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

namespace RPG.Character
{
    public class Health : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int maxHealth = 100;
        [NonSerialized] public int currentHealth;

        private PlayerUIManager playerUIManager;

       

        //private PlayerUIManager playerUIManager;

        void Awake()
        {
            currentHealth = maxHealth;
            playerUIManager = FindObjectOfType<PlayerUIManager>();
            if (playerUIManager == null)
            {
                Debug.LogError("PlayerUIManager not found in the scene.");
            }
            //playerUIManager = GetComponent<PlayerUIManager>();
        }

        // public void UpdateHealthUI(int Health)
        // {
        //     playerUIManager.UpdatePlayerHealth(photonView.ViewID, Health);
        // }

         private void FixedUpdate()
        {
            // if (Time.time >= nextSyncTime)
            // {
                
            //     nextSyncTime = Time.time + syncInterval;
            // }
            SyncHealthHealth();
        }

        public void SyncHealthHealth()
        {
            if (photonView.IsMine && playerUIManager != null)
            {
                //playerUIManager.SyncHealth(photonView.OwnerActorNr, (float)currentHealth);
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GameObject playerObject = GetPlayerObject(player.ActorNumber);
                    if (playerObject != null && playerObject.TryGetComponent(out Health playerHealth))
                    {
                        playerUIManager.SyncHealth(player.ActorNumber, playerHealth.currentHealth);
                    }
                    else
                    {
                        Debug.LogError("Player with actor number " + player.ActorNumber + " does not have a Health component or the player object is null.");
                    }
                }
            }
           
        }

        // Method to find the player object based on ActorNumber
        private GameObject GetPlayerObject(int actorNumber)
        {
            foreach (var player in FindObjectsOfType<PlayerController>())
            {
                if (player.photonView.OwnerActorNr == actorNumber)
                {
                    return player.gameObject;
                }
            }
            return null;
        }

        [PunRPC]
        public void HealthTakeDamage(int damage)
        {
            //if (!PV.IsMine) return;

            // Reduce the player's health
            currentHealth -= damage;

            currentHealth = Mathf.Clamp(currentHealth, 0, 100);

            //health.SyncHealthHealth();
            //photonView.RPC("UpdatePlayerHealth", RpcTarget.All, photonView.Owner.ActorNumber, (float)health.currentHealth);    

            if (currentHealth <= 0)
            {
                // Handle player death
                DieHealth();
            }
            
        }


        public void TakeDamage(int damage)
        {
            if (!photonView.IsMine) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log("Player health: " + currentHealth);

            if (currentHealth <= 0)
            {
                DieHealth();
            }
        }

        public void Heal(int amount)
        {
            if (!photonView.IsMine) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        private IEnumerator WaitAndHidePlayer()
        {
            yield return new WaitForSeconds(.1f);
            photonView.RPC("RPC_HealthHidePlayer", RpcTarget.AllBuffered);
        }

        public void DieHealth()
        {
            // You can add logic to handle what happens when the player dies
            Debug.Log("Player has died!");

            // Call RPC to hide the player for all clients
            //StartCoroutine(WaitAndHidePlayer());
            //health.SyncHealthHealth();
            StartCoroutine(WaitAndHidePlayer());
        }

        [PunRPC]
        private void RPC_HealthHidePlayer()
        {
            // Hide the player by disabling the game object
            gameObject.SetActive(false);
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        // {
        //     if (stream.IsWriting)
        //     {
        //         stream.SendNext(currentHealth);
        //     }
        //     else
        //     {
        //         currentHealth = (int)stream.ReceiveNext();
        //     }
        // }
    }
}
