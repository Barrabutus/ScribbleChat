using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

//Class for managing the lobby.
public class _LobbyManager : MonoBehaviourPunCallbacks
{

    //How often do we want to update the users list?
    public float UserUpdateThreadInterval;
    public List<_User> users= new List<_User>();
    public SQLManager sqlmanager;
    public _ChatManager chatManager;

    public _Manager manager;
    public GameObject userListing;
    public Transform userListingPanel;
    public List<_User> currentUsers = new List<_User>();
    public bool isUserUpdateThreadRunning;

    public InputField chatBox;
    public InputField roomName;

    public List<string> currentRoomList = new List<string>();

    public GameObject roomlistingPrefab;    
    public Transform roomlistingsPanel;

   public List<RoomInfo> _info;

   public Text chatconnection;
   public Text netwkconnection;




    // Start is called before the first frame update
    void Start()
    {
        // Gaining access to the manager allows us to access all other areas of the game 
        //We want access to the SQLManager as this will have a list of all the current connected users...
        sqlmanager = GameObject.Find("SQLManager").GetComponent<SQLManager>();
        chatManager = GameObject.Find("Manager").GetComponent<_ChatManager>();
        manager = GameObject.Find("Manager").GetComponent<_Manager>();
        sqlmanager.StartUserThread();
        sqlmanager.GetRows();

        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if((sqlmanager.GetUsersList().Count > 0) && (isUserUpdateThreadRunning == false))
        {
            isUserUpdateThreadRunning = true;
            StartCoroutine(LobbyUsersUpdateThread());
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToChat();
        }

        chatconnection.text = chatManager.isChatConnectionStateString();
        netwkconnection.text = GameObject.Find("Manager").GetComponent<_ConnectionManager>().isConnectionStateString();

     


    }



    IEnumerator LobbyUsersUpdateThread()
    {
        while(true)
        {
          //Get the users from the list.
          users = sqlmanager.GetUsersList();
          //Populate the list with the updated list.
          currentUsers = users;

          foreach(_User user in currentUsers)
          {
            if(!GameObject.Find(user.username + user.id.ToString()))
            {
                //Is the user in a room or the lobby ??
                //Create button for this user.
                //ADD CHECK IF THEY DO NOT ALREADY HAVE A LISTING>>>>>
                GameObject userPrefab = Instantiate(userListing, new Vector3(0,0,0), Quaternion.identity);
                Text userTxt = userPrefab.GetComponentInChildren<Text>();
                userTxt.text = user.username;
                userPrefab.name = user.username + user.id.ToString();
                
                Button userBtn = userPrefab.GetComponent<Button>();
                if(user.isInLobby == 1)
                            {
                            userBtn.interactable = true;
                            }else{
                            userBtn.interactable = false;
                            }

                userPrefab.transform.SetParent(userListingPanel.transform);

                //currentUsers.Add(user);

            }else{
                //Debug.Log(user.username + " exists in the users list already skipped...");

                //User already has a presence in the list. 

                 //User has a button already....
                GameObject userBtn = GameObject.Find(user.username + user.id.ToString());
                //Debug.Log("Searching for user button" +user.username + user.id.ToString() + "User lobbystate is " + user.isInLobby);
                Button  btnObject = userBtn.GetComponent<Button>();
                if(user.isInLobby == 1)
                            {
                            btnObject.interactable = true;
                            }else{
                            btnObject.interactable = false;
                            }
            }      

          }

          yield return new WaitForSeconds(UserUpdateThreadInterval);

        }
    }

    public void SendMessageToChat()
    {
        //Use PlayerPrefs to get users name. 
        string messg = PlayerPrefs.GetString("Username") + " says " + chatBox.text;
        //string messg = manager.sessionInstance().username + " says " + chatBox.text;
        chatManager.SendMessageToChannel(_ChatChannels.Global.ToString(), messg);
        chatBox.text = "";

    }

    public override void OnCreatedRoom()
   {
       Debug.Log("Room Created...");
        ///USER LEAVES LOBBY REMOVED FROM USERS FILE:::::
        //int id = manager.sessionInstance().id;
        //sqlmanager.UpdateUserStatus(id,0);
        //PhotonNetwork.CreateRoom(roomName.text, new RoomOptions{MaxPlayers = 4, EmptyRoomTtl = 3000});
        PhotonNetwork.LoadLevel("GameScene");  
        //PhotonNetwork.JoinRoom(roomName.text);                


   }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Did the OnRoomListUpdate actually occur??
        Debug.Log("FIRED");
        List<RoomInfo> _info = new List<RoomInfo>();
        _info = roomList;
        foreach(RoomInfo info in _info)
        {
            //If the room does not exist we create a button for it.
            if(!currentRoomList.Contains(info.Name))
            {
                CreateNewRoomListItem(info);
               Debug.Log("Creating new room item");
            }else if(currentRoomList.Contains(info.Name))
            {
                UpdateRoomListItem(info);
            }         
        }
    }

    public override void OnJoinedLobby()
    {


        if(chatManager.getChatConnectionState())
        {
            chatManager.SendMessageToChannel(_ChatChannels.Global.ToString(), PlayerPrefs.GetString("Username") + " has joined the lobby..");
        }


    }

    
    public void OnClickCreateRoom()
    {
        if(roomName.text.Length > 15)
        {
            roomName.text = "No More than 15 characters for a room name";
        }else{
                PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 4, EmptyRoomTtl = 3000});
                chatManager.SubscribeUserToChannel(roomName.text, PlayerPrefs.GetString("Username"));
                ///USER LEAVES LOBBY REMOVED FROM USERS FILE:::::
                int id = manager.sessionInstance().id;
                sqlmanager.UpdateUserStatus(id,0);  

            }
    }

    public void UpdateRoomListItem(RoomInfo info)
    {
        GameObject listing = GameObject.Find(info.Name);
        if(listing != null)
        {
            Text txt = listing.GetComponentInChildren<Text>();
            if(info.PlayerCount == info.MaxPlayers)
            {
                txt.text = "ROOM FULL";
                Button btn = listing.GetComponent<Button>();
                btn.interactable = false;

            }else{
                txt.text = info.Name + " - "+ info.PlayerCount + " / " + info.MaxPlayers;
            }
        }else{
            //Debug.Log("Cannot find a gameobject with the name of " + info.Name);
        }
    }

    public void CreateNewRoomListItem(RoomInfo info)
    {
        //Double check that the room does not exist.
        if(!currentRoomList.Contains(info.Name))
        {
            GameObject listing = Instantiate(roomlistingPrefab, roomlistingsPanel.transform);
            Text btnText = listing.GetComponentInChildren<Text>();

            listing.GetComponent<Button>().onClick.AddListener(() => { OnClickJoinRoom(info.Name); });
            btnText.text = info.Name + " - "+ info.PlayerCount + " / " + info.MaxPlayers;
            listing.transform.SetParent(roomlistingsPanel.transform);
           // Debug.Log("ROOM NAME"  + info.Name);
            listing.name = info.Name;
            currentRoomList.Add(info.Name);
        }
    }

    private void OnClickJoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnLeftLobby()
    {

        chatManager.UnsubscribeToChannel(_ChatChannels.Global.ToString(), PlayerPrefs.GetString("Username") + PlayerPrefs.GetInt("UserId"));
    }
     
}
