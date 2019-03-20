using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimScript : MonoBehaviour
{
    public Animator anim;
    public Animator animTeam1;
    public Animator animTeam2;
    private CharacterInput ci;
    private PlayerSyncData ps;
    public SkinnedMeshRenderer[] materials;
    public SkinnedMeshRenderer scubaShades;
    public Material orangeTeam;
    public Material blueTeam;
    public Material orangeScuba;
    public Material blueScuba;
    public Material weaponBlue;
    public Material weaponOrange;
    public SkinnedMeshRenderer weaponTank;
    public SkinnedMeshRenderer mainWeapon;
    public SkinnedMeshRenderer weaponTour;
    public Material weaponTourOrange;
    public Material weaponTourBlue;
    public Material materialWeaponTankBlue;
    public Material materialWeaponTankOrange;
    private Vector3 oldThirdPersonVelocity;
    private Vector3 thirdPersonVelocity;
    private Footsteps footsteps;

    // Use this for initialization
    void Start()
    {
        //anim = this.gameObject.GetComponent<Animator>();
        ci = GetComponent<CharacterInput>();
        ps = GetComponent<PlayerSyncData>();
        footsteps = GetComponent<Footsteps>();

        //temporärt men ska bero på vilket lag man valt
        anim = animTeam1;
    }

    public void SetTeam(int team)
    {

        Material mat = orangeTeam;
        if (team == 0)
        {
            anim = animTeam1;
            animTeam1.gameObject.SetActive(true);
            animTeam2.gameObject.SetActive(false);
            //scubaShades.material = orangeScuba;
            //mat = orangeTeam;
            //weaponTank.material = materialWeaponTankOrange;
            //mainWeapon.material = weaponOrange;
            //weaponTour.material = weaponTourOrange;
        }
        else
        {
            anim = animTeam2;
            animTeam2.gameObject.SetActive(true);
            animTeam1.gameObject.SetActive(false);
            //mat = blueTeam;
            //weaponTank.material = materialWeaponTankBlue;
            //scubaShades.material = blueScuba;
            //mainWeapon.material = weaponBlue;
            //weaponTour.material = weaponTourBlue;
        }
        for (int i = 0; i < materials.Length; i++)
        {
            //materials[i].material = mat;
        }



    }

    public void StartDeathAnimation()
    {
        anim.SetBool("Dead", true);
    }

    public void StopDeathAnimation()
    {
        anim.SetBool("Dead", false);
    }

    // Update is called once per frame
    void Update()
    {
        // movement
        thirdPersonVelocity = Vector3.Lerp(oldThirdPersonVelocity, ps.ThirdPersonVelocity, 0.2f);
        //if(Mathf.Abs(Vector3.Dot(thirdPersonVelocity, oldThirdPersonVelocity)) < 0.9f)
        //{
        //    Debug.Log("Bytte riktning");
        //}
        

        //anim.SetFloat("Vertical", ps.ThirdPersonVelocity.z);
        //anim.SetFloat("Horizontal", ps.ThirdPersonVelocity.x);
        anim.SetFloat("Vertical", thirdPersonVelocity.z);
        anim.SetFloat("Horizontal", thirdPersonVelocity.x);
        // movement upp och ner för trappor, kullar osv samt rörelser för airstate
        //anim.SetFloat("HeightMovement", ps.ThirdPersonVelocity.y);


        //80 är max neråt, 0 framåt -70 max uppåt

        //för airstate:
        if (ps.Jumped3rdPerson)
        {
            //Debug.Log("Sätter jumped i anim scriptet");
            anim.SetTrigger("Jump");
            ps.Jumped3rdPerson = false;
        }
        //Om hopp = if-satsen nedan måste hända framen före grounded blir false:
        //if (input.hopp && grounded){
        //  anim.SetTrigger("Jump");
        float Xvalue = ps.getCamRotation();
        if (Xvalue > 200)
        {
            Xvalue -= 360f; 
        }

        anim.SetFloat("Aim", Xvalue);
        //}

        //Behöver en grounded här:
        anim.SetBool("Grounded", ps.Grounded3rdPerson);
        if (ps.Grounded3rdPerson && ps.ThirdPersonVelocity.x > 0 || ps.ThirdPersonVelocity.y > 0 || ps.ThirdPersonVelocity.x < 0 || ps.ThirdPersonVelocity.y < 0)
        {
           footsteps.ThirdPersonFootstep();
        }
        oldThirdPersonVelocity = thirdPersonVelocity;

    }


}
