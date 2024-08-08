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

        // // Calculate the spawn position above ground level
        // Vector3 spawnPosition = Vector3.zero; // Default spawn position (modify as needed)
        // float spawnHeightAboveGround = 2.0f; // Adjust this value to set the spawn height above ground

        // // Perform a raycast to find the ground level if needed
        // RaycastHit hit;
        // if (Physics.Raycast(Vector3.zero, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        // {
        //     spawnPosition = hit.point + Vector3.up * spawnHeightAboveGround;
        // }
        // else
        // {
        //     spawnPosition = Vector3.up * spawnHeightAboveGround; // Default to a fixed height above the origin
        // }

        // PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, Quaternion.identity);

        // Get all spawn points with the tag "PlayerSpawnPoint"
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");

        // Check if there are available spawn points
        if (spawnPoints.Length > 0)
        {
            // Choose a random spawn point
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

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
