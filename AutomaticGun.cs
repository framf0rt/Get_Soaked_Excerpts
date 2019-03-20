using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGun : Gun
{


    private int firedInRepeat;


    public override void Start()
    {
        base.Start();
    }

    public AutomaticGun(int magSize, Transform cameraObjectTransform, float aCShoot, float aCReload, AudioSource audioSourceShoot, float fireRate, float dmg, float range, bool hasSpread, float spreadAmount, bool isBurst) : base(magSize, cameraObjectTransform, aCShoot, aCReload, audioSourceShoot, fireRate, dmg, range, hasSpread, spreadAmount, isBurst)
    {
        roundsInMag = magSize;
    }


    public override void Reload(float length)
    {
        reloading = true;
        firedInRepeat = 0;
        nextFire = Time.time + length;
        //aimSize = 0f;
    }

    public override bool Fire(bool isFiring)
    {

        // RELOAD
        if (reloading)
        {
            fire = false;
        }

        if (nextFire < Time.time && !reloading)
        {

            // Alt-fire
            if (isFiring == true && reloading == false)
            {
                roundsInMag--;
                nextFire = Time.time + (fireRate * FirerateFactor);
                fire = true;
                if (aimSize < 1f)
                {
                    spread = GetSpread(aimSize);
                    aimSize += 0.1f;
                }




                firedInRepeat++;
                return true;

            }
            else
            {
                fire = false;
                firedInRepeat = 0;
                //laserLine.enabled = false;
                return false;
            }

        }
        else
        {
            return false;
        }


    }


}
