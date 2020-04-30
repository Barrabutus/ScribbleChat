using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class _Manager : MonoBehaviour
{

    
    public _Manager instance;
   public _GameManager _gameManager;
    public _ChatManager _chatManager;
   public _LobbyManager _lobbyManager;
    public _ConnectionManager _connectionManager;
   public SQLManager _sqlManager;
   //public _RoomManager _roomManager;
   public bool isUserUpdateRunning;
    [Header("Test Settings...")]
    public bool testUser;
    public string testUsername;
    public string[] testData;
    public Text usernameLabel;
    public bool isUserSubscribed;

    public List<_User> testUserList;

    public float updateInterval { get; private set; }

    //[Header("Users Data")]
    //public List<_User> users = new List<_User>();

    //public _User _testUser;

    public _User owner;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _sqlManager = GameObject.Find("SQLManager").GetComponent<SQLManager>();
        isUserSubscribed = false;
        if(PlayerPrefs.GetString("Username") == "")
        {  
        // DontDestroyOnLoad(this.gameObject);        
        _User newUser = new _User();
        //newUser.id = UnityEngine.Random.Range(1,100);
        newUser.username = testUsername+UnityEngine.Random.Range(0000,9999);
        //Users are place in the lobby  by default until they join a room.
        newUser.isInLobby = 1;
        usernameLabel.text = newUser.getInstance().username;    
        //Gives us a response of the newest ID....
        _sqlManager.AddUser(newUser, 0);
        //newUser.id = _sqlManager.getLastInsertId();
        owner = newUser;
        PlayerPrefs.SetString("Username", newUser.username);
        instance = this;
        //Connect to chat.
        _chatManager.EstablishConnection();

        }else{
            _chatManager.EstablishConnection();
            usernameLabel.text = PlayerPrefs.GetString("Username");
            //Debug.Log("NO USERNAME IN PLAYERPREFS");
        }

        

    }

    // Update is called once per frame
    void Update()
    {
       
        if(_chatManager.GetConnectionState() && _connectionManager.GetConnectionState())
        {
            if(!isUserUpdateRunning)
            {
               StartCoroutine(GetUsersFromDB());
                owner.id = _sqlManager.getLastInsertId();
               ////MAYBE REMOVE the owner if getLastInsertId is accurate...
               PlayerPrefs.SetInt("UserId", owner.id);
            }
               if(_chatManager.SubscribeUserToChannel(_ChatChannels.Global.ToString(), PlayerPrefs.GetString("Username")+ PlayerPrefs.GetInt("UserId")) && (isUserSubscribed == false))
               {

               }
               if(!isUserSubscribed)
               {
                   if(_chatManager.SubscribeUserToChannel(_ChatChannels.Global.ToString(), PlayerPrefs.GetString("Username")+ PlayerPrefs.GetInt("UserId")))
                   {
                       isUserSubscribed=true;
                   }
               }
            
            //Create SQL Timer Function to get updates...
            
            //If both are connected we are ready to get users from the database. 

            //Debug.Log("URL RESPOSNSE IS " + response);
           // _sqlManager.

        }  
    }

    IEnumerator GetUsersFromDB()
    {
        while(true)
        {       
            isUserUpdateRunning = true;
            _sqlManager.GetRows();
            //CLEAR THE RESPONSE FROM THE TEST USERS ENTRY
            string response = ""; 
            response = _sqlManager.getUrlResponse();
            yield return new WaitForSeconds (_sqlManager.SQLThreadInterval);
        }
    }

    public _User sessionInstance()
    {
        return owner;
    }


    // public void ParseUserData(string SQLResponse)
    // {
    //      string[] usersData = SQLResponse.Split('-');
    //      //testData = SQLResponse.Split('-');
           
    //             foreach(string user in usersData)
    //             {
    //                     string[] details; 
    //                     details = user.Split(' ');
    //                     //Checks for no empty users.
    //                     if(details[0] != "")
    //                     {  
    //                             string playername = details[1];
    //                             //if the lobbyuser exists then skip them....
    //                             if(!isDuplicateUser(playername))
    //                             {
    //                                 _User newUser = new _User();
    //                                 newUser.id = Convert.ToInt16(details[0]);
    //                                 newUser.username = details[1];
    //                                 newUser.isInLobby = Convert.ToInt16(details[2]);
    //                                 //Debug.Log(newUser.name + " Does not exist in LobbyUSers...");
    //                                 users.Add(newUser);

    //                             }else{
                                    
    //                             for(int x = 0; x < users.Count; x++)
    //                                 {
    //                                         if(users[x].id == Convert.ToInt16(details[0]))
    //                                         {                                               
    //                                             users[x].isInLobby = Convert.ToInt16(details[2]);
    //                                         }
    //                                 }
    //                          }                
    //                     }
    //             }              
    //            // Debug.Log("SHOULD QUERY FOR UPDATES....");            
    //             //Send User Data to Lobby Manager to update users list of whose in a room and who isnt...
    // }

    // IEnumerator userListThread()
    // {
    //      //testData = SQLResponse.Split('-');
    //     while(true)
    //     {   
    //         testUserList = new List<_User>();
    //         testUserList = _sqlManager.GetUsersList();

    //         foreach(_User user in testUserList)
    //         {
                
    //             if(!isDuplicateUser(user.id))
    //             {
    //                 //if the user id does not exist in the current list of users. 
    //                 GameObject userNamePrefab = Instantiate();


    //             }

    //         }                
    //         yield return new WaitForSeconds(updateInterval);
    //     }              
 
    // }

    //   bool isDuplicateUser(int id)
    // {
    //     bool isDupe;

    //     foreach(_User _user in _sqlManager.GetUsersList())
    //     {
    //         if(_user.username == name)
    //         {
    //             isDupe = true;
    //             return isDupe;
    //         }

    //     }


    //     isDupe = false;
    //     return isDupe;
    // }




    

 



    /////TEST FUNCTIONS... TO BE DELETED IN FINAL.
}
