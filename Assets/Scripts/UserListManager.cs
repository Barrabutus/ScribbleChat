using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using UnityEngine.UI;
public class UserListManager : MonoBehaviour
{

    public List<ChatClient> clients = new List<ChatClient>();
    public List<string> usernames = new List<string>();
    public UserListManager instance;

    public Transform userlistBox;
    public GameObject userlistPrefab;

    public int numCurrentChatters;
    void Start()
    {
        instance = this;
        numCurrentChatters = clients.Count;
    }
    
    public void addNewClient(ChatClient client, string username)
    {
        //Chatclient index will be same as username index.
        clients.Add(client);
        usernames.Add(username);
        generateUserList();
    }

    public void generateUserList()
    {
        foreach(string name in usernames)
        {
           GameObject newClient = Instantiate(userlistPrefab, userlistBox.transform.position, Quaternion.identity);
            newClient.transform.SetParent(userlistBox.transform);
            newClient.transform.localScale = new Vector3(1,1,1);
            Text txt = newClient.GetComponentInChildren<Text>();
            txt.fontSize = 14;
            txt.text = name;
        }
    }

    public int getNumChattersAmount()
    {
        return numCurrentChatters;
    }


    ///UPDATE THE USER CHAT ROOM LIST


  

}
