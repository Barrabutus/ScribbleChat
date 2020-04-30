using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

//Class for maintaining connection to PhotonNetwork.
public class _ConnectionManager : MonoBehaviourPunCallbacks
{

    public bool isConnected;
    [Header("Status Labels")]
    public bool toggleConnection;
    public _ConnectionManager instance;

    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        instance = this;
    }

    public void EstablishConnection()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        isConnected = true;
        //connectionStateLabel.text = "<Color=yellow>Connected to Network</color>";
        PhotonNetwork.NickName = PlayerPrefs.GetString("Username");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        isConnected = false;
        //connectionStateLabel.text = "<Color=red>Disconnected from Network</color>";
    }

    private void Update() {
        
        if(toggleConnection)
        {PhotonNetwork.Disconnect();}

        if(!toggleConnection)
        {
            if(!PhotonNetwork.IsConnected)
            {
                
                PhotonNetwork.ConnectUsingSettings();

            }
        }
       
    }

    public bool GetConnectionState()
    {
        return isConnected;
    }

    public string isConnectionStateString()
    {
        if(isConnected)
        {
            return "<Color=yellow>Connected to Network</color>";
        }else{
            return "<Color=red>Disconnected from Network</color>";
        }
    }

    


}