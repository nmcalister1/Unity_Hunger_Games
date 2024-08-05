using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Character
{
    public class HealthBarUI : MonoBehaviour
    {
        public float Health, MaxHealth, Width, Height;

        [SerializeField] private RectTransform healthBar;

        public void SetMaxHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
        }


        public void SetHealth(float health)
        {
            Health = health;
            healthBar.sizeDelta = new Vector2(Width * (Health / MaxHealth), Height);
        }

    }

}

