using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class WeaponHandler : NetworkBehaviour
{
    // Input
    public CharacterInput input;
    public Aim aim;

    // Camera
    public GameObject cameraObject;

    // UI

    [Header("General Prefabs")]
    public GameObject AmmoVisual;
    public GameObject GrenadeVisual;

    // Prefabs
    public GameObject ParticleShotPrefab;
    public GameObject ParticleShotgunPrefab;
    public GameObject ParticleHitEffectPrefab;
    public GameObject MuzzleEffectPrefab;
    public GameObject GrenadePrefab;

    [HideInInspector]
    public WeaponType ActiveWeapon;
    private LoadoutInfo loadoutInfo;

    [Header("FPS Objects")]
    public GameObject SemiGunPrefabTeam1;
    public GameObject SemiGunPrefabTeam2;
    public GameObject AutoGunPrefabTeam1;
    public GameObject AutoGunPrefabTeam2;
    public GameObject ShotgunPrefabTeam1;
    public GameObject ShotgunPrefabTeam2;
    public SemiAutoGun SemiGunTeam1;
    public AutomaticGun AutoGunTeam1;
    public BurstGun ShotgunTeam1;
    public SemiAutoGun SemiGunTeam2;
    public AutomaticGun AutoGunTeam2;
    public BurstGun ShotgunTeam2;
    public GameObject HitMarker;
    private bool isRecoiling;


    [Header("3P Objects")]
    public GameObject Team1SemiGun;
    public GameObject Team1AutoGun;
    public GameObject Team1Shotgun;
    public GameObject Team2SemiGun;
    public GameObject Team2AutoGun;
    public GameObject Team2Shotgun;
    public bool Reloading3P;
    private float ReloadTime3P;


    [Header("General")]

    // Layermasks
    public LayerMask DefaultPlayer1LayerMask;
    public LayerMask DefaultPlayer2LayerMask;
    private LayerMask CurrentMask;
    public LayerMask PlayerOnlyLayer;

    // Weapon stats
    protected bool reloading;

    // RAYCAST
    protected RaycastHit hit;
    private Vector3 rayOrigin;
    private RaycastHit splash;
    private Dictionary<Collider, int> burstHitList = new Dictionary<Collider, int>();

    // GRENADE
    public Transform grenadeSpawn;
    public int numberOfGrenades;
    private int grenadesLeft;
    public bool isThrowingGrenade;
    public bool grenadeThrown = false;

    private Gun activeGun;
    private Text ammoText;
    private Text grenadeText;

    private PlayerSyncData playerSyncData;
    [HideInInspector]
    public byte CurrentTeam;

    // PERKS
    private float reloadFactor = 1f;
    private float splashDmgFactor = 1f;


    [Header("Sound")]
    private AudioSource hitMarkerAS;
    public AudioClip HitMarkerAC;
    public AudioClip AmmoSoundAC;
    private VoiceActing va;
    private bool firstReload = true;
    [HideInInspector]
    public NetworkInstanceId netID;

    public GameObject SemiGunMesh;
    public GameObject AutoGunMesh;
    public GameObject ShotGunMesh;

    private float summedRecoilY;
    private float summedRecoilX;

   


    public override void OnStartServer()
    {
        base.OnStartServer();
        netID = GetComponent<NetworkIdentity>().netId;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        netID = GetComponent<NetworkIdentity>().netId;
    }

    public void SetupWeaponHandler(byte team)
    {
        if (team == 0)
        {
            CurrentTeam = 0;
            CurrentMask = DefaultPlayer1LayerMask;
        }
        else
        {
            CurrentTeam = 1;
            CurrentMask = DefaultPlayer2LayerMask;
        }
    }

    // Use this for initialization
    void Start()
    {
        va = GetComponent<VoiceActing>();
        playerSyncData = GetComponent<PlayerSyncData>();
        hitMarkerAS = GetComponent<AudioSource>();
        SemiGunPrefabTeam1.SetActive(false);
        AutoGunPrefabTeam1.SetActive(false);
        ShotgunPrefabTeam1.SetActive(false);
        SemiGunPrefabTeam2.SetActive(false);
        AutoGunPrefabTeam2.SetActive(false);
        ShotgunPrefabTeam2.SetActive(false);
        Team1AutoGun.SetActive(false);
        Team2AutoGun.SetActive(false);
        Team1SemiGun.SetActive(false);
        Team1Shotgun.SetActive(false);
        Team2SemiGun.SetActive(false);
        Team2Shotgun.SetActive(false);

        if (!isLocalPlayer)
        {
            SetActiveGun(SemiGunTeam1);
            SetLayerRecursively(ShotgunPrefabTeam1, LayerMask.NameToLayer("Ignore Raycast"));
            SetLayerRecursively(AutoGunPrefabTeam1, LayerMask.NameToLayer("Ignore Raycast"));
            SetLayerRecursively(SemiGunPrefabTeam1, LayerMask.NameToLayer("Ignore Raycast"));
            SetLayerRecursively(ShotgunPrefabTeam2, LayerMask.NameToLayer("Ignore Raycast"));
            SetLayerRecursively(AutoGunPrefabTeam2, LayerMask.NameToLayer("Ignore Raycast"));
            SetLayerRecursively(SemiGunPrefabTeam2, LayerMask.NameToLayer("Ignore Raycast"));
            Team1AutoGun.SetActive(true);
            return;

        }




        HitMarker.SetActive(false);
        grenadesLeft = numberOfGrenades;
        input = GetComponent<CharacterInput>();
        ammoText = AmmoVisual.GetComponent<Text>();
        grenadeText = GrenadeVisual.GetComponent<Text>();

        SemiGunPrefabTeam1.SetActive(true);
        SetActiveGun(SemiGunTeam1);

        AutoGunPrefabTeam1.SetActive(false);
        ShotgunPrefabTeam1.SetActive(false);
        SemiGunPrefabTeam1.SetActive(false);
        SemiGunPrefabTeam2.SetActive(false);
        AutoGunPrefabTeam2.SetActive(false);
        ShotgunPrefabTeam2.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {


        if (!isLocalPlayer)
        {
            if(Time.time > ReloadTime3P)
            {
                Reloading3P = false;
            }

            return;
        }

        if (CurrentTeam == 0)
        {
            if (SemiGunPrefabTeam2.activeSelf)
            {
                SemiGunPrefabTeam1.SetActive(true);
                SemiGunPrefabTeam2.SetActive(false);
            }

            if (AutoGunPrefabTeam2.activeSelf)
            {
                // AutoGunPrefabTeam1.SetActive(true);
                //   AutoGunPrefabTeam2.SetActive(false);
            }
            if (ShotgunPrefabTeam2.activeSelf)
            {
                ShotgunPrefabTeam1.SetActive(true);
                ShotgunPrefabTeam2.SetActive(false);
            }

        }

        if (CurrentTeam == 1)
        {
            if (SemiGunPrefabTeam1.activeSelf)
            {
                SemiGunPrefabTeam2.SetActive(true);
                SemiGunPrefabTeam1.SetActive(false);
            }

            if (AutoGunPrefabTeam1.activeSelf)
            {
                // AutoGunPrefabTeam2.SetActive(true);
                //   AutoGunPrefabTeam1.SetActive(false);
            }
            if (ShotgunPrefabTeam1.activeSelf)
            {
                ShotgunPrefabTeam2.SetActive(true);
                ShotgunPrefabTeam1.SetActive(false);
            }

        }


        if (isThrowingGrenade == true)
        {
            return;
        }
        else if (input.GetGrenadeKey() == true && grenadesLeft > 0)
        {
            ThrowGrenade(GetComponent<CharacterController3D>().movement);
        }
        else
        {

            if (((input.GetReloadKey() == true) || (GetActiveGun().roundsInMag < 1)) && GetActiveGun().roundsInMag < GetActiveGun().magSize && GetActiveGun().reloading == false && GetActiveGun().AmmoLeft > 0)
            {
                float reloadTime = reloadTime = GetActiveGun().reloadTime * reloadFactor;
                GetActiveGun().reloading = true;


                if (isServer)
                {
                    RpcReload3P();
                }
                else
                {
                    CmdReload3P();
                }

                StartCoroutine(ReloadEffect(reloadTime));
                if (firstReload)
                {
                    firstReload = false;
                }
                else
                {
                    va.Reloading();
                }


            }
            else if (GetActiveGun().roundsInMag > 0 && !GetActiveGun().reloading)
            {
                GunHandler();
            }
            if (GetActiveGun().roundsInMag == 0 && GetActiveGun().AmmoLeft == 0 && input.GetMouse0())
            {
                va.OutOfAmmo();
            }

        }

        if (isRecoiling == true)
        {
            Recoil(GetActiveGun().RecoilY, GetActiveGun().RecoilX);
        }


        GetActiveGun().UpdateGun();
        aim.SetAutoAimSize(GetActiveGun().aimSize);


        ammoText.text = GetActiveGun().GetAmmoState();
        grenadeText.text = grenadesLeft.ToString();

    }



    public void SetLayerRecursively(GameObject obj, LayerMask layer)
    {
        obj.gameObject.layer = layer;

        foreach (Transform child in obj.gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public void Loadout(WeaponType weapon, PerkType perk1, byte perk2)
    {
        ResetPerks();
        if (weapon == WeaponType.Assault)
        {
            SetActiveGun(WeaponType.Assault);
        }
        if (weapon == WeaponType.Railgun)
        {
            SetActiveGun(WeaponType.Railgun);
        }
        if (weapon == WeaponType.Shotgun)
        {
            SetActiveGun(WeaponType.Shotgun);
        }

        loadoutInfo = new LoadoutInfo();
        loadoutInfo.weaponType = ActiveWeapon;
        loadoutInfo.weaponPerk = perk1;
        SemiGunTeam1.reloading = false;
        SemiGunTeam2.reloading = false;
        AutoGunTeam1.reloading = false;
        AutoGunTeam2.reloading = false;
        ShotgunTeam1.reloading = false;
        ShotgunTeam2.reloading = false;
        RefillAmmoFull();

    }



    public void ResetPerks()
    {
        ShotgunTeam1.FirerateFactor = 1f;
        SemiGunTeam1.FirerateFactor = 1f;
        AutoGunTeam1.FirerateFactor = 1f;
        reloadFactor = 1f;
        splashDmgFactor = 1f;
        
    }

    public void InstantRecoil(float y, float x)
    {
        float recoilY = (y / 5);
        float recoilX = (x / 5);
        input.MouseY += recoilY;
        input.MouseX += recoilX;
        if (summedRecoilY < 10)
        {
            summedRecoilY += recoilY;
            summedRecoilX += recoilX;
        }

    }

    public void Recoil(float valueY, float valueX)
    {

        float k = Mathf.Lerp(0, valueY / 10, Time.deltaTime * 10);
        input.MouseY -= k;
        summedRecoilY -= k;


        float s = Mathf.Lerp(0, valueX / 10, Time.deltaTime * 10);
        input.MouseX -= s;
        summedRecoilX -= s;

        if (summedRecoilY < 1)
        {
            isRecoiling = false;
        }

    }

    public Gun GetActiveGun()
    {
        return activeGun;
    }

    public void SetActiveGun(Gun gun)
    {
        activeGun = gun;
        if (gun is AutomaticGun)
        {
            aim.SetAutoAimActive();
            Change3PWeapon(WeaponType.Assault);
        }
        if (gun is SemiAutoGun)
        {
            aim.SetRailAimActive();
            Change3PWeapon(WeaponType.Railgun);
        }
        if (gun is BurstGun)
        {
            aim.SetShotgunAimActive();
            Change3PWeapon(WeaponType.Shotgun);
        }
        SemiGunTeam1.reloading = false;
        SemiGunTeam2.reloading = false;
        AutoGunTeam1.reloading = false;
        AutoGunTeam2.reloading = false;
        ShotgunTeam1.reloading = false;
        ShotgunTeam2.reloading = false;

    }
    public void SetActiveGun(WeaponType wT)
    {
        if (wT == WeaponType.Assault)
        {
            if (CurrentTeam == 0)
            {
                AutoGunPrefabTeam1.SetActive(true);
                SemiGunPrefabTeam1.SetActive(false);
                ShotgunPrefabTeam1.SetActive(false);
                SetActiveGun(AutoGunTeam1);
            }
            else
            {
                AutoGunPrefabTeam2.SetActive(true);
                SemiGunPrefabTeam2.SetActive(false);
                ShotgunPrefabTeam2.SetActive(false);
                SetActiveGun(AutoGunTeam2);
            }
            ActiveWeapon = WeaponType.Assault;
        }
        if (wT == WeaponType.Railgun)
        {
            if (CurrentTeam == 0)
            {
                SemiGunPrefabTeam1.SetActive(true);
                AutoGunPrefabTeam1.SetActive(false);
                ShotgunPrefabTeam1.SetActive(false);
                SetActiveGun(SemiGunTeam1);
            }
            else
            {
                SemiGunPrefabTeam2.SetActive(true);
                AutoGunPrefabTeam2.SetActive(false);
                ShotgunPrefabTeam2.SetActive(false);
                SetActiveGun(SemiGunTeam2);
            }
            ActiveWeapon = WeaponType.Railgun;
        }
        if (wT == WeaponType.Shotgun)
        {

            if (CurrentTeam == 0)
            {
                ShotgunPrefabTeam1.SetActive(true);
                SemiGunPrefabTeam1.SetActive(false);
                AutoGunPrefabTeam1.SetActive(false);
                SetActiveGun(ShotgunTeam1);
            }
            else
            {
                ShotgunPrefabTeam2.SetActive(true);
                SemiGunPrefabTeam2.SetActive(false);
                AutoGunPrefabTeam2.SetActive(false);
                SetActiveGun(ShotgunTeam2);
            }

            ActiveWeapon = WeaponType.Shotgun;
        }
        Change3PWeapon(wT);
        RefillAmmoFull();
    }

    public void GunHandler()
    {

        if (GetActiveGun().Fire(input.GetMouse0()))
        {
            if (GetActiveGun() is BurstGun)
            {
                BurstGun bg = (BurstGun)GetActiveGun();
                Vector3[] sList = bg.GetSpreadList();
                StartCoroutine(ShotEffect(0.07f));
                ShootShotgunRay(sList, bg.fwd, GetActiveGun().dmg, GetActiveGun().range);
                if (isServer)
                {
                    RpcSGR();
                }
                else
                {
                    CmdSGR();
                }
            }
            else
            {
                ShootRay(GetActiveGun().spread, GetActiveGun().dmg, GetActiveGun().range);
                StartCoroutine(ShotEffect(0.07f));
                if (isServer)
                {
                    RpcSR(GetActiveGun().spread, GetActiveGun().range);
                }
                else
                {
                    CmdSR(GetActiveGun().spread, GetActiveGun().range);
                }
            }
        }
        else
        {


        }
    }


    public void Splash(Vector3 hit, bool hitPlayer)
    {


        var hitEffect = (GameObject)Instantiate(ParticleHitEffectPrefab, hit, cameraObject.transform.rotation);
        Destroy(hitEffect, 1f);
        if (!hitPlayer)
        {


            Collider[] hitColliders = Physics.OverlapSphere(hitEffect.transform.position, 3, PlayerOnlyLayer);
            if (hitColliders.Length > 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    float dmg = GetActiveGun().dmg / 10;
                    Vector3 raycastDir = hitColliders[i].transform.position - hitEffect.transform.position;
                    Physics.Raycast(hitEffect.transform.position, raycastDir, out splash, CurrentMask);   // CHECK TO SEE IF PLAYER IS BEHIND WALL
                    if (splash.collider != null && splash.collider.tag == "Player" && splash.collider.transform.gameObject.GetComponent<PlayerSyncData>().GetTeam() != CurrentTeam)
                    {
                        //float distanceFromCenter = Vector3.Distance(hitEffect.transform.position, hitColliders[i].gameObject.transform.position);
                        //dmg = dmg / distanceFromCenter;
                        CmdDmg(dmg * splashDmgFactor, splash.collider.transform.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                        StartCoroutine(HitMarkerEffect());
                    }
                }
            }
        }
    }



    public void ShootShotgunRay(Vector3[] spreadArray, Vector3 fwd, float dmg, float range)
    {

        if (isLocalPlayer)
        {
            // Spawn muzzle effect
            var muzzleEffect = (GameObject)Instantiate(MuzzleEffectPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(muzzleEffect, 1f);

            // Spawn shotgun effect
            var shotgunEffect = (GameObject)Instantiate(ParticleShotgunPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(shotgunEffect, 1f);

            for (int i = 0; i < spreadArray.Length - 1; i++)
            {

                rayOrigin = cameraObject.transform.position;

                if (Physics.Raycast(cameraObject.transform.position, spreadArray[i], out hit, range, CurrentMask))
                {

                    if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject.GetComponent<PlayerSyncData>().GetTeam() != CurrentTeam)
                    {
                        StartCoroutine(HitMarkerEffect());
                        if (burstHitList.ContainsKey(hit.collider)) // Check if target has been hit more then once, if hit more then count up with 1. Else add to list
                        {

                            int val = 0;
                            if (burstHitList.TryGetValue(hit.collider, out val))
                            {
                                burstHitList[hit.collider] = val += 1;
                            }
                        }
                        else
                        {
                            burstHitList.Add(hit.collider, 0);
                        }
                    }
                }
            }

            // Go through list and apply damage to everyone hit. Damage equals guns damage times number of times hit
            foreach (KeyValuePair<Collider, int> entry in burstHitList)
            {
                float dis = Vector3.Distance(GetActiveGun().gunEnding.transform.position, entry.Key.transform.position);

                if (dis < 3f)
                {
                    if ((entry.Value * dmg) < 100)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(100, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }

                }
                else if (dis < 10f)
                {
                    if ((entry.Value * dmg) < 90)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(80, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                }
                else if (dis < 15f)
                {
                    if ((entry.Value * dmg) < 75)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(60, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                }
                else if (dis < 20f)
                {
                    if ((entry.Value * dmg) < 55)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(55, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                }
                else if (dis < 25f)
                {
                    if ((entry.Value * dmg) < 30)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(30, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                }
                else
                {
                    if ((entry.Value * dmg) < 20)
                    {
                        CmdDmg((dmg * entry.Value), entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                    else
                    {
                        CmdDmg(20, entry.Key.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    }
                }
            }

            burstHitList.Clear();
            isRecoiling = true;
            InstantRecoil(GetActiveGun().RecoilY, GetActiveGun().RecoilX);
        }
    }



    public void ShootRay(Vector3 direction, float dmg, float range)
    {
        if (isLocalPlayer)
        {

            //Spawn muzzle effect
            var muzzleEffect = (GameObject)Instantiate(MuzzleEffectPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(muzzleEffect, 1f);

            rayOrigin = cameraObject.transform.position;

            if (Physics.Raycast(cameraObject.transform.position, direction, out hit, range, CurrentMask))
            {

                if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject.GetComponent<PlayerSyncData>().GetTeam() != CurrentTeam) // If player hit, apply damage
                {
                    CmdDmg(dmg, hit.transform.gameObject.GetComponent<NetworkIdentity>().netId, transform.position);
                    StartCoroutine(HitMarkerEffect());
                    Splash(hit.point, true);
                }
                else
                {
                    Splash(hit.point, false);
                }

                // Spawn splash effect + Splash damage check


                // Spawn Shoot effect
                SpawnShotEffect(hit.point);



            }
            else // Spawn Shoot effect if nothing is hit
            {
                SpawnShotEffect(rayOrigin + (direction * range));
            }

            isRecoiling = true;
            InstantRecoil(GetActiveGun().RecoilY, GetActiveGun().RecoilX);
        }

    }

    public void SpawnShotEffect(Vector3 endPoint) // Spawns the shoot effect
    {
        Vector3 _direction = (endPoint - GetActiveGun().gunEnding.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        Quaternion rot = Quaternion.Lerp(transform.rotation, _lookRotation, 1);
        var lineEffect = (GameObject)Instantiate(ParticleShotPrefab, GetActiveGun().gunEnding.transform.position, cameraObject.transform.rotation);
        lineEffect.transform.position = GetActiveGun().gunEnding.transform.position + (endPoint - GetActiveGun().gunEnding.transform.position) / 2;
        lineEffect.transform.rotation = rot;
        ParticleSystem.ShapeModule shapeModule = lineEffect.GetComponent<ParticleSystem>().shape;
        shapeModule.radius = Vector3.Distance(GetActiveGun().gunEnding.transform.position, endPoint) / 2;
        Destroy(lineEffect, 2f);
    }

    public void ThrowGrenade(Vector3 move)
    {
        grenadesLeft--;
        isThrowingGrenade = true;
        grenadeThrown = true;
        //Debug.Log("Personen som kastars nätverksid" + GetComponent<NetworkIdentity>().netId.Value);
        if (isServer)
        {
            CmdThrowGrenade(Vector3.zero, GetComponent<NetworkIdentity>().netId, CurrentTeam, cameraObject.transform.rotation);
        }
        else
        {
            //Debug.Log((float)(NATTraversal.NetworkManager.singleton.client.GetRTT() / 1000f));
            CmdThrowGrenade(move * 0.2f, GetComponent<NetworkIdentity>().netId, CurrentTeam, cameraObject.transform.rotation);

        }

        StartCoroutine(GrenadeDelay());
    }

    public IEnumerator HitMarkerEffect()
    {
        HitMarker.SetActive(true);

        if (hitMarkerAS != null && HitMarkerAC != null && !hitMarkerAS.isPlaying)
        {

            //Debug.Log("HitMarker");
            hitMarkerAS.pitch = 1;
            hitMarkerAS.PlayOneShot(HitMarkerAC);
        }


        yield return new WaitForSeconds(0.2f);

        HitMarker.SetActive(false);
    }

    public void SetMoreAmmo() // IF PICKING UP MORE AMMO FROM DROPPED WEAPON
    {
        GetActiveGun().AmmoLeft = GetActiveGun().MaxAmmo;
        if (hitMarkerAS != null && AmmoSoundAC != null && !hitMarkerAS.isPlaying)
        {

            hitMarkerAS.pitch = 1;
            hitMarkerAS.PlayOneShot(AmmoSoundAC);
        }

    }

    public void SetMoreAmmoGrenades()
    {
        GetActiveGun().AmmoLeft = GetActiveGun().MaxAmmo;
        grenadesLeft = numberOfGrenades;
        if (hitMarkerAS != null && AmmoSoundAC != null && !hitMarkerAS.isPlaying)
        {

            hitMarkerAS.pitch = 1;
            hitMarkerAS.PlayOneShot(AmmoSoundAC);
        }

    }

    public void RefillAmmoFull()
    {
        GetActiveGun().AmmoLeft = GetActiveGun().MaxAmmo;
        GetActiveGun().roundsInMag = GetActiveGun().magSize;
        grenadesLeft = numberOfGrenades;
    }

    public void ResetWeaponOnRespawn()
    {

        SetActiveGun(loadoutInfo.weaponType);
        grenadesLeft = numberOfGrenades;
        GetActiveGun().reloading = false;
        RefillAmmoFull();
        // SET PERKS HERE
    }



    public WeaponType GetActiveWeapon()
    {
        return ActiveWeapon;
    }

    public void Change3PWeapon(WeaponType wT)
    {
        if (isServer)
        {
            RpcActiveWeapon(wT);
        }
        else
        {
            CmdActiveWeapon(wT);
        }
    }

 

    [Command] // Called by client to command server to spawn shotgun effect on other clients
    public void CmdReload3P()
    {
        RpcReload3P();
    }

    [ClientRpc] // Called by server to spawn shotgun effect on other clients
    public void RpcReload3P()
    {
        Reloading3P = true;
        ReloadTime3P = Time.time + 1.5f;
   
    }

    [Command] // Called by client to command server to spawn shotgun effect on other clients
    public void CmdActiveWeapon(WeaponType wT)
    {
        RpcActiveWeapon(wT);

    }

    [ClientRpc] // Called by server to spawn shotgun effect on other clients
    public void RpcActiveWeapon(WeaponType wT)
    {

        if (!isLocalPlayer)
        {
            Team1SemiGun.SetActive(false);
            Team2SemiGun.SetActive(false);
            Team1AutoGun.SetActive(false);
            Team2AutoGun.SetActive(false);
            Team1Shotgun.SetActive(false);
            Team2Shotgun.SetActive(false);

            ActiveWeapon = wT;

            if (wT == WeaponType.Assault)
            {
                if (CurrentTeam == 0)
                {
                    Team2AutoGun.SetActive(true);
                }
                else
                {
                    Team1AutoGun.SetActive(true);
                }
            }
            if (wT == WeaponType.Railgun)
            {
                if (CurrentTeam == 0)
                {
                    Team2SemiGun.SetActive(true);
                }
                else
                {
                    Team1SemiGun.SetActive(true);
                }
            }
            if (wT == WeaponType.Shotgun)
            {

                if (CurrentTeam == 0)
                {
                    Team2Shotgun.SetActive(true);
                }
                else
                {
                    Team1Shotgun.SetActive(true);
                }
            }
        }

    }

    [Command] // Command server to spawn a grenade on all clients
    public void CmdThrowGrenade(Vector3 move, NetworkInstanceId playerID, byte t, Quaternion camRot)
    {
        isThrowingGrenade = true;
        //var grenade = (GameObject)Instantiate(GrenadePrefab, grenadeSpawn.position+move, grenadeSpawn.rotation);
        var grenade = (GameObject)Instantiate(GrenadePrefab, grenadeSpawn.position + move, camRot);
        grenade.GetComponent<Rigidbody>().velocity = grenade.transform.forward * 20;
        grenade.GetComponent<Grenade>().Id = (int)netID.Value;
        //Debug.Log("Player id är " + playerID);
        grenade.GetComponent<Grenade>().spawnedByPlayer = playerID;
        grenade.GetComponent<Grenade>().team = t;

        NetworkServer.Spawn(grenade);

        Destroy(grenade, 10f);

    }

    [Command] // Called to cause damage to player. Only server can change value
    public void CmdDmg(float amount, NetworkInstanceId playerHit, Vector3 pos)
    {
        GameObject player = NetworkServer.FindLocalObject(playerHit);
        var health = player.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(amount, netID, pos);
        }
    }

    [Command] // Called by client to command server to spawn shotgun effect on other clients
    public void CmdSGR()

    {
        if (isServer)
        {
            RpcSGR();
        }

    }

    [ClientRpc] // Called by server to spawn shotgun effect on other clients
    public void RpcSGR()

    {

        if (!isLocalPlayer)
        {

            var muzzleEffect = (GameObject)Instantiate(MuzzleEffectPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(muzzleEffect, 1f);
            var shotgunEffect = (GameObject)Instantiate(ParticleShotgunPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(shotgunEffect, 1f);

        }


    }

    [Command] // Called by client to command server to spawn shoot effect on other clients
    public void CmdSR(Vector3 direction, float range)

    {
        if (isServer)
        {
            RpcSR(direction, range);
        }

    }

    [ClientRpc] // Called by server to spawn shoot effect on other clients
    public void RpcSR(Vector3 direction, float range)

    {

        if (!isLocalPlayer)
        {
            // Spawn muzzle effect
            var muzzleEffect = (GameObject)Instantiate(MuzzleEffectPrefab, GetActiveGun().gunEnding.transform.position, GetActiveGun().gunEnding.transform.rotation);
            Destroy(muzzleEffect, 1f);

            // minimap.SpawnMarker();

            rayOrigin = cameraObject.transform.position;

            if (Physics.Raycast(cameraObject.transform.position, direction, out hit, range, CurrentMask))
            {
                // Spawn Shoot effect
                SpawnShotEffect(hit.point);

                // Spawn hit effect. Could use splash effect?????????
                var hitEffect = (GameObject)Instantiate(ParticleHitEffectPrefab, hit.point, cameraObject.transform.rotation);
                Destroy(hitEffect, 1f);

            }
            else
            {
                // Spawn Shoot effect
                SpawnShotEffect(rayOrigin + (direction * range));

            }

            if (GetActiveGun().audioSourceShoot != null && GetActiveGun().audioClips[0] != null)
            {
                float pitch = Random.Range(0.9f, 1);
                GetActiveGun().audioSourceShoot.pitch = pitch;
                GetActiveGun().audioSourceShoot.PlayOneShot(GetActiveGun().audioClips[0]);
            }

        }


    }

    public IEnumerator GrenadeDelay() // Delay until grenade can be used again
    {

        yield return new WaitForSeconds(1f);
        isThrowingGrenade = false;

    }

    public IEnumerator Reload3P(float length)
    {
        
        yield return new WaitForSeconds(length);
        Reloading3P = false;

    }

        public IEnumerator ReloadEffect(float length)
    {

        if (GetActiveGun().audioSourceShoot != null && GetActiveGun().audioClips[1] != null)
        {

            GetActiveGun().audioSourceShoot.pitch = 1;
            GetActiveGun().audioSourceShoot.PlayOneShot(GetActiveGun().audioClips[1], 1);
        }

        //Wait for reload sounds length seconds
        yield return new WaitForSeconds(length);

        int ammoInGun = 0;
        if (GetActiveGun().roundsInMag > 0)
        {
            ammoInGun = GetActiveGun().roundsInMag;
        }
        if (GetActiveGun().magSize < GetActiveGun().AmmoLeft)
        {
            GetActiveGun().roundsInMag = GetActiveGun().magSize;
            GetActiveGun().AmmoLeft -= GetActiveGun().magSize;
            GetActiveGun().AmmoLeft += ammoInGun;
        }
        else
        {
            int ammoOverflow = 0;
            int ammo = GetActiveGun().AmmoLeft;
            if ((ammo + ammoInGun) > GetActiveGun().magSize)
            {
                ammoOverflow = ammo + ammoInGun - GetActiveGun().magSize;
                GetActiveGun().roundsInMag = GetActiveGun().magSize;
                //GetActiveGun().roundsInMag = ammo;
            }
            else
            {
                GetActiveGun().roundsInMag = ammo + ammoInGun;
            }

            GetActiveGun().AmmoLeft = ammoOverflow;
        }

        GetActiveGun().reloading = false;

    }


    public IEnumerator ShotEffect(float length)
    {

        if (GetActiveGun().audioSourceShoot != null && GetActiveGun().audioClips[0] != null)
        {
            float pitch = Random.Range(0.9f, 1);
            GetActiveGun().audioSourceShoot.pitch = pitch;
            GetActiveGun().audioSourceShoot.PlayOneShot(GetActiveGun().audioClips[0]);
        }

        yield return new WaitForSeconds(0.5f);

        //isRecoiling = false;
    }
}

public struct LoadoutInfo
{
    public WeaponType weaponType;
    public PerkType weaponPerk;
    //public byte playerPerk;
}

public enum PerkType
{
    FasterReload = 0, FasterFirerate = 1, LessSpread = 2, IncreasedSplashDmg = 3
}

public enum WeaponType
{
    Assault = 1, Railgun = 2, Shotgun = 3
}

