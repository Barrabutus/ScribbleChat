using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Chat;
using System;


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
    public WebCommunication wwwComms;
    public string UserId;
    public string[] splitResponse;

    public string[] users;
    public string response;
    public List<LobbyUser> lobbyUsers = new List<LobbyUser>();


    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        chatManager = GameObject.Find("ChatManager").GetComponent<ChatManager>();
        //roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        wwwComms = GetComponent<WebCommunication>();
        PhotonNetwork.ConnectUsingSettings();
    }
 
    public override void OnConnectedToMaster()
    {
        wwwComms.GetAllRows();
        Debug.Log("Joined Master....Going to lobby");
        PhotonNetwork.JoinLobby();
        roomConnectionStateLabel.text = "Connected to room!";
    }
 
    public override void OnJoinedLobby()
    {
        //Debug.Log("We have joined the lobby getting list of rooms....");
        //Append a random number to a name so we can identify the user in the room scene.
        username = namePrefix+UnityEngine.Random.Range(0000,99999);
        PhotonNetwork.NickName = username;
       // Debug.Log("Welcome " + username);
        chatManager.ConnectToChatServer();
        chatManager.SetUsername(username);
        lobbyManager.SetUsernameLabels();
        PhotonNetwork.AutomaticallySyncScene = true;
        wwwComms.AddUser(PhotonNetwork.NickName, 0, 1);
        //StartCoroutine(SplitResponsefromWWW());
        StartCoroutine(GetUsersFromDb()); 
       // lobbyManager.UpdateUsers();


        //PhotonNetwork.JoinOrCreateRoom(hiddenRoom, new RoomOptions{MaxPlayers = 20, IsVisible = false}, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        
      //  roomConnectionStateLabel.text = "Disconnected from room server...";

    }

    IEnumerator GetUsersFromDb()
    {
      while(true)
      {

                users = response.Split('-');
                foreach(string user in users)
                {

                        
                        string[] details; 
                        details = user.Split(' ');

                        //Checks for no empty users.
                        if(details[0] != "")
                        {
                           
                                string playername = details[1];
                                //if the lobbyuser exists then skip them....
                                if(!isDuplicateUser(playername))
                                {
                                     LobbyUser newUser = new LobbyUser();
                                    newUser.id = Convert.ToInt16(details[0]);
                                    newUser.name = details[1];
                                    newUser.state = Convert.ToInt16(details[2]);
                                    Debug.Log(newUser.name + " Does not exist in LobbyUSers...");
                                    lobbyUsers.Add(newUser);

                                }else{

                                    for(int x = 0; x < lobbyUsers.Count; x++)
                                    {
                                            
                                            if(lobbyUsers[x].id == Convert.ToInt16(details[0]))
                                            {
                                                
                                                lobbyUsers[x].state = Convert.ToInt16(details[2]);


                                            }

                                    }


                                




                                }
                    
                        }
                    //lobbyUsers.Add(newUser);
                    //yield return new WaitForSeconds(0f);

                }
                
                
                Debug.Log("SHOULD QUERY FOR UPDATES....");
                lobbyManager.UpdateUsers();
                wwwComms.GetAllRows();
                
                yield return new WaitForSeconds(5f);

      }
    }

    bool isDuplicateUser(string name)
    {
        bool isDupe;

        foreach(LobbyUser _user in lobbyUsers)
        {
            if(_user.name == name)
            {
                isDupe = true;
                return isDupe;
            }

        }


        isDupe = false;
        return isDupe;
    }
    
    

}

[System.Serializable]
public class LobbyUser{




    public int id;
    public string name;
    public int state;





}

