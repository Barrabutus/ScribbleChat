using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.Collections;
using System;

public class WebCommunication : MonoBehaviour {

    public string webUrl;
    public string webPostUrl;
    private string getResult;
    public string action;

    [System.Serializable]
    public enum DBCommand
    {
        ADD,
        DELETE,
        GETALL,
        GET,
        UPDATE,

        LOBBY,
    }
    public DBCommand command;
    public string response;

    public ConnectionManager connectionManager;
    public LobbyManager lobbyManager;
    
    void Start() {      
        //command = DBCommand.ADD;
        connectionManager = GetComponent<ConnectionManager>();
       lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    public void GetAllRows()
    {

        action = DBCommand.GETALL.ToString();
        StartCoroutine(GetAll());
    }

    //AddUser will return the newest ID of the new user.
    public string AddUser(string user, int score,  int inLobby)
    {
        action = DBCommand.ADD.ToString();
        StartCoroutine(AddNew(user, score,  inLobby));

        return response;

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

    public void GetAllLobbyUsers()
    {

        action = DBCommand.LOBBY.ToString();
        StartCoroutine(GetLobbyUsers());
      


    }

    //For getting data.
    IEnumerator GetAll() {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            connectionManager.response = www.downloadHandler.text;
            // Or retrieve results as binary data
           
        }
        

        yield return new WaitForSeconds(3f);
       
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
                Debug.Log("Unable to Delete data...");
            }else{
                
               Debug.Log(www.downloadHandler.text); 
            }
    
    }

    //ADDS A USER WHEN THEY JOIN THE LOBBY
    IEnumerator AddNew(string user, int score , int inLobby)
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));
        wwwForm.Add(new MultipartFormDataSection("username", user));
        wwwForm.Add(new MultipartFormDataSection("score", Convert.ToString(score)));
        wwwForm.Add(new MultipartFormDataSection("userstate", Convert.ToString(inLobby)));

        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                //Debug.Log("NEW USER ADDED TO DATABASE");
            }else{
                
               Debug.Log(www.downloadHandler.text); 
                connectionManager.UserId = www.downloadHandler.text;

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
                Debug.Log("Unable to update user ..." +rowId);

            }else{
                
               Debug.Log(www.downloadHandler.text); 
            }


    }


    IEnumerator GetLobbyUsers()
    {

        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("action", action));

        UnityWebRequest www = UnityWebRequest.Post(webPostUrl, wwwForm);
        yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Unable to get lobby users......");

            }else{
                
               lobbyManager.response = www.downloadHandler.text;

            }


    }


    







}

 