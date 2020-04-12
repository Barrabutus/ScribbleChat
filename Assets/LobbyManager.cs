using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Text[] usernameLabels;
    public Text lobbyConnectionStateLabel;
    public Text chatConnectionStateLabel;
    // Start is called before the first frame update
    void Start()
    {
        
      


    }

    public void SetUsernameLabels()
    {
        foreach (Text item in usernameLabels)
        {
            item.text = PhotonNetwork.NickName;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        
    }
    public override void OnLeftLobby()
    {

        ChatManager chatManager = GameObject.Find("ChatManager").GetComponent<ChatManager>();
        chatManager.SendMessage(PhotonNetwork.NickName + " has left the room");

    }
}
