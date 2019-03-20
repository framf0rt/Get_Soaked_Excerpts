using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Gun : MonoBehaviour
{
    public Transform cameraObjectTransform;
    [HideInInspector]
    public AudioSource audioSourceShoot;
    public Transform gunEnding;
    public float reloadTime;
    protected float nextFire;
    public float fireRate;
    public int magSize;
    public int MaxAmmo;
    [HideInInspector]
    public int AmmoLeft;
    public float RecoilY;
    public float RecoilX;
    public float dmg;
    public float range;
    public bool hasSpread;
    public float spreadAmount;
    public bool isBurst;
    public bool fire;
    public AudioClip[] audioClips = new AudioClip[5];
    

    [HideInInspector]
    public float FirerateFactor = 1f;
  

    [HideInInspector]
    public bool reloading;
    [HideInInspector]
    public int roundsInMag;

    [HideInInspector]
    public Quaternion spreadAngleX;
    [HideInInspector]
    public Quaternion spreadAngleY;
    [HideInInspector]
    public Vector3 spreadX;
    [HideInInspector]
    public Vector3 spread;
    [HideInInspector]
    public Vector3 fwd;
    [HideInInspector]
    public float aimSize = 0.01f;

    public virtual void Start()
    {
       //cameraObjectTransform = GetComponentInParent<Transform>();
       audioSourceShoot = GetComponent<AudioSource>();
        roundsInMag = magSize;
        AmmoLeft = MaxAmmo;
        FirerateFactor = 1f;

    }

    public virtual void UpdateGun()
    {
        if (aimSize > 0.01f)
        {
            aimSize -= 0.5f * Time.deltaTime;
        } else
        {
            aimSize = 0.01f;
        }
    }

    public Gun( int magSize, Transform cameraObjectTransform, float  aCShoot, float reloadTime, AudioSource audioSourceShoot, float fireRate, float dmg, float range, bool hasSpread, float spreadAmount, bool isBurst )
    {
        this.magSize = magSize;
       
        this.reloadTime = reloadTime;
        this.audioSourceShoot = audioSourceShoot;
        this.fireRate = fireRate;
        this.dmg = dmg;
        this.range = range;
        this.cameraObjectTransform = cameraObjectTransform;
        this.spreadAmount = spreadAmount;
        this.hasSpread = hasSpread;
        this.isBurst = isBurst;
    }

    public virtual Vector3 GetSpread(float aimSize)
    {
        fwd = cameraObjectTransform.TransformDirection(Vector3.forward);
        spreadAngleX = Quaternion.AngleAxis(Random.Range(-spreadAmount * aimSize, spreadAmount * aimSize), new Vector3(0, 1, 0) );
        spreadAngleY = Quaternion.AngleAxis(Random.Range(-spreadAmount * aimSize, spreadAmount * aimSize), new Vector3(1, 0, 0));
        spreadX = spreadAngleX * fwd;
        spread = spreadAngleY * spreadX;
        return spread;
    }

    public virtual string GetAmmoState()
    {
        return roundsInMag.ToString() + "/" + AmmoLeft;
    }


    public virtual void Reload(float length)
    {
       // reloading = true;
       // nextFire = Time.time + length;

    }


    public abstract bool Fire(bool isFiring);


  
}
