using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class HealthBarFillScript : MonoBehaviourPunCallbacks
{
    public RectTransform healthBar;
    public float Height, Width;

    public TextMeshProUGUI deadText;

    private void Start()
    {
        Height = healthBar.sizeDelta.y;
        Width = healthBar.sizeDelta.x;
    }

    [PunRPC]
    private void RPC_UpdateHealth(float health)
    {
        Debug.Log("Updating health bar");
        Debug.Log("Health: " + health);
        healthBar.sizeDelta = new Vector2(Width * (health / 100), Height);

        if (deadText != null)
        {
            deadText.gameObject.SetActive(health <= 0);
        }
    }

    public void UpdateHealth(float health)
    {
        photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health);
    }


}

