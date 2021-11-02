using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using SocketMessages;

public class AppManager : MonoBehaviour
{
    [Header("Global")]
    public bool debugServer;

    [Header("Scripts")]
    public SocketManager socketManager;

    //send cmd to TouchDesigner
    public void ChangeVideo (string _cmd) {
        if (debugServer) return;
        PlayerEvent newPlayerEvent = new PlayerEvent();
        newPlayerEvent.cmd = _cmd;
        string json = JsonUtility.ToJson(newPlayerEvent);
        socketManager.SendMessageToNode(socketManager.socketEmitMessageList.playerMessage, json);
    }
}
