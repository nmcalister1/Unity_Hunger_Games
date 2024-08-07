using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthBarFillScript : MonoBehaviour
{
    public RectTransform healthBar;
    public float Height, Width;

    [PunRPC]
    public void UpdateHealth(float health)
    {
        Debug.Log("Updating health bar");
        Debug.Log("Health: " + health);
        healthBar.sizeDelta = new Vector2(Width * (health / 100), Height);
    }
}

