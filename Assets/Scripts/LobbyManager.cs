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
    public InputField inputRoomName;

    [Header("Room Listings Config")]
    public GameObject listingPrefab;
    public List<string> currentRoomList = new List<string>();
    public Transform listingsPanel;

    //FORMATTING ROOM NAMES TO BE ALL UPPERS
    public void OnEnterInput()
    {
        inputRoomName.text = inputRoomName.text.ToUpper();   
    }

    private void Awake() {
        inputRoomName.onValidateInput += delegate (string input, int charIndex, char addedChar) { return SetToUpper(addedChar); };
    }
    public char SetToUpper(char c) {
        string str = c.ToString().ToUpper();
        char[] chars = str.ToCharArray();
        return chars[0];
    }
    ///END FORMATTING
    public void SetUsernameLabels()
    {
        foreach (Text item in usernameLabels)
        {
            item.text = PhotonNetwork.NickName;
        }
    }

    public void OnClickCreateRoom()
    {
        if(inputRoomName.text.Length > 15)
        {
            inputRoomName.text = "No More than 15 characters for a room name";
        }else{
            PhotonNetwork.CreateRoom(inputRoomName.text, new RoomOptions {MaxPlayers = 2, EmptyRoomTtl = 3000});
        }
    }

   public override void OnCreatedRoom()
   {
       Debug.Log("Room Created...");
   }


    // Update is called once per frame
    void Update()
    {
          
    }
    public override void OnLeftLobby()
    {
        ChatManager chatManager = GameObject.Find("ChatManager").GetComponent<ChatManager>();
        chatManager.SendMessageToChannel(PhotonNetwork.NickName + " has left the room");
    }

     public void OnClickJoinRoom(string roomname)
    {          
        Debug.Log(roomname);
        PhotonNetwork.JoinRoom(roomname);
        PhotonNetwork.LoadLevel("GameScene");       
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Did the OnRoomListUpdate actually occur??
        Debug.Log("FIRED");
        foreach(RoomInfo info in roomList)
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
            Debug.Log("Cannot find a gameobject with the name of " + info.Name);
        }
    }

    public void CreateNewRoomListItem(RoomInfo info)
    {
        //Double check that the room does not exist.
        if(!currentRoomList.Contains(info.Name))
        {
            GameObject listing = Instantiate(listingPrefab, listingsPanel.transform);
            Text btnText = listing.GetComponentInChildren<Text>();
            listing.GetComponent<Button>().onClick.AddListener(() => { OnClickJoinRoom(info.Name); });
            btnText.text = info.Name + " - "+ info.PlayerCount + " / " + info.MaxPlayers;
            listing.transform.SetParent(listingsPanel.transform);
            Debug.Log("ROOM NAME"  + info.Name);
            listing.name = info.Name;
            currentRoomList.Add(info.Name);
        }
    }



}
