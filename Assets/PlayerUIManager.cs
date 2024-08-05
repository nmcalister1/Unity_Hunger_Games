using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

namespace RPG.Character
{
    public class PlayerUIManager : MonoBehaviourPunCallbacks
    {
        public GameObject healthPanelPrefab;
        public Transform playerListContainer;
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
            GameObject uiInstance = Instantiate(healthPanelPrefab, playerListContainer);
            //GameObject uiInstance = PhotonNetwork.Instantiate(healthPanelPrefab.name, playerListContainer.position, Quaternion.identity);
            Debug.Log("UI Instance: " + uiInstance);
            PlayerUI playerUI = uiInstance.GetComponent<PlayerUI>();
            playerUI.SetPlayer(player);
            playerUIs[player.ActorNumber] = playerUI;
        }

        public void UpdatePlayerHealth(int playerID, int health)
        {
            if (playerUIs.TryGetValue(playerID, out PlayerUI playerUI))
            {
                Debug.Log("Calling update health method");
                playerUI.photonView.RPC("UpdateHealth", RpcTarget.All, health);
            }
        }

    }
}
