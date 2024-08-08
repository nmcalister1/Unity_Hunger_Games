using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using RPG.Character;

public class PlayerUIManager : MonoBehaviourPunCallbacks
{
    public GameObject healthPanel;
    private Dictionary<int, PlayerUI> playerUIs = new Dictionary<int, PlayerUI>();

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                CreatePlayerUI(player);
            }
        }
    }

    private void CreatePlayerUI(Photon.Realtime.Player player)
    {
        // Find all HealthGroups within the health panel
        PlayerUI[] healthGroups = healthPanel.GetComponentsInChildren<PlayerUI>(true);

        // Counter for the number of active health groups
        int activeHealthGroups = 0;

        // Find the next available HealthGroup
        foreach (var healthGroup in healthGroups)
        {
            if (healthGroup.gameObject.activeSelf)
            {
                activeHealthGroups++;
            }
            else if (!healthGroup.gameObject.activeSelf && activeHealthGroups < PhotonNetwork.PlayerList.Length)
            {
                healthGroup.gameObject.SetActive(true);
                healthGroup.SetPlayer(player);
                playerUIs[player.ActorNumber] = healthGroup;
                activeHealthGroups++;
                break;
            }
        }
    }

    [PunRPC]
    public void UpdatePlayerHealth(int playerID, float health)
    {
        if (playerUIs.TryGetValue(playerID, out PlayerUI playerUI))
        {
            if (playerUI == null)
            {
                Debug.Log("PlayerUI is null, skipping update.");
                return;
            }
            HealthBarFillScript healthBar = playerUI.GetComponentInChildren<HealthBarFillScript>();
            healthBar.UpdateHealth(health);
            //healthBar.photonView.RPC("RPC_UpdateHealth", RpcTarget.All, health);
        }
        else 
        {
            Debug.Log("No player UI value");
        }
    } 

    // Call this method to update health across the network
    public void SyncHealth(int playerID, float health)
    {
        photonView.RPC("UpdatePlayerHealth", RpcTarget.All, playerID, health);
    }

    
}

