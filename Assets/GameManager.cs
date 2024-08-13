using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.Events;

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

    // public void StartCountdownTimer()
    // {
    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //     foreach (GameObject player in players)
    //     {
    //         PhotonView playerPhotonView = player.GetComponent<PhotonView>();
    //         if (playerPhotonView != null)
    //         {
    //             playerPhotonView.RPC("DisableMovementDuringCountdownRPC", RpcTarget.AllBuffered);
    //         }
    //     }

    // }

    // [PunRPC]
    // private void DisableMovementDuringCountdownRPC()
    // {
    //     DisableMovementDuringCountdown();
    // }

    // private void DisableMovementDuringCountdown()
    // {
    //     CountdownTimer countdownTimer = GetComponent<CountdownTimer>();
    
    //     if (countdownTimer != null)
    //     {
    //         countdownTimer.enabled = true;
    //         StartCoroutine(WaitForCountdown());
           
    //     }
    // }

    // private IEnumerator WaitForCountdown()
    // {
    //     yield return new WaitForSeconds(3f);
    //     DestroySpawnPointsRPC();

    // }

    // //[PunRPC]
    // private void DestroySpawnPointsRPC()
    // {
    //     GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
    //     foreach (GameObject spawnPoint in spawnPoints)
    //     {
    //         Destroy(spawnPoint);
    //     }
    // }

    // private new void OnEnable()
    // {
    //     OnNewSuppliesEvent.AddListener(OnNewSupplies);
    // }

    // private new void OnDisable()
    // {
    //     OnNewSuppliesEvent.RemoveListener(OnNewSupplies);
    // }

    // public void OnNewSupplies()
    // {
    //     Debug.Log("New supplies event has been triggered.");
    //     photonView.RPC("StartNewSuppliesEvent", RpcTarget.All);
    // }

    // [PunRPC]
    // private void StartNewSuppliesEvent()
    // {
    //     StartCoroutine(ShowNewSuppliesText());
    // }

    // private IEnumerator ShowNewSuppliesText()
    // {
    //     if (newSuppliesText != null)
    //     {
    //         Debug.Log("Showing new supplies text.");
    //         newSuppliesText.gameObject.SetActive(true);
    //         yield return new WaitForSeconds(2f);
    //         newSuppliesText.gameObject.SetActive(false);
    //     }
    // }
}





