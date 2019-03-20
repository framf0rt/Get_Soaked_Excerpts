using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetMenuScript : MonoBehaviour {


    public GameObject PauseMenu;
    public GameObject Settings;
    public GameObject MouseSensPanel;
    public GameObject ControlScheme;
    public GameObject AudioPanel;

    [HideInInspector]
    public CharacterInput input;
    [HideInInspector]
    public WeaponHandler weaponHandler;

    private AdvancedNetworkManager netman;

    // Use this for initialization
    void Start () {
        input = GetComponentInParent<CharacterInput>();
        weaponHandler = GetComponentInParent<WeaponHandler>();
        netman = GameObject.Find("AdvancedNetworkManager").GetComponent<AdvancedNetworkManager>();
        Settings.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if(input.Paused == true)
        {
            PauseMenu.SetActive(true);
            
        }
        else
        {
            PauseMenu.SetActive(false);
           
        }
	}

    public void OnClickDisconnect()
    {
        netman.Disconnect();
    }

    public void OnClickLoadout1()
    {
        weaponHandler.Loadout(WeaponType.Assault, PerkType.IncreasedSplashDmg, 0);
        OnClickClose();
    }

    public void OnClickLoadout2()
    {
        weaponHandler.Loadout(WeaponType.Railgun, PerkType.FasterReload, 0);
        OnClickClose();
    }

    public void OnClickLoadout3()
    {
        weaponHandler.Loadout(WeaponType.Shotgun, PerkType.FasterFirerate, 0);
        OnClickClose();
    }

    public void OnClickClose()
    {
        PauseMenu.SetActive(false);
        input.Unpause();
     
    }

    public void OnClickSettings()
    {
        Settings.SetActive(true);
    }

    public void OnClickBackSettings()
    {
        Settings.SetActive(false);
    }

    public void OnClickControlScheme()
    {
        MouseSensPanel.SetActive(false);
        AudioPanel.SetActive(false);
        ControlScheme.SetActive(true);
    }

    public void OnClickMouseSens()
    {
        ControlScheme.SetActive(false);
        AudioPanel.SetActive(false);
        MouseSensPanel.SetActive(true);
      
    }

    public void OnClickAudioSettings()
    {
        ControlScheme.SetActive(false);
        AudioPanel.SetActive(true);
        MouseSensPanel.SetActive(false);
    }

}
