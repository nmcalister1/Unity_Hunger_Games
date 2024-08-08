using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace RPG.Character
{
    public class PlayerUI : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI usernameText;
        

        public void SetPlayer(Photon.Realtime.Player player)
        {
            usernameText.text = player.NickName;
            //healthText.text = "100";
        }

        // [PunRPC]
        // public void UpdateHealth(float health)
        // {
        //     //healthText.text = health.ToString();
            
        //     HealthBarFillScript healthBar = GetComponentInChildren<HealthBarFillScript>();
        //     if (healthBar != null)
        //     {
        //         PhotonView photonView = healthBar.GetComponent<PhotonView>();
        //         if (photonView != null)
        //         {
        //             photonView.RPC("UpdateHealth", RpcTarget.All, health);
        //         }
        //         else
        //         {
        //             Debug.LogError("PhotonView is missing on HealthBarFillScript GameObject.");
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError("HealthBarFillScript is missing in PlayerUI.");
        //     }

        //     // Check health and enable the "Dead" text if health is 0 or below
        //     if (deadText != null)
        //     {
        //         deadText.gameObject.SetActive(health <= 0);
        //     }
        // }
    }

}
