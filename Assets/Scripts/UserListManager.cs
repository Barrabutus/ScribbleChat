using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UserListManager : MonoBehaviourPunCallbacks
{
    public string hiddenUserRoom = "HIDDEN";
    public int numPlayers;
    public List<int> currentRoomPlayers =new List<int>();
    public Transform userPanel;
    public GameObject userPrefab;
    public ConnectionManager connectionManager;
  
}
