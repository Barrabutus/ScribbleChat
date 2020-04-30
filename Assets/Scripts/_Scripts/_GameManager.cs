using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for managing a game and the users chat.
public class _GameManager : MonoBehaviour
{

    public List<_User> GameUsers = new List<_User>();
    public _GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
