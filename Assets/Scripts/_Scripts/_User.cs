using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Class for defining a user.
public class _User 
{
    public int id; 
    public string username;
    //1 = in the lobby area. 
    //0 = in a room.
    public int isInLobby;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public _User getInstance()
    {
        return this;
    }
}
