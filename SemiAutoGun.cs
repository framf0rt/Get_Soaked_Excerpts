using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SemiAutoGun : Gun
{



    [HideInInspector]
    public bool isSemiAuto;



    public override void Start()
    {
        base.Start();

    }

    public override void UpdateGun()
    {
        aimSize = 1f;
    }


    public SemiAutoGun(int magSize, Transform cameraObjectTransform, float aCShoot, float aCReload, AudioSource audioSourceShoot, float fireRate, float dmg, float range, bool hasSpread, float spreadAmount, bool isBurst) : base(magSize, cameraObjectTransform, aCShoot, aCReload, audioSourceShoot, fireRate,  dmg,  range, hasSpread, spreadAmount, isBurst)
    {
        roundsInMag = magSize;
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
             
                roundsInMag--;
                isSemiAuto = true;
                nextFire = Time.time + (fireRate * FirerateFactor) ;
                if(hasSpread == true)
                {

                    spread = GetSpread(aimSize);
                }
                else
                {
                    spread = cameraObjectTransform.TransformDirection(Vector3.forward);
                }
                fire = true;
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
                return false;
            }

        }else
        {
            return false;
        }



    }





}
