using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGUI : MonoBehaviour
{

    public GameObject PlayerName;
    public GameObject Status;
    public GameObject PlayersWithoutTeam;
    public LobbyHandler LobbyHandler;
    public InputField inputField;
    public Text CountDown;
    public GameObject[] LobbySlotsArray = new GameObject[8];
    private float timer = 5.5f;
    private TimerFormat tm = new TimerFormat();
    private bool closeLobby = false;
    private CharacterInput input;
    //private GameObject[] lobbySlotsOccupied = new GameObject[8];

    private Dictionary<int, PlayerInfo> playerChoices = new Dictionary<int, PlayerInfo>();

    private int matchSize = 4;

    // Use this for initialization
    void Start()
    {
        input = GetComponentInParent<CharacterInput>();
        input.Pause();


    }

    public void Update()
    {
        if (closeLobby)
        {
            timer -= Time.deltaTime;
            int i = (int)timer;
            CountDown.text = i.ToString();
            if (timer < 0)
            {
                this.gameObject.SetActive(false);
               
            }
        }
    }


    public void CloseLobby()
    {
        closeLobby = true;

        foreach (KeyValuePair<int, PlayerInfo> entry in playerChoices)
        {

            if (LobbyHandler.MyNetID == entry.Value.id)
            {
                GetComponentInParent<PlayerSyncData>().SetName(entry.Value.name);
                if (entry.Value.chosenSlot == 0 || entry.Value.chosenSlot == 2 || entry.Value.chosenSlot == 4 || entry.Value.chosenSlot == 6)
                {
                    GetComponentInParent<PlayerSyncData>().SetTeam(0);
                 
                }
                else
                {
                    GetComponentInParent<PlayerSyncData>().SetTeam(1);
                
                }
                GetComponentInParent<PlayerSyncData>().SetSlot(entry.Value.chosenSlot);
                
                GetComponentInParent<Health>().CmdStartSpawn();
            }
          
        }
    }

    public void UpdateGUI(Dictionary<int, PlayerInfo> pC)
    {
        if (!closeLobby)
        {
            UpdatePlayersWithoutTeamReset();
            foreach (GameObject lb in LobbySlotsArray)
            {
                lb.GetComponent<LobbySlot>().SlotEmpty();
            }

            for (int i = 0; i < 8; i++)
            {
                if (pC.Count > i)
                {
                    
                    LobbySlotsArray[i].SetActive(true);
                    LobbySlotsArray[i].GetComponent<LobbySlot>().lobbyGUI = this;
                    LobbySlotsArray[i].GetComponent<LobbySlot>().SlotNumber = (byte)i;
                }
                else
                {
                    LobbySlotsArray[i].SetActive(false);
                    //LobbySlotsArray[i].GetComponent<LobbySlot>().lobbyGUI = this;
                    //LobbySlotsArray[i].GetComponent<LobbySlot>().SlotNumber = (byte)i;
                    //LobbySlotsArray[i].GetComponent<LobbySlot>().SetBlank();
                }
            }

            playerChoices = pC;

            foreach (KeyValuePair<int, PlayerInfo> entry in playerChoices)
            {

                if (LobbyHandler.MyNetID == entry.Value.id)
                {
                    PlayerName.GetComponent<Text>().text = entry.Value.name;
                }

                //Debug.Log(entry.Key + " " + entry.Value.chosenSlot + " " + entry.Value.name);
                if (entry.Value.chosenSlot > 8)
                {
                    UpdatePlayersWithoutTeam(entry.Value.name);

                }
                else
                {

                    GameObject lB = LobbySlotsArray[entry.Value.chosenSlot];
                    LobbySlot lobbySlot = lB.GetComponent<LobbySlot>();
                    
                    if (LobbyHandler.MyNetID == entry.Value.id)
                    {
                        
                        lobbySlot.SlotLocalPlayer(entry.Value);
                       
                    }
                    else
                    {
                        lobbySlot.SlotNotLocalPlayer(entry.Value);
                    }

                    lobbySlot.PlayerNameText.text = entry.Value.name;

                }
            }

        }

    }



    public void OnClickChangeName()
    {
        if (!closeLobby)
        {
            PlayerName.GetComponent<Text>().text = inputField.text;
            LobbyHandler.PlayerName = inputField.text;
            Debug.Log("New name: " + inputField.text);
            LobbyHandler.DataChanged();
        }
    }

    public void OnClickTakeSlot(byte slotNumber)
    {
        LobbyHandler.ChosenSlot = slotNumber;
        if (slotNumber == 0 || slotNumber == 2 || slotNumber == 4 || slotNumber == 6)
        {
            LobbyHandler.Team = 0;
        } else
        {
            LobbyHandler.Team = 1;
        }
        
        //Debug.Log("You have switched slot");
        LobbyHandler.DataChanged();
    }

    public void OnClickReady()
    {
        LobbyHandler.IsReady = true;
        LobbyHandler.DataChanged();
    }

    public void OnClickUnready()
    {
        LobbyHandler.IsReady = false;
        LobbyHandler.DataChanged();
    }

    public void UpdatePlayersWithoutTeam(string name)
    {
        string current = PlayersWithoutTeam.GetComponent<Text>().text;
        PlayersWithoutTeam.GetComponent<Text>().text = current + " " + name;
    }

    public void UpdatePlayersWithoutTeamReset()
    {
        PlayersWithoutTeam.GetComponent<Text>().text = "Players without team: ";
    }

    public void UpdateStatus(string Name)
    {

    }
}
