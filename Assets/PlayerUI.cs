using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace RPG.Character
{
    public class PlayerUI : MonoBehaviourPunCallbacks
    {
        public RectTransform healthBar;
        public float Height, Width;
        public TextMeshProUGUI usernameText;

        public void SetPlayer(Player player)
        {
            usernameText.text = player.NickName;
        }

        [PunRPC]
        public void UpdateHealth(int health)
        {
            Debug.Log("Updating health bar");
            Debug.Log("Health: " + health);
            healthBar.sizeDelta = new Vector2(Width * (health / 100), Height);
        }
    }

}
