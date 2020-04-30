using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


//Manager for the chat channels and functions;
public class _ChatManager : MonoBehaviour, IChatClientListener
{

    public bool isConnected;
    public ChatClient chatClient;
    public Text globalChatWindow;
    public bool toggleConnection;

    private void Awake() {
        globalChatWindow.text = "";
    }


    //USER DEFINED METHODS

    public void EstablishConnection()
    {
        ConnectionProtocol connectProtocol = ConnectionProtocol.Udp;
        chatClient = new ChatClient(this,connectProtocol);
        chatClient.ChatRegion = "EU";
        Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
        authValues.AuthType = Photon.Chat.CustomAuthenticationType.None;
        if(!chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,"0", authValues))
        {
            Debug.Log("Unable to connect to chat...");
        }
    }

    public void SendMessageToChannel(string ChannelName, string message)
    {

        chatClient.PublishMessage(ChannelName, message);

    }

    public bool GetConnectionState()
    {
        return isConnected;
    }

    public bool SubscribeUserToChannel(string ChannelName, string name)
    {

        if(!chatClient.Subscribe(new string[] {ChannelName}))
        {
            Debug.Log("Unable to subscribe to channel " + ChannelName);
            Debug.Log(GetConnectionState());
            return false;
        }

        return true;
        //Debug.Log("user subscribed to channel " + ChannelName);
        ///chatUser = user;

    }

    public void UnsubscribeToChannel(string ChannelName, string name)
    {
        
    }

 
    //REQUIRED BY IChatClientListener (PhotonChat)
    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        isConnected = true;
    }

    public void OnDisconnected()
    {
        isConnected = false;
    }

    // public void OnGetMessages(string channelName, string[] senders, object[] messages)
    // {
    //      string msgs = "";
    //         for (int i = 0; i < senders.Length; i++)
    //         {
    //             msgs = messages[i].ToString();
    //            // globalChatWindow.text += msgs + "\n";
               
    //         }
        
    // }
    public void OnGetMessages(string channelName, string[] senders, object[] messages )
    {
       string msgs = "";
       for ( int i = 0; i < senders.Length; i++ )
       {
           //msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
            msgs = messages[i].ToString();
            globalChatWindow.text += msgs + "\n";

       }
       Debug.Log( "OnGetMessages: "+ channelName+ " - " + senders + " > " + messages);
       // All public messages are automatically cached in `Dictionary<string, ChatChannel> PublicChannels`.
       // So you don't have to keep track of them.
       // The channel name is the key for `PublicChannels`.
       // In very long or active conversations, you might want to trim each channels history.
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
            //Debug.Log("User subscribed to " + channels.ToString());
            //chatClient.PublishMessage(PlayerPrefs.GetString("Username") + " has joined the channel...");   
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient!=null) { chatClient.Service(); }
    }

    public bool getChatConnectionState()
    {
        return isConnected;
    }

    
    public string isChatConnectionStateString()
    {
        if(isConnected)
        {
            return "<Color=yellow>Connected to Chat Server</color>";
        }else{
            return "<Color=red>Disconnected from Chat Server</color>";
        }
    }




}

