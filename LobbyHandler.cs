using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyHandler : NetworkBehaviour {
    //private const short testMessageFromServer = 131;
    //private const short testMessageFromClient = 132;
    public LobbyGUI LobbyGUI;
    public StartGameCountDown StartGameTimer;
    private const short testMessage = 133;
    private const short initMessage = 134;
    private const short confirmInit = 135;
    private const short recDataServ = 136;
    private const short recDataClient = 137;
    private const short startGame = 138;
    private NetworkClient myClient;
    private int matchSize;
    private Dictionary<int, PlayerInfo> playerChoices = new Dictionary<int, PlayerInfo>();
    private bool isReady;
    private byte team;
    private byte chosenSlot;
    private string playerName;
    private int myNetID;
    private AdvancedNetworkManager netman;
    private ScoreKeeper score;
    private int numberOfPlayers = 1;
    
    



    // Use this for initialization



    public struct Players
    {
        public PlayerInfo player1;
        public PlayerInfo player2;
        public PlayerInfo player3;
        public PlayerInfo player4;
        public PlayerInfo player5;
        public PlayerInfo player6;
        public PlayerInfo player7;
        public PlayerInfo player8;
     
    }

    
    public class PlayerData : MessageBase
    {
        public Players p;
    }

    public class InitData : MessageBase
    {
        public int netID;
    }
    public class PlayerInfoMessage : MessageBase
    {
        public PlayerInfo p;
    }

	void Start () {
        //Debug.Log(GetComponent<NetworkIdentity>().netId.Value);
        score = GetComponent<ScoreKeeper>();
        
        if (isServer)
        {
            
            PlayerInfo p = new PlayerInfo();
            p.id = (int)GetComponent<NetworkIdentity>().netId.Value;
            p.name = "Player" + numberOfPlayers;
            numberOfPlayers++;
            Debug.Log("amount of players" + NATTraversal.NetworkManager.singleton.numPlayers);
            p.team = 100;
            p.chosenSlot = 100;
            p.isReady = false;
            playerChoices.Add(p.id, p);
            playerName = p.name;
            team = p.team;
            chosenSlot = p.chosenSlot;
            isReady = false;
            myNetID = p.id;
            UpdateUI();
            
            
            //NetworkServer.RegisterHandler(testMessageFromClient, ReceiveTestMessageFromClient);
            NetworkServer.RegisterHandler(initMessage, ReceiveInitClient);
            NetworkServer.RegisterHandler(recDataClient, ReceivePlayerDataFromClient);
            matchSize = (int)NATTraversal.NetworkManager.singleton.matchSize;

            
        }else
        {
            
            myClient = NATTraversal.NetworkManager.singleton.client;
            
            Debug.Log("amount of players" + myClient.connection.playerControllers.Count);
            //myClient.RegisterHandler(testMessageFromServer, ReceiveTestMessage);
            myClient.RegisterHandler(confirmInit, RecInitConfirm);
            myClient.RegisterHandler(recDataServ, ReceivePlayerDataFromServer);
            myClient.RegisterHandler(startGame, ReceiveStartGame);
            //StringMessage msg = new StringMessage();
            PlayerData msg = new PlayerData();
            msg.p = new Players();
            msg.p.player1 = new PlayerInfo();
            msg.p.player2 = new PlayerInfo();
            msg.p.player1.id = 2;
            msg.p.player1.name = "Daniel";
            msg.p.player2.id = 3;
            msg.p.player2.name = "Anders";
            Debug.Log(NATTraversal.NetworkManager.singleton.matchSize);
            //myClient.Send(testMessageFromClient, msg);
            PlayerInfoMessage playermsg = new PlayerInfoMessage();
            playermsg.p = new PlayerInfo();
            playermsg.p.id= (int)GetComponent<NetworkIdentity>().netId.Value;
            playermsg.p.name = "Player" + playermsg.p.id;
            playermsg.p.chosenSlot = 100;
            playermsg.p.team = 100;
            playermsg.p.isReady = false;
            playerName = playermsg.p.name;
            chosenSlot = 100;
            team = 100;
            isReady = false;
            myNetID = playermsg.p.id;
      
            myClient.Send(initMessage, playermsg);

            
        }
		
	}
    public override void OnStartServer()
    {
        base.OnStartServer();
        //NetworkServer.RegisterHandler(testMessageFromClient, ReceiveTestMessageFromClient);
        //GameObject.Find("AdvancedNetworkManager").GetComponent<AdvancedNetworkManager>().SetServerHandler(transform.gameObject);
        netman = GameObject.Find("AdvancedNetworkManager").GetComponent<AdvancedNetworkManager>();
    }

    public void ClientDisconnected(int id)
    {
        
        playerChoices.Remove(id);
       
        SendClientData();
        UpdateUI();
    }

    public void StartGame()
    {
        LobbyGUI.CloseLobby();
        StartGameTimer.isCounting = true;
        score.InitializeScoreKeeper(playerChoices);
        score.running = true;
        this.enabled = false;
    }

    public void ReceiveStartGame(NetworkMessage msg)
    {
        Debug.Log("Should start Game");
        StartGame();
    }
    // Update is called once per frame
    void Update () {
		if(isServer)
        {
            if (netman.playerHasDisconnectedLobby)
            {
                
                netman.playerHasDisconnectedLobby = false;
                playerChoices.Remove(netman.playerDisconnectedID);
                SendClientData();
                UpdateUI();
            }
        
        }
	}
    public void DataChanged()
    {
        if (isServer)
        {
            UpdateDataLocally();
            
        }else
        {
            SendDataFromClient();
        }
    }
    public void UpdateDataLocally()
    {
        PlayerInfo pi = new PlayerInfo();
        pi.id = myNetID;
        pi.name = playerName;
        pi.team = team;
        pi.chosenSlot = chosenSlot;
        pi.isReady = isReady;
        playerChoices[pi.id] = pi;
        SendClientData();
        UpdateUI();

    }
    public void ReceivePlayerDataFromServer(NetworkMessage msg)
    {
        Players p = msg.ReadMessage<PlayerData>().p;
        playerChoices.Clear();
        if (p.player1.id!= 0)
        {
            playerChoices.Add(p.player1.id, p.player1);
        }
        if (p.player2.id != 0)
        {
            playerChoices.Add(p.player2.id, p.player2);
        }
        if (p.player3.id != 0)
        {
            playerChoices.Add(p.player3.id, p.player3);
        }
        if (p.player4.id != 0)
        {
            playerChoices.Add(p.player4.id, p.player4);
        }
        if (p.player5.id != 0)
        {
            playerChoices.Add(p.player5.id, p.player5);
        }
        if (p.player6.id != 0)
        {
            playerChoices.Add(p.player6.id, p.player6);
        }
        if (p.player7.id != 0)
        {
            playerChoices.Add(p.player7.id, p.player7);
        }
        if (p.player8.id != 0)
        {
            playerChoices.Add(p.player8.id, p.player8);
        }
        UpdateUI();

    }
    public void UpdateUI()
    {
        //Debug.Log("Spelare i listan "+playerChoices.Count);
        LobbyGUI.UpdateGUI(playerChoices);
        if (isServer)
        {
            foreach (KeyValuePair<int, PlayerInfo> entry in playerChoices) {
                if (!entry.Value.isReady)
                {
                    return;
                }
            }
            //Debug.Log("All are ready");
            NetworkServer.SendToAll(startGame, new IntegerMessage());
            //Debug.Log("Server is starting game");
            StartGame();
            FindObjectOfType<DominationPoint>().GetComponent<DominationPoint>().InitiateDomination();
        }
        
    }


    public void ReceivePlayerDataFromClient(NetworkMessage msg)
    {
        PlayerInfo pi = msg.ReadMessage<PlayerInfoMessage>().p;
        //Här måste vi kolla att spelaren får göra detta.
        foreach (KeyValuePair<int, PlayerInfo> entry in playerChoices)
        {
            if(entry.Value.chosenSlot==pi.chosenSlot && entry.Value.id != pi.id)
            {
                return;
            }
        }
        playerChoices[pi.id] = pi;
        SendClientData();
        UpdateUI();
    }
    public void SendDataFromClient()
    {
        PlayerInfo pi = new PlayerInfo();
        pi.id = myNetID;
        pi.name = playerName;
        pi.team = team;
        pi.chosenSlot = chosenSlot;
        pi.isReady = isReady;
        PlayerInfoMessage pmsg = new PlayerInfoMessage();
        pmsg.p = pi;
        myClient.Send(recDataClient, pmsg);
    }
    public void SendClientData()
    {
        Players players = new Players();
        int i = 1;
        foreach(KeyValuePair<int,PlayerInfo> entry in playerChoices)
        {
            if(i == 1)
            {
                players.player1 = entry.Value;
            }
            if (i == 2)
            {
                Debug.Log("position 2 " + entry.Value.name);
                players.player2 = entry.Value;
            }
            if (i == 3)
            {
                players.player3 = entry.Value;
            }
            if (i == 4)
            {
                players.player4 = entry.Value;
            }
            if (i == 5)
            {
                players.player5 = entry.Value;
            }
            if (i == 6)
            {
                players.player6 = entry.Value;
            }
            if (i == 7)
            {
                players.player7 = entry.Value;
            }
            if (i == 8)
            {
                players.player8 = entry.Value;
            }
            i++;
        }
        PlayerData msg = new PlayerData();
        msg.p = players;
        NetworkServer.SendToAll(recDataServ, msg);
    }

    public void ReceiveTestMessageFromClient(NetworkMessage msg)
    {
        //Debug.Log(msg.ReadMessage<StringMessage>().value);
        Players p = msg.ReadMessage<PlayerData>().p;
        Debug.Log(p.player1.name);
        Debug.Log(p.player2.name);
        //Debug.Log(NetworkManager.singleton.matchSize);
    }
    //public void ReceiveTestMessage(NetworkMessage msg)
    //{
    //    Debug.Log(msg.ReadMessage<StringMessage>().value);
    //}

    public void ReceiveInitClient(NetworkMessage msg)
    {
        PlayerInfo pi = msg.ReadMessage<PlayerInfoMessage>().p;
        pi.name = "Player" + numberOfPlayers;
        IntegerMessage intmessage = new IntegerMessage();

        intmessage.value = numberOfPlayers;
        numberOfPlayers++;
        playerChoices.Add(pi.id, pi);
       
       
        NetworkServer.SendToClient(msg.conn.connectionId, confirmInit, intmessage);
        SendClientData();
        UpdateUI();

    }
    public void RecInitConfirm(NetworkMessage msg)
    {
        int playerNo = msg.ReadMessage<IntegerMessage>().value;
        playerName = "Player" + playerNo;
        
    }



    

    public byte ChosenSlot
    {
        get
        {
            return chosenSlot;
        }

        set
        {
            chosenSlot = value;
        }
    }

    public int MyNetID
    {
        get
        {
            return myNetID;
        }

        set
        {
            myNetID = value;
        }
    }

    public string PlayerName
    {
        get
        {
            return playerName;
        }

        set
        {
            playerName = value;
        }
    }

    public bool IsReady
    {
        get
        {
            return isReady;
        }

        set
        {
            isReady = value;
        }
    }

    public byte Team
    {
        get
        {
            return team;
        }

        set
        {
            team = value;
        }
    }
}


public struct PlayerInfo
{
    public int id;
    public string name;
    public byte chosenSlot;
    public byte team;
    public bool isReady;
}
