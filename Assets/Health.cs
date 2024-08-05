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

        void Awake()
        {
            currentHealth = maxHealth;
            playerUIManager = GetComponent<PlayerUIManager>();
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
    }
}
