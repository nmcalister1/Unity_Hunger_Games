// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Photon.Pun;
// using System.IO;
// using RPG.Character;
// using Photon.Realtime;
// using ExitGames.Client.Photon;
// using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

// public class PlayerManager : MonoBehaviourPunCallbacks
// {
//     private PhotonView PV;
//     private const float SpawnHeightAboveGround = 2.0f;
//     private const float XOffset = 7.0f;

//     void Awake()
//     {
//         PV = GetComponent<PhotonView>();
//     }

//     void Start()
//     {
//         if (PV.IsMine)
//         {
//             CreateController();
//         }
//     }

//     void CreateController()
//     {
//         Vector3 spawnPosition = transform.position;
//         spawnPosition.x += XOffset;
//         spawnPosition += Vector3.up * SpawnHeightAboveGround;

//         GameObject faceTile = GameObject.FindGameObjectWithTag("FaceTile");
//         Quaternion targetRotation = Quaternion.identity;
//         if (faceTile != null)
//         {
//             Vector3 directionToFace = faceTile.transform.position - spawnPosition;
//             directionToFace.y = 0;
//             targetRotation = Quaternion.LookRotation(directionToFace);
//         }

//         GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, targetRotation);
//         Debug.Log("Player instantiated");

//         CountdownTimer countdownTimer = player.GetComponent<CountdownTimer>();
//         if (countdownTimer != null)
//         {
//             countdownTimer.enabled = true;
//             StartCoroutine(DisableMovementDuringCountdown(player, countdownTimer));
//         }
//     }

//     IEnumerator DisableMovementDuringCountdown(GameObject player, CountdownTimer countdownTimer)
//     {
//         Debug.Log("Disabling movement during countdown");
//         PlayerController playerController = player.GetComponent<PlayerController>();
//         if (playerController != null)
//         {
//             yield return new WaitUntil(() => countdownTimer.IsCountdownFinished());
//             playerController.EnableMovement();
//         }
//     }
// }





using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using RPG.Character;
using System.IO;
using TMPro;

namespace RPG.Character
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        private PhotonView PV;
        private const int CountdownTime = 5; // Countdown in seconds
        private const string GameStartKey = "GameStarted";
        private const float SpawnHeightAboveGround = 2.0f;
        private const float XOffset = 7.0f;

        // Define a list of possible spawn positions
        private List<Vector3> spawnPositions = new List<Vector3>
        {
            new Vector3(7, 0, 10),
            new Vector3(7, 0, 5),
            new Vector3(7, 0, -3),
            new Vector3(0, 0, -3),
            new Vector3(-6, 0, -3),
            new Vector3(-6, 0, 4),
            new Vector3(-6, 0, 11),
            new Vector3(0, 0, 11),
            // Add more pairs as needed
        };

        void Awake()
        {
            PV = GetComponent<PhotonView>();
        }

        void Start()
        {
            if (PV.IsMine)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(StartGameCountdown());
                }
            }
        }

        private IEnumerator StartGameCountdown()
        {
            Debug.Log("Starting game countdown...");

            // Start a countdown
            yield return new WaitForSeconds(CountdownTime);

            // Call the RPC to create player controllers
            PV.RPC("CreateController", RpcTarget.All);
        }

        [PunRPC]
        void CreateController()
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            //Vector3 spawnPosition = transform.position;
            spawnPosition += Vector3.up * SpawnHeightAboveGround;

            GameObject faceTile = GameObject.FindGameObjectWithTag("FaceTile");
            Quaternion targetRotation = Quaternion.identity;
            if (faceTile != null)
            {
                Vector3 directionToFace = faceTile.transform.position - spawnPosition;
                directionToFace.y = 0;
                targetRotation = Quaternion.LookRotation(directionToFace);
            }

            // wait for event to fire from master client before instantiating player

            GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, targetRotation);
            Debug.Log("Player instantiated");

        }

        private Vector3 GetRandomSpawnPosition()
        {
            int randomIndex = Random.Range(0, spawnPositions.Count); // Get a random index
            return spawnPositions[randomIndex]; // Return the spawn position at that index
        }
    }
}






