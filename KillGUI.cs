using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillGUI : MonoBehaviour
{

    //public GameObject[] killFeedObjects = new GameObject[5];
    //public KillFeedText1[] killFeed = new KillFeedText1[5];

    public GameObject killFeedPrefab;
    public float duration;
    [HideInInspector]
    //public List<GameObject> list = new List<GameObject>();

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        byte team = (byte)Random.Range(0, 2);
        byte team2 = (byte)Random.Range(0, 2);
        if (Input.GetKey(KeyCode.F7))
        {
            AddToKillFeed("A" + team, team, "B" + team2, team2, false, "test");
        }

    }

    public string GetRandomKillWord()
    {
        return "soaked";
    }

    public string GetRandomSuicideWord()
    {

        return "screwed up";
    }

    public void AddToKillFeed(string name1, byte team1, string name2, byte team2, bool suicide,string killword)
    {
        if (suicide)
        {
            var killFeed = (GameObject)Instantiate(killFeedPrefab, new Vector3(200, 0, 0), new Quaternion(0, 0, 0, 0));
            killFeed.transform.SetParent(this.transform);

            Text text = killFeed.GetComponent<Text>();
            if (team1 == 0)
            {
                text.text = "<color=#FF7800FF>" + name1 + "</color>" + " " + GetRandomSuicideWord();
            }
            else
            {
                text.text = "<color=#41AFCFFF>" + name1 + "</color>" + " " + GetRandomSuicideWord();
            }
            Destroy(killFeed, duration);
        }
        else
        {

            var killFeed = (GameObject)Instantiate(killFeedPrefab, new Vector3(200, 0, 0), new Quaternion(0, 0, 0, 0));
            killFeed.transform.SetParent(this.transform);

            Text text = killFeed.GetComponent<Text>();
            if (team1 == 0 && team2 == 1) // ORANGE
            {
                text.text = "<color=#FF7800FF>" + name1 + "</color>" + " " + killword + " " + "<color=#41AFCFFF>" + name2 + "</color>";
            }
            else if (team1 == 1 && team2 == 0) // BLUE
            {
                text.text = "<color=#41AFCFFF>" + name1 + "</color>" + " " + killword + " " + "<color=#FF7800FF>" + name2 + "</color>";
            }
            else if ((team1 == 0 && team2 == 0)) // BLUE
            {
                text.text = "<color=#FF7800FF>" + name1 + "</color>" + " " + "helped" + " " + "<color=#FF7800FF>" + name2 + "</color>";
            }
            else if ((team1 == 1 && team2 == 1)) // BLUE
            {
                text.text = "<color=#41AFCFFF>" + name1 + "</color>" + " " + "helped" + " " + "<color=#41AFCFFF>" + name2 + "</color>";
            }
            Destroy(killFeed, duration);
        }
        
    }

}
