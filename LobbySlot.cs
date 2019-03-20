using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlot : MonoBehaviour {


    
    public GameObject ReadyButton;
    public GameObject UnreadyButton;
    public GameObject TakeSlotButton;
    public Text PlayerNameText;
    public Text ReadyText;

    public byte SlotNumber;

    [HideInInspector]
    public LobbyGUI lobbyGUI;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SlotLocalPlayer(PlayerInfo pI)
    {
        TakeSlotButton.SetActive(false);
        ReadyButton.SetActive(!pI.isReady);
        UnreadyButton.SetActive(pI.isReady);
        PlayerNameText.text = pI.name;
    }

    public void SlotNotLocalPlayer(PlayerInfo pI)
    {
        TakeSlotButton.SetActive(false);
        ReadyButton.SetActive(false);
        UnreadyButton.SetActive(false);
        PlayerNameText.text = pI.name;
    }

    public void SlotEmpty()
    {
        TakeSlotButton.SetActive(true);
        ReadyButton.SetActive(false);
        UnreadyButton.SetActive(false);
        PlayerNameText.text = "Empty";
    }

    public void OnClickReady()
    {
        lobbyGUI.OnClickReady();
   
    }
    public void OnClickUnready()
    {
        lobbyGUI.OnClickUnready();
 
    }


    public void OnReady()
    {
        ReadyButton.SetActive(false);
        UnreadyButton.SetActive(true);
    }

    public void OnUnready()
    {
        UnreadyButton.SetActive(false);
        ReadyButton.SetActive(true);
    }

    public void OnClickTakeSlot()
    {
        lobbyGUI.OnClickTakeSlot(SlotNumber);
    }

    public void SetBlank()
    {
        TakeSlotButton.SetActive(false);
        ReadyButton.SetActive(false);
        UnreadyButton.SetActive(false);
        ReadyText.text = "";
        PlayerNameText.text = "";
    }

}
