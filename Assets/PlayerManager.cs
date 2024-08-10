using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using RPG.Character;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    private CountdownTimer countdownTimer;
    private bool canMove = false;
    private static List<GameObject> availableSpawnPoints = new List<GameObject>();

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
        // Check if the available spawn points list is empty, if so, populate it
        if (availableSpawnPoints.Count == 0)
        {
            availableSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpawnPoint"));
        }

        // Check if there are available spawn points
        if (availableSpawnPoints.Count > 0)
        {
            // Choose a random spawn point
            GameObject spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];

            // Remove the selected spawn point from the available list
            availableSpawnPoints.Remove(spawnPoint);

            // Get the spawn position
            Vector3 spawnPosition = spawnPoint.transform.position;

            // Adjust the spawn position to be above the ground if needed
            float spawnHeightAboveGround = 2.0f; // Adjust this value to set the spawn height above ground
            spawnPosition += Vector3.up * spawnHeightAboveGround;

            // Find the "FaceTile" cube
            GameObject faceTile = GameObject.FindGameObjectWithTag("FaceTile");
            Quaternion targetRotation = Quaternion.identity;
            if (faceTile != null)
            {
                // Calculate the direction to face the "FaceTile" cube
                Vector3 directionToFace = faceTile.transform.position - spawnPosition;
                directionToFace.y = 0; // Keep only the horizontal direction
                targetRotation = Quaternion.LookRotation(directionToFace);
            }

            // Instantiate the player at the chosen spawn point with the target rotation
            GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, targetRotation);

            // Enable the CountdownTimer script and disable movement initially
            CountdownTimer countdownTimer = player.GetComponent<CountdownTimer>();
            countdownTimer.enabled = true;
            StartCoroutine(DisableMovementDuringCountdown(player, countdownTimer));
        }
        else
        {
            // Default spawn position if no spawn points are found
            Vector3 spawnPosition = Vector3.up * 2.0f; // Default to a fixed height above the origin

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator DisableMovementDuringCountdown(GameObject player, CountdownTimer countdownTimer)
    {
        // Get the PlayerController component and disable movement
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            yield return new WaitUntil(() => countdownTimer.IsCountdownFinished());
            playerController.EnableMovement();
        }
    }
}

