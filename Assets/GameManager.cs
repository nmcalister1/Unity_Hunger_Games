using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.Events;
using RPG.Character;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;
    public static UnityEvent OnNewSuppliesEvent = new UnityEvent();

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    //[SerializeField] private TextMeshPro winnerText; // UI Text element for winner
    //[SerializeField] private Camera mainCamera; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DecreaseHealthPeriodically());
        }
    }

    private IEnumerator DecreaseHealthPeriodically()
    {
        Debug.Log("DecreaseHealthPeriodically called.");
        while (true)
        {
            yield return new WaitForSeconds(15f);
            DecreaseAllPlayersHealth();
        }
    }

    private void DecreaseAllPlayersHealth()
    {
        // photonView.RPC("TakeDamage", RpcTarget.All, 5);
        // ChickenAtStart();
        // Find all GameObjects with the "Player" tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Debug.Log("Player: " + player.name);
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();

            if (playerPhotonView != null)
            {
                // Call the TakeDamage method on each player's PhotonView
                playerPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, 5);
            }
        }
    }

    public void CheckForWinner()
    {
        Debug.Log("CheckForWinner called.");
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     return;
        // }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int aliveCount = 0;
        GameObject lastAlivePlayer = null;

        foreach (GameObject player in players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.isAlive)
            {
                Debug.Log("Player is alive: " + player.name);
                aliveCount++;
                lastAlivePlayer = player;
            }
        }

        Debug.Log("Alive count: " + aliveCount);

        if (aliveCount == 1 && lastAlivePlayer != null)
        {
            lastAlivePlayer.GetComponent<PlayerController>().HandleWin();
            //lastAlivePlayer.GetComponent<PlayerController>().HandleRestartGame();
            // The last alive player sends an RPC to all players to load the main menu

            //CleanUp();

            photonView.RPC("LoadMainMenuForAll", RpcTarget.All);
        }

        
    }

    [PunRPC]
    private void LoadMainMenuForAll()
    {
        StartCoroutine(CleanupAndLoadMainMenu());
    }

    private IEnumerator CleanupAndLoadMainMenu()
    {
        yield return new WaitForSeconds(5f);

        // Leave the room to disconnect from the current game session
        PhotonNetwork.LeaveRoom();

        // Wait until the player has left the room
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        // Load the main menu or game level 0
        PhotonNetwork.LoadLevel(0);
    }

}





