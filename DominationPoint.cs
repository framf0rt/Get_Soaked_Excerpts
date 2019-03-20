using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DominationPoint : NetworkBehaviour
{

    [SyncVar]
    int team1;
    [SyncVar]
    int team2;
    private DominationState state, previousState;
    public Text dominationStanding;
    public Text ObjectiveText;
    public Text status, objectivesWon;
    public GameObject Point1TeamA, Point2TeamA;
    public GameObject Point1TeamB, Point2TeamB;
    public Image MiddleCircle;
    public Color Orange;
    public Color Blue;
    public Color NoneOwned;
    private float objectiveTextDuration;
    private float captureTimer = 0f;
    private float timeToCapture = 3f;
    private bool aIsCapturing, bIsCapturing;
    private delegate void StatePointer();
    private StatePointer sp;
    private float pointTimer;
    private float pointTeamA, pointTeamB;
    private int ScorePointA, ScorePointB;
    private float waitTimer;
    private byte winConidtion = 100;
    public Transform[] dominationPads;
    public GameObject animatedDom;
    private System.Random rnd;
    private List<DominationPadInfo> padList = new List<DominationPadInfo>();
    public Canvas domCanvas;
    private bool matchOver;
    private float lobbyTimer;
    private float timeUntilLobby = 5f;
    private int scoreTeamA, ScoreTeamB;
    private Material domMat;
    private Color currentCaptureColor;
    private Image maskImage;
    private bool teamAIsWinningPlayed;
    private bool teamBIsWinningPlayed;
    private bool firstPointActivated;
    private Dictionary<NetworkInstanceId, NetworkIdentity> players = new Dictionary<NetworkInstanceId, NetworkIdentity>();




    public Slider Slider;
    public Image SliderImage;
    public Image OwnerImage;

    private struct DominationPadInfo
    {
        public Vector3 pos;
        public Image icon;
        public byte listNumber;
    }

    private enum DominationState
    {
        Even = 0, Team1 = 1, Team2 = 2, Team1Capturing = 4, Team2Capturing = 8, Contested = 16, Waiting = 32, MatchOver = 64
    }

    private void Start()
    {
        
      
        domMat = animatedDom.GetComponent<MeshRenderer>().material;
        maskImage = domCanvas.transform.GetChild(0).GetComponent<Image>();
        rnd = new System.Random(System.DateTime.Now.Millisecond);

        Point1TeamA.SetActive(false);
        Point2TeamA.SetActive(false);
        Point1TeamB.SetActive(false);
        Point2TeamB.SetActive(false);

        for (int i = 0; i < dominationPads.Length; i++)
        {
            DominationPadInfo info = new DominationPadInfo();
            info.pos = dominationPads[i].position;
            info.icon = dominationPads[i].GetComponent<Image>();
            info.listNumber = (byte)i;
            padList.Add(info);
        }
        GetComponent<CapsuleCollider>().enabled = false;
        sp = new StatePointer(StartState);
        //objectivesWon.text = "A= " + ScorePointA + " / " + "B= " + ScorePointB;

       
        domMat.SetFloat("_NeutralEvent", 1f);
        OwnerImage.color = NoneOwned;
        MiddleCircle.color = NoneOwned;

       


    }

    public void InitiateDomination()
    {
        if (isServer)
        {
   
            int currentPad = rnd.Next(0, padList.Count - 1);
            DominationPadInfo info = padList[currentPad];
            transform.position = info.pos;
            maskImage.sprite = info.icon.sprite;
            padList.RemoveAt(currentPad);
            GetComponent<CapsuleCollider>().enabled = true;
            InvokeRepeating("SetPoints", 1f, 0.5f);
            RpcActivatePoint(info.listNumber);
        }
    }


    void Update()
    {
        if (isServer)
        {
            sp();


        }
        if (DominationState.Team1Capturing == state || DominationState.Team2Capturing == state)
        {
            captureTimer += Time.deltaTime;
            Slider.value = captureTimer;
            domMat.SetFloat("_Capturing", 1f);
            //if (currentCaptureColor == Orange)
            //{
            //    domMat.SetFloat("_ColorChange", captureTimer);
            //}
            //else
            //{
            //    domMat.SetFloat("_ColorChange", 1 - captureTimer);
            //}
        }
        if (matchOver)
        {
            lobbyTimer += Time.deltaTime;
            if (isServer)
            {
                if (lobbyTimer > (timeUntilLobby + 2f))
                {
                    NetworkManager.singleton.StopHost();
                    NetworkManager.singleton.StopMatchMaker();
                }
            }
            else
            {
                if (lobbyTimer > timeUntilLobby)
                {
                    NetworkManager.singleton.StopClient();
                    NetworkManager.singleton.StopMatchMaker();
                    
                }
            }
        }

        //if (state == (byte)DominationState.Team2)
        //{

        //    Slider.value += Time.deltaTime; // Räkna upp till maxvärdet
        //}
        //if (state == (byte)DominationState.Team1)
        //{
        //    Slider.value += Time.deltaTime; // Räkna upp till maxvärdet
        //}

        if (objectiveTextDuration < 0f)
        {
            ObjectiveText.text = "";
        }
        else
        {
            objectiveTextDuration -= Time.deltaTime;
        }

    }

    public void SetObjectiveText(string s, float duration)
    {
        ObjectiveText.text = s;
        objectiveTextDuration = duration;
    }

    public void HideCapturing()
    {
        SliderImage.color = Color.clear;
    }

    public void Captured(Color color)
    {
        OwnerImage.color = color;
        MiddleCircle.color = color;
    }

    public void Capturing(Color color) // Change color of capturing slider
    {
        captureTimer = 0f;
        Slider.value = 0;

        SliderImage.color = color;
        Slider.maxValue = timeToCapture; // DETTA MÅSTE VARA SAMMA SOM TIMERN RÄKNAR UPP TILL
    }



    private void SetPoints()
    {
        if((pointTeamA/winConidtion) > 0.8f)
        {
            if (!teamAIsWinningPlayed)
            {

                teamAIsWinningPlayed = true;
                players = NetworkServer.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        NetworkServer.FindLocalObject(entry.Key).GetComponent<VoiceActing>().TeamWinning(0);
                    }
                }
            }
            
        }
        if ((pointTeamB / winConidtion) > 0.8f)
        {
            if (!teamBIsWinningPlayed)
            {

                teamBIsWinningPlayed = true;
                players = NetworkServer.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        NetworkServer.FindLocalObject(entry.Key).GetComponent<VoiceActing>().TeamWinning(1);
                    }
                }
            }

        }
        RpcSetPoints(pointTeamA, pointTeamB);
    }

    private void StartState()
    {
        captureTimer += Time.deltaTime;
        if (team1 > team2)
        {
            bIsCapturing = false;
            if (!aIsCapturing)
            {
                aIsCapturing = true;
                captureTimer = 0f;
                state = DominationState.Team1Capturing;
                RpcSetState((byte)DominationState.Team1Capturing);
            }
            if (captureTimer > timeToCapture)
            {
                sp = new StatePointer(TeamAState);
                RpcSetState((byte)DominationState.Team1);
                state = DominationState.Team1;
                aIsCapturing = false;
                bIsCapturing = false;
                return;

            }
        }
        if (team2 > team1)
        {
            aIsCapturing = false;
            if (!bIsCapturing)
            {
                bIsCapturing = true;
                captureTimer = 0f;
                RpcSetState((byte)DominationState.Team2Capturing);
                state = DominationState.Team2Capturing;
            }
            if (captureTimer > timeToCapture)
            {
                sp = new StatePointer(TeamBState);
                state = DominationState.Team2;
                RpcSetState((byte)DominationState.Team2);
                aIsCapturing = false;
                bIsCapturing = false;
                return;
            }
        }
        if (team1 == team2 && team1 == 0 && state != DominationState.Even)
        {
            captureTimer = 0f;
            aIsCapturing = false;
            bIsCapturing = false;
            state = DominationState.Even;
            RpcSetState((byte)DominationState.Even);

        }
        if (team1 == team2 && team1 != 0 && state != DominationState.Contested)
        {
            captureTimer = 0f;
            aIsCapturing = false;
            bIsCapturing = false;
            state = DominationState.Contested;
            RpcSetState((byte)DominationState.Contested);
        }

    }
    [ClientRpc]
    public void RpcQuitGame()
    {
        if (!isServer)
        {
            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;
            //#endif
            //            Application.Quit();
            NetworkManager.singleton.StopClient();
        }

    }
    private void TeamAState()
    {
        if (!bIsCapturing && state != DominationState.Contested)
        {
            pointTimer += Time.deltaTime;
        }
        //if (pointTimer >= 1f)
        //{
        //    pointTeamA++;
        //    pointTimer = 0f;
        //}
        if (state != DominationState.Contested)
        {
            pointTeamA += Time.deltaTime;
        }
        
        captureTimer += Time.deltaTime;
        if (team1 > team2 && state != DominationState.Team1)
        {
            state = DominationState.Team1;
            RpcSetState((byte)state);
        }
        if (team2 > team1)
        {

            if (!bIsCapturing)
            {
                bIsCapturing = true;
                captureTimer = 0f;
                RpcSetState((byte)DominationState.Team2Capturing);
                state = DominationState.Team2Capturing;
            }
            if (captureTimer > timeToCapture)
            {
                sp = new StatePointer(TeamBState);
                pointTimer = 0f;
                RpcSetState((byte)DominationState.Team2);
                bIsCapturing = false;
                aIsCapturing = false;

                return;
            }
        }
        if (team1 == team2 && team1 == 0 && state != DominationState.Team1)
        {
            captureTimer = 0f;

            bIsCapturing = false;
            state = DominationState.Team1;
            RpcSetState((byte)DominationState.Team1);

        }
        if (team1 == team2 && team1 != 0 && state != DominationState.Contested)
        {
            captureTimer = 0f;

            bIsCapturing = false;
            state = DominationState.Contested;
            RpcSetState((byte)DominationState.Contested);
        }
        if (pointTeamA >= winConidtion)
        {
            sp = new StatePointer(WaitingForNextState);
            state = DominationState.Waiting;
            RpcGivePointToTeam(0);
            RpcSetState((byte)state);
            ScorePointA++;
            if (ScorePointA == 1)
            {
                //tema0 vann första pointen
                players = NetworkServer.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        NetworkServer.FindLocalObject(entry.Key).GetComponent<VoiceActing>().PointCaptured(0);
                    }
                }
            }

            if (CheckVictory(ScorePointA))
            {

                status.text = "Team A won!";
                RpcTeamWins(0);
                matchOver = true;
            }
            aIsCapturing = false;
            bIsCapturing = false;
            captureTimer = 0f;
            pointTeamA = 0;
            pointTeamB = 0;
        }
    }
    private void TeamBState()
    {

        if (!aIsCapturing && state != DominationState.Contested)
        {
            pointTimer += Time.deltaTime;
        }
        //if (pointTimer >= 1f)
        //{
        //    pointTeamB++;
        //    pointTimer = 0f;
        //}
        if (state != DominationState.Contested)
        {
            pointTeamB += Time.deltaTime;
        }
        //pointTeamB += Time.deltaTime;
        captureTimer += Time.deltaTime;
        if (team2 > team1 && state != DominationState.Team2)
        {
            state = DominationState.Team2;
            RpcSetState((byte)state);
        }
        if (team1 > team2)
        {
            if (!aIsCapturing)
            {
                aIsCapturing = true;
                captureTimer = 0f;
                RpcSetState((byte)DominationState.Team1Capturing);
                state = DominationState.Team1Capturing;
            }
            if (captureTimer > timeToCapture)
            {
                sp = new StatePointer(TeamAState);
                pointTimer = 0f;
                RpcSetState((byte)DominationState.Team1);
                bIsCapturing = false;
                aIsCapturing = false;

                return;
            }
        }
        if (team1 == team2 && team1 == 0 && state != DominationState.Team2)
        {
            captureTimer = 0f;

            aIsCapturing = false;
            state = DominationState.Team2;
            RpcSetState((byte)DominationState.Team2);

        }
        if (team1 == team2 && team1 != 0 && state != DominationState.Contested)
        {
            captureTimer = 0f;

            aIsCapturing = false;
            state = DominationState.Contested;
            RpcSetState((byte)DominationState.Contested);
        }
        if (pointTeamB >= winConidtion)
        {
            sp = new StatePointer(WaitingForNextState);
            state = DominationState.Waiting;
            RpcSetState((byte)state);
            RpcGivePointToTeam(1);
            ScorePointB++;
            if (ScorePointB == 1)
            {
                //tema0 vann första pointen
                players = NetworkServer.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        NetworkServer.FindLocalObject(entry.Key).GetComponent<VoiceActing>().PointCaptured(1);
                    }
                }
            }
            if (CheckVictory(ScorePointB))
            {

                RpcTeamWins(1);
                status.text = "Team B won!";
                matchOver = true;
            }
            aIsCapturing = false;
            bIsCapturing = false;
            captureTimer = 0f;
            pointTeamA = 0;
            pointTeamB = 0;
        }
    }

    private bool CheckVictory(int team)
    {
        if (team >= 2)
        {
            return true;
        }
        return false;
    }


    private void WaitingForNextState()
    {
        if (isServer && !matchOver)
        {

            waitTimer += Time.deltaTime;
            if (waitTimer > 5f)
            {

                waitTimer = 0f;
                sp = new StatePointer(StartState);
              
                int currentPad = rnd.Next(0, padList.Count - 1);
                DominationPadInfo info = padList[currentPad];
                transform.position = info.pos;
                maskImage.sprite = info.icon.sprite;
                padList.RemoveAt(currentPad);
                //GetComponent<MeshRenderer>().enabled = true;
                //GetComponent<CapsuleCollider>().enabled = true;
                resetPlayersOnPad();
                players = NetworkServer.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        NetworkServer.FindLocalObject(entry.Key).GetComponent<VoiceActing>().NextCapPoint();
                    }
                }
                RpcActivatePoint(info.listNumber);

            }
        }
    }


    private void resetPlayersOnPad()
    {
        team1 = 0;
        team2 = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isServer)
            {
                //Debug.Log(other.GetComponent<NetworkIdentity>().netId);
                //if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
                //{
                //    CmdSendDominationInfo(0, 0);
                //}
                //else
                //{

                //    CmdSendDominationInfo(0, 1);
                //}
                if (other.GetComponent<PlayerSyncData>().GetTeam() == 0)
                {
                    CmdSendDominationInfo(0, 0);
                }
                else
                {
                    CmdSendDominationInfo(0, 1);
                }
            }
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isServer)
            {
                //if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
                //{
                //    CmdSendDominationInfo(1, 0);
                //}else
                //{
                //    CmdSendDominationInfo(1, 1);
                //} 

                if (other.GetComponent<PlayerSyncData>().GetTeam() == 0)
                {
                    CmdSendDominationInfo(1, 0);
                }
                else
                {
                    CmdSendDominationInfo(1, 1);
                }
            }
        }
    }






    [Command]
    public void CmdSendDominationInfo(byte enterExitInfo, byte teamInfo)
    {



        if (enterExitInfo == 0)
        {
            if (teamInfo == 0)
            {

                team1++;

            }
            else
            {

                team2++;

            }

        }
        if (enterExitInfo == 1)
        {
            if (teamInfo == 0)
            {
                team1--;

            }
            else
            {
                team2--;

            }

        }


        //Tar hand om information
    }
    [ClientRpc]
    public void RpcTeamWins(int team)
    {


        if (team == 0)
        {
            SetObjectiveText("<color=#FF7800FF>" + "Orange Team Wins!" + "</color>", 30);
            //ObjectiveText.text = "<color=#FF7800FF>" + "Orange Team Wins!" + "</color>";
        }
        if (team == 1)
        {
            SetObjectiveText("<color=#41AFCFFF>" + "Blue Team Wins!" + "</color>", 30);
            //ObjectiveText.text = "<color=#41AFCFFF>" + "Blue Team Wins!" + "</color>";
        }
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in p)
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer){

                player.GetComponent<ScoreKeeper>().SaveFinalScore(team, true);
                player.GetComponent<VoiceActing>().TeamWon(team);
            }
            
        }
        matchOver = true;



    }

   

    [ClientRpc]
    private void RpcSetPoints(float teamA, float teamB)
    {
        
        if (state == DominationState.Team1)
        {
      
            int t = (int)Mathf.Round((teamA / winConidtion) * 100);
            if (!isServer)
            {
                if (t > 100)
                {
                    if (!teamAIsWinningPlayed)
                    {
                        teamAIsWinningPlayed = true;
                        players = ClientScene.objects;
                        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                        {
                            if (entry.Value.isLocalPlayer)
                            {
                                ClientScene.FindLocalObject(entry.Key).GetComponent<VoiceActing>().TeamWinning(0);
                            }
                        }
                    }

                   
                }
            }
            dominationStanding.text = t.ToString() + "%";
        }
        else if (state == DominationState.Team2)
        {
           
           
            int t = (int)Mathf.Round((teamB / winConidtion) * 100);
            if (!isServer)
            {
                if (t > 100)
                {
                    if (!teamBIsWinningPlayed)
                    {
                        teamBIsWinningPlayed = true;
                        players = ClientScene.objects;
                        foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                        {
                            if (entry.Value.isLocalPlayer)
                            {
                                ClientScene.FindLocalObject(entry.Key).GetComponent<VoiceActing>().TeamWinning(1);  
                            }
                        }
                    }


                }
            }
            dominationStanding.text = t.ToString() + "%"; 
        }
        else
        {
            dominationStanding.text = "-";
        }

    }
    [ClientRpc]
    public void RpcSetState(byte newstate)
    {

        if (newstate == (byte)DominationState.Even)
        {
            OwnerImage.color = NoneOwned;
            MiddleCircle.color = NoneOwned;
            domMat.SetFloat("_NeutralEvent", 1f);
            domMat.SetFloat("_Capturing", 0f);
            state = DominationState.Even;
            status.text = "Objective A Active";
            SetObjectiveText("New objective active", 3);
            HideCapturing();
        }
        if (newstate == (byte)DominationState.Contested)
        {

            state = DominationState.Contested;
            status.text = "Contested";
            domMat.SetFloat("_NeutralEvent", 1f);
            domMat.SetFloat("_Capturing", 1f);
            HideCapturing();
        }
        if (newstate == (byte)DominationState.Team1)
        {
            state = DominationState.Team1;
            status.text = "Team A has point A";
            SetObjectiveText("Orange team has captured the point", 3);
            domMat.SetFloat("_ColorChange", 1f);
            domMat.SetFloat("_Capturing", 0f);
            domMat.SetFloat("_NeutralEvent", 0f);
            Captured(Orange);
            HideCapturing();

        }
        if (newstate == (byte)DominationState.Team1Capturing)
        {
            state = DominationState.Team1Capturing;
            status.text = "Team A is Capturing point A";
            currentCaptureColor = Orange;
            domMat.SetFloat("_Capturing", 1f);
            Capturing(Orange);
        }
        if (newstate == (byte)DominationState.Team2)
        {
            state = DominationState.Team2;
            //status.text = "Team B has point A";
            SetObjectiveText("Blue team has captured the point", 3);
            domMat.SetFloat("_ColorChange", 0f);
            domMat.SetFloat("_NeutralEvent", 0f);
            domMat.SetFloat("_Capturing", 0f);
            Captured(Blue);
            HideCapturing();
        }
        if (newstate == (byte)DominationState.Team2Capturing)
        {
            state = DominationState.Team2Capturing;
            status.text = "Team B is Capturing point A";
            currentCaptureColor = Blue;
            domMat.SetFloat("_Capturing", 1f);
            Capturing(Blue);
        }
        if (newstate == (byte)DominationState.Waiting)
        {
            state = DominationState.Waiting;
            OwnerImage.color = NoneOwned;
            MiddleCircle.color = NoneOwned;
            domMat.SetFloat("_NeutralEvent", 1f);
            domMat.SetFloat("_Capturing", 0f);
            animatedDom.SetActive(false);
            status.text = "Waiting for next Domination Point";
            HideCapturing();
        }
    }

    [ClientRpc]
    private void RpcGivePointToTeam(byte team)
    {
        if (!isServer)
        {


            if (team == 0)
            {
                ScorePointA++;
            }
            else
            {
                ScorePointB++;
            }
        }
        if (ScorePointA == 1 && team == 0)
        {
            players = ClientScene.objects;
            foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
            {
                if (entry.Value.isLocalPlayer)
                {
                    ClientScene.FindLocalObject(entry.Key).GetComponent<VoiceActing>().PointCaptured(team);
                }
            }
            //team0 vann första pointen
        }
        if (ScorePointB == 1 && team == 1)
        {
            players = ClientScene.objects;
            foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
            {
                if (entry.Value.isLocalPlayer)
                {
                    ClientScene.FindLocalObject(entry.Key).GetComponent<VoiceActing>().PointCaptured(team);
                }
            }
            //team1 vann första pointen
        }
        domCanvas.enabled = false;
        //GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        //transform.position = new Vector3(50f, -1.65f, 31f);
        //objectivesWon.text = "A= " + ScorePointA + " / " + "B= " + ScorePointB;
        if (ScorePointA == 1)
        {
            Point1TeamA.SetActive(true);
        }
        if (ScorePointA == 2)
        {
            Point2TeamA.SetActive(true);
        }

        if (ScorePointB == 1)
        {
            Point1TeamB.SetActive(true);
        }
        if (ScorePointB == 2)
        {
            Point2TeamB.SetActive(true);
        }
    }
    [ClientRpc]
    private void RpcActivatePoint(byte listNumber)
    {
        if (!isServer)
        {
            if (firstPointActivated)
            {

                
                players = ClientScene.objects;
                foreach (KeyValuePair<NetworkInstanceId, NetworkIdentity> entry in players)
                {
                    if (entry.Value.isLocalPlayer)
                    {
                        ClientScene.FindLocalObject(entry.Key).GetComponent<VoiceActing>().NextCapPoint();
                    }
                }
            }else
            {
                firstPointActivated = true;
            }
            for (int i = 0; i < padList.Count; i++)
            {
                if (i == listNumber)
                {
                    Debug.Log("Sätter position lokalt");
                    transform.position = padList[i].pos;
                    maskImage.sprite = padList[i].icon.sprite;
                }
            }
        }

        animatedDom.SetActive(true);
        domCanvas.enabled = true;
        //GetComponent<MeshRenderer>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
    }

}
