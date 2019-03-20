using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreEntry : MonoBehaviour {


    public Text playerName;
    public Text kills;
    public Text deaths;
    public Text kd;

	// Use this for initialization
	void Start () {
        this.playerName.text = "";
        this.kills.text = "";
        this.deaths.text = "";
        this.kd.text = "";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    
    public void SetScore(string name, int kills, int deaths, float kd)
    {
        //Debug.Log("Namn " + name + " kills " + kills + " deaths " + deaths +" kd " + kd);
        this.playerName.text = name;
        this.kills.text = kills.ToString();
        this.deaths.text = deaths.ToString();
        this.kd.text = kd.ToString();
    }

    public void SetEmpty()
    {
        
        this.playerName.text = "";
        this.kills.text = "";
        this.deaths.text = "";
        this.kd.text = "";
    }
}
