using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.Video;
using System;
using SocketMessages;

public class SocketSettings
{
    public string ipAdress;
    public string port;
}

public class SocketManager : MonoBehaviour
{

    #region Variables

    [Header("Socket")]
    public string serverIP;
    public string serverPort;
    public bool isConnected;

    [Header("UI")]
    bool contentsLoaded;
    public GameObject connectionErrorPanel;
    public GameObject socketSettingPanel;
    public InputField inputIPAdress;
    public InputField inputPort;

    [Header("Scripts Socket")]
    public SocketIOComponent socket;
    public SocketEmitMessageList socketEmitMessageList;
    SocketSettings socketSettings;

    [Header("Scripts Global")]
    [SerializeField] AppManager appManager;

    #endregion


    #region Standard Function

    private void Awake()
    {
        // Load Socket settings
        LoadSettings();
    }

    public void StartConnection()
    {
        // Connection
        socket.gameObject.SetActive(true);
        socket.Connect();

        // socket listeners
        socket.On("player:get-player-event", ReceivePlayerEvent);
    }

    public void ReceivePlayerEvent(SocketIOEvent e)
    {
        if(e.data == null) return;
        Debug.Log("[SocketIO]: " + e.name + " " + e.data);
    }

    #endregion


    #region Socket Settings Function

    public void UpdateServerAdress()
    {
        socket.url = "ws://" + serverIP + ":" + serverPort + "/socket.io/?EIO=4&transport=websocket";
    }

    public IEnumerator DisableBlocker(float _wait)
    {
        yield return new WaitForSeconds(_wait);
        if (connectionErrorPanel.activeSelf)
            connectionErrorPanel.transform.GetChild(connectionErrorPanel.transform.childCount - 1).gameObject.SetActive(false);
    }

    #endregion


    #region Socket Standard Listeners

    public void SendMessageToNode(string _message, string _contents)
    {
        //Debug.Log(_message);
        JSONObject jsonContents = new JSONObject(_contents);
        socket.Emit(_message, jsonContents);
    }

    #endregion


    #region UI Socket Settings

    public void ManageSocketSettingWindow(bool state)
    {
        socketSettingPanel.SetActive(state);
    }

    public void ChangeIpAdress(string _ip, string _port)
    {
        serverIP = _ip;
        serverPort = _port;
    }

    public void UpdateIpAdress(string _ip, string _port)
    {
        SaveServerSettings(_ip, _port);
        connectionErrorPanel.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveSettingsButton()
    {
        string _newIp = inputIPAdress.text;
        if (_newIp == "")
            _newIp = inputIPAdress.placeholder.GetComponent<Text>().text;
        string _newPort = inputPort.text;
        if (_newPort == "")
            _newPort = inputPort.placeholder.GetComponent<Text>().text;
        UpdateIpAdress(_newIp, _newPort);
        // UI
        ManageSocketSettingWindow(false);
    }

    #endregion


    #region Player Prefabs 

    public void LoadSettings()
    {
        socketSettings = new SocketSettings();

        if (PlayerPrefs.HasKey("ipAdress") && PlayerPrefs.HasKey("port"))
            ChangeIpAdress(PlayerPrefs.GetString("ipAdress"), PlayerPrefs.GetString("port"));
        else
            UpdateIpAdress("127.0.0.1", "8888");

        UpdateServerAdress();

        StartConnection();
    }

    public void SaveServerSettings(string _newIp, string _newPort)
    {
        socketSettings.ipAdress = _newIp;
        PlayerPrefs.SetString("ipAdress", _newIp);
        socketSettings.port = _newPort;
        PlayerPrefs.SetString("port", _newPort);
    }

    #endregion

}