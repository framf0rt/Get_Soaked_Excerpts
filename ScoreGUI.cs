using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGUI : MonoBehaviour
{
    public GameObject scoreGUI;
    public GameObject[] team1 = new GameObject[4];
    public GameObject[] team2 = new GameObject[4];
    private PlayerScoreEntry[] team1Score = new PlayerScoreEntry[4];
    private PlayerScoreEntry[] team2Score = new PlayerScoreEntry[4];
    private CharacterInput input;
    private List<PlayerScore> scores = new List<PlayerScore>();
    private bool isShowing;


    // Use this for initialization
    void Start()
    {
        
        
        OnClose();
        input = GetComponentInParent<CharacterInput>();
        
        int i = 0;
        foreach (GameObject t in team1)
        {
            team1Score[i] = team1[i].GetComponent<PlayerScoreEntry>();
            i++;

        }
        int x = 0;
        foreach (GameObject t in team2)
        {
            team2Score[x] = team2[x].GetComponent<PlayerScoreEntry>();
            x++;

        }
        SetScoreInfo();

    }

    // Update is called once per frame
    void Update()
    {
        
		//if (firstSyncDone&&firstUpdate) {
		//	firstSyncDone = false;
		//	OnClose ();

		//}
		//if (firstSyncDone && !firstUpdate) {
		//	OnOpen ();
		//	firstUpdate=true;

		//}


        if(input.ShowScore &&!isShowing)
        {
            isShowing = true;
         
            OnOpen();
          
        }
        if(!input.ShowScore)
        {
            isShowing = false;
            OnClose();
        }
    }

    public void UpdateGUI(List<PlayerScore> scores)
    {
        this.scores = scores;
        SetScoreInfo();
        


    }

    private void SetScoreInfo()
    {
        //      Debug.Log("händer");
        //      if (!firstSyncDone&&!firstUpdate) {
        //	firstSyncDone = true;
        //}
      
        int i = 0;
        foreach (GameObject t in team1)
        {
            team1Score[i].SetEmpty();
            i++;

        }
        int x = 0;
        foreach (GameObject t in team2)
        {
            team2Score[x].SetEmpty();
            x++;

        }

        int team1Pos = 0;
        int team2Pos = 0;
        
        foreach (PlayerScore s in scores)
        {

            float kills = (float)s.kills;
            float deaths = (float)s.deaths;
            float kd = kills;
            if (deaths != 0)
            {
                kd = kills / deaths;
            }

            if (s.team == 0)
            {
                
                team1Score[team1Pos].SetScore(s.playerName, s.kills, s.deaths, kd);
                team1Pos++;
            }
            if (s.team == 1)
            {

                team2Score[team2Pos].SetScore(s.playerName, s.kills, s.deaths, kd);
                team2Pos++;
            }
            
        }
     
        
    }

    public void OnOpen()
    {
        SetScoreInfo();
        scoreGUI.SetActive(true);
        
    }

    public void OnClose()
    {
        scoreGUI.SetActive(false);
    }

}
