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

        private float syncInterval = 0.2f;
        private float nextSyncTime = 0f;

       

        //private PlayerUIManager playerUIManager;

        void Awake()
        {
            currentHealth = maxHealth;
            //playerUIManager = GetComponent<PlayerUIManager>();
        }

        // public void UpdateHealthUI(int Health)
        // {
        //     playerUIManager.UpdatePlayerHealth(photonView.ViewID, Health);
        // }

         private void Update()
        {
            if (Time.time >= nextSyncTime)
            {
                SyncHealthHealth();
                nextSyncTime = Time.time + syncInterval;
            }
        }

        public void SyncHealthHealth()
        {
            if (photonView.IsMine)
            {
                PlayerUIManager playerUIManager = FindObjectOfType<PlayerUIManager>();
                if (playerUIManager != null)
                {
                    playerUIManager.SyncHealth(photonView.OwnerActorNr, currentHealth);
                }
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
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (!photonView.IsMine) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        private void Die()
        {
            // Add logic for what happens when the player dies
            Debug.Log("Player died!");
            // For example, you might want to respawn the player
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
