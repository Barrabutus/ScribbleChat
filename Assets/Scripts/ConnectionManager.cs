using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Chat;


public class ConnectionManager : MonoBehaviourPunCallbacks
{
    
    public string namePrefix;
    
    [Header("Script References")]
    public ChatManager chatManager;
    public RoomManager roomManager;
    public LobbyManager lobbyManager; 
    public ConnectionManager instance;
    public Text roomConnectionStateLabel;
    private string username;

    //TEST ROOM
    public string hiddenRoom = "HIDDEN";

 
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        chatManager = GameObject.Find("ChatManager").GetComponent<ChatManager>();
        //roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        PhotonNetwork.ConnectUsingSettings();
    }
 
    public override void OnConnectedToMaster()
    {
        Debug.Log("Joined Master....Going to lobby");
        PhotonNetwork.JoinLobby();
        roomConnectionStateLabel.text = "Connected to room!";
    }
 
    public override void OnJoinedLobby()
    {
        Debug.Log("We have joined the lobby getting list of rooms....");
        //Append a random number to a name so we can identify the user in the room scene.
        username = namePrefix+Random.Range(0000,99999);
        PhotonNetwork.NickName = username;
        Debug.Log("Welcome " + username);
        chatManager.ConnectToChatServer();
        chatManager.SetUsername(username);
        lobbyManager.SetUsernameLabels();
        PhotonNetwork.AutomaticallySyncScene = true;
       
        instance = this;

        //PhotonNetwork.JoinOrCreateRoom(hiddenRoom, new RoomOptions{MaxPlayers = 20, IsVisible = false}, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        
      //  roomConnectionStateLabel.text = "Disconnected from room server...";

    }

    

}
