using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    private byte maxPlayersPerRoom = 4;
    [SerializeField] [Tooltip("The level of logging to show from Photon.")]
    public PunLogLevel loglevel = PunLogLevel.Informational;
    [SerializeField] [Tooltip("The number of updates that should be sent/received to the server per second. Higher values will result in smoother gameplay at the cost of increased network traffic. Values above 20 may result " +
                              "in server timeouts if using cloud servers.")]
    public int sendRate = 30;
    public bool autoconnect = true;
    public GameObject connectionMenu;

    static NetworkLauncher _instance;
    public static NetworkLauncher Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<NetworkLauncher>()); }
    }

    void Awake()
    {
        if (!PhotonNetwork.IsConnected && autoconnect)
        {
            ConnectToServer();
        }
    }

    public void ConnectToServer()
    {
        string userId = Guid.NewGuid().ToString();

        // We use the player custom properties as a workaround because the user id set in AuthValues returns null for the master client
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.SetPlayerCustomProperties(properties);
        PhotonNetwork.AuthValues = new AuthenticationValues(userId);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LogLevel = loglevel;
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = sendRate;

        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(gameObject);

        if (connectionMenu != null)
            Destroy(connectionMenu);
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master");

        Debug.Log("Joining random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby() {
        Debug.Log("Joined lobby");

        Debug.Log("Joining random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnLeftLobby() {
        Debug.Log("Left lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string msg) {
        Debug.Log("Can't join random room!");
        Debug.Log("Creating room...");

        var options = new RoomOptions();
        options.MaxPlayers = maxPlayersPerRoom;
        options.PlayerTtl = 10000;
        PhotonNetwork.CreateRoom(null, options, null);
    }

    public override void OnCreatedRoom() {
        Debug.Log("Created room");
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
    }

    public override void OnLeftRoom() {
        Debug.Log("Left room");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log("Player connected");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player disconnected");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Connection failed to the Photon network, cause " + cause.ToString());
    }

}