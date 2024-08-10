using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject PlayerListItemPrefab;
    [SerializeField] private GameObject startGameButton;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // public override void OnConnectedToMaster()
    // {
    //     PhotonNetwork.JoinLobby();
    //     PhotonNetwork.AutomaticallySyncScene = true;
    // }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master. Opening 'createNickname' menu.");
        MenuManager.Instance.OpenMenu("createNickname");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // public void OnCreateNickname()
    // {
    //     MenuManager.Instance.OpenMenu("create nickname");
    //     PhotonNetwork.NickName = nicknameInputField.text;
    //     MenuManager.Instance.OpenMenu("loading");
    //     MenuManager.Instance.OpenMenu("title");
    // }

    public void OnCreateNickname()
    {
        //MenuManager.Instance.OpenMenu("create nickname");
        if (string.IsNullOrEmpty(nicknameInputField.text))
        {
            // Use a default or random nickname if the input field is empty
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        }
        else
        {
            PhotonNetwork.NickName = nicknameInputField.text;
        }
        PhotonNetwork.JoinLobby();
        Debug.Log("Nickname set. Joining lobby and opening 'loading' menu.");
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

        // Check if the nickname input field is not empty
        // if (!string.IsNullOrEmpty(nicknameInputField.text))
        // {
        //     PhotonNetwork.NickName = nicknameInputField.text;
        // }
        // else
        // {
        //     // Use a default or random nickname if the input field is empty
        //     PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
        // }
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

}
