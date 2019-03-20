using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    public GameObject AutoAim;
    public GameObject RailAim;
    public GameObject ShotgunAim;
    public GameObject AALeft;
    public GameObject AARight;
    public GameObject AAUp;
    public GameObject AADown;

    private Vector3 leftVector1 =new Vector3( 20,0,0);
    private Vector3 leftVector2 = new Vector3(80, 0, 0);
    private Vector3 rightVector1 = new Vector3(-20, 0, 0);
    private Vector3 rightVector2 = new Vector3(-80, 0, 0);
    private Vector3 upVector1 = new Vector3(0, 20, 0);
    private Vector3 upVector2 = new Vector3(0, 80, 0);
    private Vector3 downVector1 = new Vector3(0, -20, 0);
    private Vector3 downVector2 = new Vector3(0,-80, 0);

    // Use this for initialization
    void Start () {
        SetRailAimActive();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetAutoAimActive()
    {
        AutoAim.SetActive(true);
        RailAim.SetActive(false);
        ShotgunAim.SetActive(false);
    }

    public void SetRailAimActive()
    {
        AutoAim.SetActive(false);
        RailAim.SetActive(true);
        ShotgunAim.SetActive(false);
    }

    public void SetShotgunAimActive()
    {
        AutoAim.SetActive(false);
        RailAim.SetActive(false);
        ShotgunAim.SetActive(true);
    }

    public void SetAutoAimSize(float size)
    {
        AALeft.transform.localPosition = Vector3.Lerp(leftVector1, leftVector2, size);
        AARight.transform.localPosition = Vector3.Lerp(rightVector1, rightVector2, size);
        AAUp.transform.localPosition = Vector3.Lerp(upVector1, upVector2, size);
        AADown.transform.localPosition = Vector3.Lerp(downVector1, downVector2, size);
    }

    public void SetShotgunAimSize(float size)
    {

    }
}
