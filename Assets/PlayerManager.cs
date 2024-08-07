using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using RPG.Character;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);

        // Calculate the spawn position above ground level
        Vector3 spawnPosition = Vector3.zero; // Default spawn position (modify as needed)
        float spawnHeightAboveGround = 2.0f; // Adjust this value to set the spawn height above ground

        // Perform a raycast to find the ground level if needed
        RaycastHit hit;
        if (Physics.Raycast(Vector3.zero, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            spawnPosition = hit.point + Vector3.up * spawnHeightAboveGround;
        }
        else
        {
            spawnPosition = Vector3.up * spawnHeightAboveGround; // Default to a fixed height above the origin
        }

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, Quaternion.identity);

        // Set the TagObject to the Health component for the player
        // Health healthComponent = playerController.GetComponent<Health>();
        // if (healthComponent == null)
        // {
        //     Debug.LogError("Health component is missing on PlayerController prefab.");
        // }
        // else
        // {
        //     Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;
        //     player.TagObject = healthComponent;
        //     Debug.Log("Health component set for player: " + player.NickName);
        // }
    }
}
