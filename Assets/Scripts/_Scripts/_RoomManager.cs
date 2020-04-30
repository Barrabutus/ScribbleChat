using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;

public class _RoomManager : MonoBehaviourPunCallbacks
{
    //Access to the chat manager.
    /*USE : Handles all the guesses and chat from the gameplayers.*/


    public _ChatManager chatManager;
    //Access to the sqlManager 
    /* USE : To update the users state if they leave the room we need to tell the SQL server that they have left and update their state.*/
    public SQLManager sqlManager;
    public List<_User> currentRoomPlayers = new List<_User>();
    public List<int> currentActorsID = new List<int>();
    public float PlayerUpdateThreadInterval;
    private int numPlayers;
    public GameObject userPrefab;
    public Transform userPanel;
    public _Manager manager;
    public _ConnectionManager connectionManager;
    public _RoomManager instance;
    public Text roomnamelabel;
    public Text netwkconnection;
    public Text chatconnection;
    public RoomInfo roomInfo;
    public bool userListUpdate;

    [Header("Waiting Room Variables.")]
    public Image waitingCircle;
    public Text numPlayersWaitingGame;
    public Animator animator;

    [Header("Chat Variables")]
    public Text chatWindow;
    public InputField chatMsg;
    public Button submitChat;
    public bool isChatConnected;




    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        sqlManager = GetComponent<SQLManager>();
        chatManager = GetComponent<_ChatManager>();
        manager = GetComponent<_Manager>();  
        connectionManager = GetComponent<_ConnectionManager>();
        roomInfo = PhotonNetwork.CurrentRoom;
        animator = GameObject.Find("WaitingCircle").GetComponent<Animator>();
        animator.SetBool("waitingPlayers", true);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
         connectionManager.isConnected = true;
        numPlayersWaitingGame.text = "Current number of players " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            animator.SetBool("waitingPlayers", false);
        }  
        } 

        if((chatManager.getChatConnectionState() == true) && (isChatConnected == false))
        {
            
            chatManager.SubscribeUserToChannel(PhotonNetwork.CurrentRoom.Name, PlayerPrefs.GetString("Username"));
            chatManager.SendMessageToChannel(PhotonNetwork.CurrentRoom.Name, PlayerPrefs.GetString("Username") + " has joined the room..");
            isChatConnected = true;
        }

        chatconnection.text = chatManager.isChatConnectionStateString();
        netwkconnection.text = connectionManager.isConnectionStateString();


    }
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        chatManager.UnsubscribeToChannel(PhotonNetwork.CurrentRoom.Name, PlayerPrefs.GetString("Username"));

        PhotonNetwork.LoadLevel("TestScene");


    }

    public override void OnJoinedRoom()
    {

        roomnamelabel.text = PhotonNetwork.CurrentRoom.Name;

        if(PhotonNetwork.IsConnectedAndReady)
        {
            chatWindow.text = "";
            StartCoroutine(playerList());
            chatManager.EstablishConnection();
            //string channel = PhotonNetwork.CurrentRoom.Name.ToString();
          

        }

    }

     public void SendMessageToChat()
    {
        //Use PlayerPrefs to get users name. 
        string messg = PlayerPrefs.GetString("Username") + " says " + chatMsg.text;
        //string messg = manager.sessionInstance().username + " says " + chatBox.text;
        chatManager.SendMessageToChannel(PhotonNetwork.CurrentRoom.Name, messg);
        chatMsg.text = "";
        Debug.Log("HELLLO...");

    }

     public override void OnDisconnected(DisconnectCause cause)
    {

        //sqlManager.UpdateUserStatus(PlayerPrefs.GetInt("UserID"), 1);  
        PhotonNetwork.LoadLevel("TestScene");
        //MAYBE READD TO ROOM IF A DISCONNECTION WAS DETECTED>....



    }


    //Update the users lobby state if they leave the room for any reason.
  

    IEnumerator playerListUpdate()
    {
        userListUpdate = true;
           while(true)
            {
            
                    Dictionary<int, Photon.Realtime.Player> pList = Photon.Pun.PhotonNetwork.CurrentRoom.Players;

                    foreach (KeyValuePair<int, Photon.Realtime.Player> p in pList)
                    {
                     //ActorNUmber is -1 outside of rooms....
                       if(!currentActorsID.Contains(p.Value.ActorNumber))
                       {
                            GameObject listing = Instantiate(userPrefab, userPanel.transform);
                            listing.name = p.Value.ActorNumber.ToString();
                            Text btnText = listing.GetComponentInChildren<Text>();     
                            btnText.text = p.Value.NickName;
                            Debug.Log("Nickname is"+p.Value.NickName);
                            listing.transform.SetParent(userPanel.transform);
                             //Add to the list of current players in the room.
                            currentActorsID.Add(p.Value.ActorNumber);

                            numPlayers = PhotonNetwork.CurrentRoom.PlayerCount; 
                       }
                    }                                 
                
              //  Debug.Log("Waiting for "+ PlayerUpdateThreadInterval + " to run again");
                yield return new WaitForSeconds(PlayerUpdateThreadInterval);

            }
            

    }

    IEnumerator playerList()
    {
        while(true)
        {
             Player[] pList = PhotonNetwork.PlayerList;
            foreach(Player p in pList)
            {
                 if(!currentActorsID.Contains(p.ActorNumber))
                       {
                            GameObject listing = Instantiate(userPrefab, userPanel.transform);
                            listing.name = p.ActorNumber.ToString();
                            Text btnText = listing.GetComponentInChildren<Text>();     
                            btnText.text = p.NickName;
                            Debug.Log("Nickname is"+p.NickName);
                            listing.transform.SetParent(userPanel.transform);
                             //Add to the list of current players in the room.
                            currentActorsID.Add(p.ActorNumber);

                            numPlayers = PhotonNetwork.CurrentRoom.PlayerCount; 
                       }
            }
               // Debug.Log("Waiting for "+ PlayerUpdateThreadInterval + " to run again");
                yield return new WaitForSeconds(PlayerUpdateThreadInterval);

        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left, disconnected or closed the game");
        //Stop the routine because we are modifying the list. 
       // StopCoroutine(playerListUpdate());
        foreach(int x in currentActorsID)
        {
            if(x == otherPlayer.ActorNumber)
            {
                //currentRoomPlayers.Remove(x);
                GameObject playerListItem = GameObject.Find(otherPlayer.ActorNumber.ToString());
                sqlManager.UpdateUserStatus(PlayerPrefs.GetInt("UserId"), 1);
                Destroy(playerListItem);
            }
        }

       

        PhotonNetwork.LoadLevel("TestScene");
        //restart the routine
        StartCoroutine(playerListUpdate());
    }

    

    
}
