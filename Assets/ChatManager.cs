using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Chat;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public ChatClient client;
    public string username;
    public Text chatboxWindow;
    public Text chatConnectionStateLabel;
    public UserListManager userListManager;

    [Header("User Chat Input Controls")]
    public InputField userChatInputField;

    public List<Player> players = new List<Player>();
    public Player[] players__;




    enum  _Channel{
       Global,
        Private,
        Room

    };
    

    public void GetConnectedUsers()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);
        int numplayers = PhotonNetwork.CountOfPlayers;

        for(int x = 0; x < numplayers; x++)
        {
            
        }
        
       
    }
    

    public void SetUsername(string name)
    {
        username = name;
    }
    
    void Start()
    {
       chatboxWindow.text = "";
       userListManager = GameObject.Find("ChatManager").GetComponent<UserListManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (client!=null) { client.Service(); }
    }

    public void ConnectToChatServer()
    {

        ConnectionProtocol connectProtocol = ConnectionProtocol.Udp;
        client = new ChatClient(this,connectProtocol);
        client.ChatRegion = "EU";
        Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
       // authValues.UserId = username;
        authValues.AuthType = Photon.Chat.CustomAuthenticationType.None;
        client.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,"0", authValues);



    }

    public void OnClickSendMessageToChannel()
    {
        if(userChatInputField.text.Length > 0)
        {
             string message = username + " says " + userChatInputField.text;
             client.PublishMessage(_Channel.Global.ToString(), message);
             userChatInputField.text = "";

        }else{
            userChatInputField.text = "";
        }
    }

    public void SendMessageToChannel(string msg)
    {
        
        client.PublishMessage(_Channel.Global.ToString(), msg);


    }

    public void UserUnsubFromChannel()
    {
        client.Unsubscribe(new string[] {_Channel.Global.ToString()});
    }

 


    //-----PREDEFINED PHOTON CHAT METHODS------/////
    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
      
      Debug.Log("Welcome " + username);
      client.Subscribe(new string[] {_Channel.Global.ToString()});
      chatConnectionStateLabel.text = "Connected to chat.";
      GetConnectedUsers();

    }

    public void OnDisconnected()
    {
        chatConnectionStateLabel.text = "Disconnected from chat";
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
            string msgs = "";
            for ( int i = 0; i < senders.Length; i++ )
            {
                msgs = messages[i].ToString();
                chatboxWindow.text += msgs + "\n";
            }


    }
    void OnApplicationQuit()
    {
         chatConnectionStateLabel.text = "Disconnected from chat";

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        client.PublishMessage(_Channel.Global.ToString(), username + " has joined the global channel");
        userListManager.addNewClient(client, username);
    }

    public void OnUnsubscribed(string[] channels)
    {
        client.PublishMessage(_Channel.Global.ToString(), username + " has left the global channel");
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
  


 
}
