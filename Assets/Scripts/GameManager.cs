using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DodoDataModel;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string content = String.Empty;
    private string contentReply = String.Empty;
    private string url = "http://localhost:5001/hello";
    private HubConnection hubConnection = null;
    private UnityMainThreadDispatcher _dispatcher;
    public User user;


    async void Start()
    {
        // create new guid
        user = new User();
        user.guid = Guid.NewGuid();

        // set room 
        user.keyRoom = "lol";

        // set distpacher
        _dispatcher = UnityMainThreadDispatcher.Instance();

        // start client
        await this.StartSignalRAsync();
    }

    private GUIStyle _guiStyle = new GUIStyle();

    void OnGUI()
    {
        _guiStyle.fontSize = 40;
        GUI.Label(new Rect(100, 100, 100, 200), "msg: " + content, _guiStyle);

        if (!String.IsNullOrEmpty(contentReply))
        {
            GUI.Label(new Rect(100, 300, 100, 200), "Reply: " + contentReply, _guiStyle);
        }
    }


    void ReplyServer(string w)
    {
        _dispatcher.Enqueue(() => { contentReply = "Комната готова - можно начинать"; });
    }

    async Task StartSignalRAsync()
    {
        if (this.hubConnection == null)
        {
            // create hub and settings
            this.hubConnection = new HubConnectionBuilder()
                .WithUrl(url, options => { })
                .Build();

            // subscribe to method on server
            this.hubConnection.On<string>("OnRoomComlete", ReplyServer);


            // start server
            await this.hubConnection.StartAsync();


            // registation client on server 
            await hubConnection.InvokeAsync("ClientRegistration", Newtonsoft.Json.JsonConvert.SerializeObject(user));

            content += "Status: connected";
        }
        else
        {
            content = "Status: already connected";
        }
    }
}