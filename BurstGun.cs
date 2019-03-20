using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BurstGun : Gun
{



  
    public int burstAmount;
    public bool isShotgun;
    [HideInInspector]
    public bool isSemiAuto;
    
    private Vector3[] spreadBurst = new Vector3[100];

    public override void Start()
    {
        base.Start();
        spreadBurst = new Vector3[burstAmount];
        aimSize = 1f;

    }

    public override void UpdateGun()
    {
        aimSize = 1f;
    }

    public BurstGun(int magSize, Transform cameraObjectTransform, float aCShoot, float aCReload, AudioSource audioSourceShoot, float fireRate, int dmg, float range, bool hasSpread, float spreadAmount, bool isBurst, int burstAmount, bool isShotgun) : base(magSize, cameraObjectTransform, aCShoot, aCReload, audioSourceShoot, fireRate, dmg, range, hasSpread, spreadAmount, isBurst)
    {
        roundsInMag = magSize;
        this.burstAmount = burstAmount;
       
        this.isShotgun = isShotgun;
    }

    // Use this for initialization

 


    public override bool Fire(bool isFiring)
    {
        if (reloading)
        {
            fire = false;
        }

        // FIRE
        if (nextFire < Time.time && !reloading)
        {

            // FIRE
            if (isFiring == true && isSemiAuto == false)
            {
                fire = true;
                if(isShotgun == true)
                {
                    roundsInMag--;
                }
                else
                {
                    roundsInMag -= burstAmount;
                }
                

                isSemiAuto = true;
                nextFire = Time.time + (fireRate * FirerateFactor);
                    for(int i = 0; i < burstAmount; i++)
                    {
                        spreadBurst[i] = base.GetSpread(aimSize);
                    }
            
                return true;
            }
            else if (isFiring == false && isSemiAuto == true)
            {
                isSemiAuto = false;
              
                return false;
                
            }

            else
            {
                fire = false;
                //laserLine.enabled = false;
                return false;
            }

        }
        else
        {
            
            return false;
        }



    }



    public Vector3[] GetSpreadList()
    {
        return spreadBurst;
    }





}
