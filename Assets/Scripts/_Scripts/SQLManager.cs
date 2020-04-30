using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Class for managing SQL Commands
public class SQLManager : MonoBehaviour
{
    public string webPostUrl;
    public string urlResult;
    public string action;
    //How often do we want to query the database?
    //Currently every 2 seconds.
    public float SQLThreadInterval;

    public SQLManager instance;

    public List<_User> users = new List<_User>();

    public string SQLError;

    public int lastInsertId;
    
    [System.Serializable]
    public enum DBCommand
    {
        ADD,
        DELETE,
        GETALL,
        GET,
        UPDATE,
    
    }
    public DBCommand command;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }

 

    //This allows us to keep a "fresh" list  of users in the users list. 
    public void StartUserThread()
    {
        StartCoroutine(GetUsersFromDb());
    }


      public void GetRows()
    {
        action = DBCommand.GETALL.ToString();
        StartCoroutine(getRows());
        
    }

    public void AddUser(_User user, int score)
    {
        action = DBCommand.ADD.ToString();
        StartCoroutine(AddNew(user, score));
        
    }
    public void DeleteUser(string tablename, string tablecolumn, int tablerowid)
    {
        action = DBCommand.DELETE.ToString();
        StartCoroutine(DeleteRow(tablename, tablecolumn, tablerowid));
    }

    public void UpdateUserStatus(int rowid, int userstate)
    {
        action = DBCommand.UPDATE.ToString();
        StartCoroutine(UpdateUserState(rowid, userstate));
    }


    public void CreateWebForm(List<string> formValues, List<string> formData)
    {

    }

    private void Update() {
        
        
    }

    //For getting data. 
    //I only want this to run once to retrieve data and then the user can "get" the result from UrlResult?
    IEnumerator getRows() {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
          //urlResult = www.error;
          ShowSQLError(www.error);
        }
        else {
            // Show results as text
            // Debug.Log("RESULTS : " +www.downloadHandler.text);
            //connectionManager.response = www.downloadHandler.text;
            urlResult = www.downloadHandler.text;
            // Or retrieve results as binary data
           
        }
        

       
    }

    //FOR DELETING USERS..
    private IEnumerator DeleteRow(string table, string column, int rowId)
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        wwwForm.Add(new MultipartFormDataSection("table", table));
        wwwForm.Add(new MultipartFormDataSection("column", column));
        wwwForm.Add(new MultipartFormDataSection("rowId", Convert.ToString(rowId)));
        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
              ShowSQLError(www.error);
            }else{
                
              // Debug.Log(www.downloadHandler.text); 
            }
    
    }

    //ADDS A USER WHEN THEY JOIN THE LOBBY
    IEnumerator AddNew(_User user, int score)
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        wwwForm.Add(new MultipartFormDataSection("username", user.username));
        wwwForm.Add(new MultipartFormDataSection("score", Convert.ToString(score)));
        wwwForm.Add(new MultipartFormDataSection("userstate", Convert.ToString(user.isInLobby)));

        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
               ShowSQLError(www.error);
            }else{
                
             
               urlResult = www.downloadHandler.text;
                lastInsertId = Convert.ToInt16(www.downloadHandler.text);
            }

    }

    //WHEN THE USER JOINS A ROOM....OR RE-ENTERS THE LOBBY
    IEnumerator UpdateUserState(int rowId, int inLobby)
    {
      List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        wwwForm.Add(new MultipartFormDataSection("rowId", Convert.ToString(rowId)));   
        wwwForm.Add(new MultipartFormDataSection("state",  Convert.ToString(inLobby)));

        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                
                ShowSQLError(www.error);

            }else{
                
            //   Debug.Log(www.downloadHandler.text); 
            }


    }




//Get the results from the PHP Page?
    public string getUrlResponse()
    {
        
        return urlResult;
    }

    //Runs infinitely to keep the users up-to date. 
    IEnumerator GetUsersFromDb()
    {
      while(true)
      {
          users.Clear();
          if(urlResult != "")
          {
                string[] _users = urlResult.Split('-');
                foreach(string user in _users)
                {

                        
                        string[] details; 
                        details = user.Split(' ');

                        //Checks for no empty users.
                        if(details[0] != "")
                        {
                           //Create users from the list of players from the database. 
                            
                                string playername = details[1];
                                //if the lobbyuser exists then skip them....
                                if(!isDuplicateUser(playername))
                                {
                                    _User newUser = new _User();
                                    newUser.id = Convert.ToInt16(details[0]);
                                    newUser.username = details[1];
                                    newUser.isInLobby = Convert.ToInt16(details[2]);
                                    //Debug.Log(newUser.name + " Does not exist in LobbyUSers...");
                                    users.Add(newUser);
                                  
                                }

                        }
     
                }          
                
          }
           yield return new WaitForSeconds(SQLThreadInterval);
               
      }
    }

    bool isDuplicateUser(string name)
    {
        bool isDupe;

        foreach(_User user in users)
        {
            if(user.username == name)
            {
                isDupe = true;
                return isDupe;
            }

        }


        isDupe = false;
        return isDupe;
    }


    public List<_User> GetUsersList()
    {
        return users;
    }

    public void ShowSQLError(string error)
    {

        Debug.Log(error);

    }

    public int getLastInsertId()
    {
        return lastInsertId;
    }
    



}
