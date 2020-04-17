using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserDatabase : MonoBehaviour
{
   
    public List<LobbyUser> users = new List<LobbyUser>();
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setData(List<LobbyUser> _users)
    {

        _users = users;

    }

    public List<LobbyUser> getUsers()
    {
        return users;
    }

}
