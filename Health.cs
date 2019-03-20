using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;




[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class Health : NetworkBehaviour
{

    public const float maxHealth = 100;
    //[SyncVar(hook = "OnChangeHealth")]
    public float currentHealth = maxHealth;

   // public RectTransform healthBar;
    public Slider HealthBar;
    public Slider HealthBarPlayer;
    //public RectTransform healthBarPlayer;

    private NetworkStartPosition[] spawnPoints;
    public float HpRegenRate;
    private float regenTimeout;
    private AudioSource audioS;
    public AudioClip ouchSound;
    public GameObject[] weaponDrop;
    private RaycastHit groundInfo;
    public LayerMask rayLayer;
    public GameObject[] spawnLocations;
    private ScoreManager scores;
    private float respawnTime;
    public int netID;
    public AudioClip[] DMGSounds;
    public AudioClip DeathSounds;
    private int lastDMGSound;
    private float splatDuration = 2f;
    public float TimeToRegenStart;
    public bool respawning = false;
    private bool respawned = false;
    public float DeathAnimTime;
    private Vector3 currentDmgDirection;
    private CharacterInput input;
    private CharacterController3D cc;

    public GameObject splatPanel;
    public GameObject deathPanel;
    private Material splatMat;
    private Material deathMat;
    private Vector3 colliderPos;
    [HideInInspector]
    public bool isSafe = false;
    public GameObject healthBarCanvas;
    private VoiceActing va;
    private bool firstRespawn = true;
    private bool firstSpawnRetry = true;
    private const short youKilledPlayer = 141;

    //for DeathAnimation

    private NetworkAnimScript animScript;
   

    void Start()
    {
        animScript = GetComponent<NetworkAnimScript>();
        if (isLocalPlayer)
        {
            cc = GetComponent<CharacterController3D>();
            input = GetComponent<CharacterInput>();
            splatMat = splatPanel.GetComponent<Image>().material;
            deathMat = deathPanel.GetComponent<Image>().material;
            deathMat.SetFloat("_FadeToBlack", 0f);
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            spawnLocations = GameObject.FindGameObjectsWithTag("Spawn Location");
        }
        va = GetComponent<VoiceActing>();
        colliderPos = GetComponent<CapsuleCollider>().center;
        if (isServer)
        {
            scores = GameObject.Find("Score Manager").GetComponent<ScoreManager>();
           
        }
        int netID = (int)GetComponent<NetworkIdentity>().netId.Value;
        audioS = GetComponent<AudioSource>();
    }

 

   

    public void Update()
    {
        if (respawning)
        {
            if (isLocalPlayer)
            {
                deathMat.SetFloat("_FadeToBlack", (Time.time - respawnTime) / DeathAnimTime);
            }
            if ((Time.time - respawnTime) > DeathAnimTime)
            {
                
                respawning = false;
                if (!isLocalPlayer)
                {
                    Debug.Log("SLutar spela dödsanimation på motståndaren");
                    animScript.StopDeathAnimation();
                }
                
               
                currentHealth = maxHealth;
                HealthBar.value = currentHealth;
                HealthBarPlayer.value = currentHealth;
                healthBarCanvas.SetActive(true);
                //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
                //healthBarPlayer.sizeDelta = new Vector2(currentHealth * 3, healthBarPlayer.sizeDelta.y);
                GetComponent<CapsuleCollider>().center = colliderPos;
                if (isLocalPlayer)
                {
                    if (firstSpawnRetry)
                    {
                        firstSpawnRetry = false;
                    }else
                    {
                        va.SpawnRetry();
                    }
                    
                    deathPanel.transform.parent.gameObject.SetActive(false);
                    cc.SetRespawning(false);
                    input.LockedMovement = false;
                    //input.LockedRotation = false;
                    deathMat.SetFloat("_FadeToBlack", 0f);
                    // Set the spawn point to origin as a default value
                    Vector3 spawnPoint = Vector3.zero;
                    GetComponent<WeaponHandler>().ResetWeaponOnRespawn();
                    // If there is a spawn point array and the array is not empty, pick a spawn point at random
                    //if (spawnPoints != null && spawnPoints.Length > 0)
                    //{
                    //    spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                    //}
                    int team = GetComponent<PlayerSyncData>().GetTeam();
                    int slot = GetComponent<PlayerSyncData>().GetSlot();
                    if (team == 0)
                    {
                        if (slot == 0)
                        {
                            spawnPoint = spawnLocations[0].GetComponent<SpawnLocationInfo>().spawnTransform[0].position;
                        }
                        if (slot == 2)
                        {
                            spawnPoint = spawnLocations[0].GetComponent<SpawnLocationInfo>().spawnTransform[1].position;
                        }
                        if (slot == 4)
                        {
                            spawnPoint = spawnLocations[0].GetComponent<SpawnLocationInfo>().spawnTransform[2].position;
                        }
                        if (slot == 6)
                        {
                            spawnPoint = spawnLocations[0].GetComponent<SpawnLocationInfo>().spawnTransform[3].position;
                        }
                    }
                    else
                    {
                        if (slot == 1)
                        {
                            spawnPoint = spawnLocations[1].GetComponent<SpawnLocationInfo>().spawnTransform[0].position;
                        }
                        if (slot == 3)
                        {
                            spawnPoint = spawnLocations[1].GetComponent<SpawnLocationInfo>().spawnTransform[1].position;
                        }
                        if (slot == 5)
                        {
                            spawnPoint = spawnLocations[1].GetComponent<SpawnLocationInfo>().spawnTransform[2].position;
                        }
                        if (slot == 7)
                        {
                            spawnPoint = spawnLocations[1].GetComponent<SpawnLocationInfo>().spawnTransform[3].position;
                        }
                    }

                    // Set the player’s position to the chosen spawn point
                    transform.position = spawnPoint;
                    respawned = true;

                }
            }
        }
        
        if (isLocalPlayer)
        {
            Vector3 targetDir = currentDmgDirection - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            Vector3 cross = Vector3.Cross(targetDir, transform.forward);
            if (cross.y < 0) angle = -angle;
            splatMat.SetFloat("_DamageDirection", angle);
            splatMat.SetFloat("_Activate", Mathf.Clamp(regenTimeout / splatDuration,0f, 1f));
            regenTimeout += Time.deltaTime;
            if (regenTimeout > TimeToRegenStart)
            {
                if (currentHealth <= maxHealth)
                {
                    
                    currentHealth += Time.deltaTime * HpRegenRate;
                    currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);
                    //Debug.Log("currentHealth Local" + currentHealth);
                    HealthBar.value = currentHealth;
                    HealthBarPlayer.value = currentHealth;
                    //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
                    //healthBarPlayer.sizeDelta = new Vector2(currentHealth * 3, healthBarPlayer.sizeDelta.y);
                }
            }
            
        }
    }

  

    public void TakeDamage(float amount, NetworkInstanceId playerWhoHit, Vector3 pos)
    {
        if (!isServer)
        {


            return;
        }
        if (isSafe||respawning)
        {
            return;
        }
        if ((Time.time - respawnTime) < 2f ||respawning)
        {
            return;
        }
        currentHealth -= amount;
        RpcSetHealth(currentHealth,pos);
        //print(currentHealth);
        if (currentHealth <= 0)
        {
           
            respawning = true;
            respawnTime = Time.time;
            //Debug.Log(GetComponent<NetworkIdentity>().netId.Value);
            //Debug.Log("Player kill ID " + playerWhoHit + " Player who died ID " + netID);
            scores.SetKillInfo((int)playerWhoHit.Value, (int)GetComponent<NetworkIdentity>().netId.Value);
            
            // called on the Server, but invoked on the Clients
            
            //Debug.Log(GetComponent<WeaponHandler>().GetActiveWeapon());
            CmdDropWeapon(transform.position,transform.rotation, (byte)GetComponent<WeaponHandler>().GetActiveWeapon());
           
         
            CmdRespawn();
           



        }
        else
        {


        }


    }

    public bool getRespawned()
    {
        return respawned;
    }

    public void setRespawned(bool r)
    {
        respawned = r;
    }


    public void PlayDMGSound()
    {
        int s = Random.Range(0, DMGSounds.Length);
        if (s == lastDMGSound)
        {
            s = Random.Range(0, DMGSounds.Length);
        }
        lastDMGSound = s;
        if (!audioS.isPlaying && DMGSounds[s] != null)
        {
            audioS.PlayOneShot(DMGSounds[s]);
        }
    }

    [Command]
    public void CmdRespawn()
    {
        RpcRespawn();
       
        //currentHealth = maxHealth;
    }
 


    [Command]
    public void CmdStartSpawn()
    {
        RpcRespawn();
    }
    [Command]
    public void CmdDropWeapon(Vector3 pos, Quaternion rot, byte wt)
    {
        //WeaponType wt = GetComponent<WeaponHandler>().GetActiveWeapon();
        GameObject objectToSpawn;
        if (wt == (byte)WeaponType.Assault)
        {
            objectToSpawn = Instantiate(weaponDrop[0]);
        }else if (wt == (byte)WeaponType.Railgun)
        {
            objectToSpawn = Instantiate(weaponDrop[1]);
        }else
        {
            objectToSpawn = Instantiate(weaponDrop[2]);
        }
       
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rot;
       
        //objectToSpawn.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        if(Physics.Raycast(objectToSpawn.transform.position, Vector3.down, out groundInfo, 100f,rayLayer))
        {
            //Debug.Log("Träffar nåt");
            objectToSpawn.transform.position = groundInfo.point+0.5f*Vector3.up;

        }
        
        NetworkServer.Spawn(objectToSpawn);
        //Destroy(objectToSpawn, 15f);
    }

    public void SetHealth(float h)
    {
      
        currentHealth = h;
        HealthBar.value = currentHealth;
        HealthBarPlayer.value = currentHealth;
        //healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        //healthBarPlayer.sizeDelta = new Vector2(currentHealth * 3, healthBarPlayer.sizeDelta.y);
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    [Command]
    public void CmdKilledSomeone(NetworkInstanceId net)
    {
        
        NetworkConnection c = NetworkServer.FindLocalObject(net).GetComponent<NetworkIdentity>().connectionToClient;
        TargetYouKilledSomeone(c);
    }
    [TargetRpc]
    public void TargetYouKilledSomeone(NetworkConnection n)
    {
        //Debug.Log("I killed someone!");
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (!isLocalPlayer)
        {
            animScript.StartDeathAnimation();
            Debug.Log("Börjar spela dödsanimation på motståndaren");
        }

        if (isLocalPlayer)
        {
            CmdKilledSomeone(GetComponent<NetworkIdentity>().netId);
            if (firstRespawn)
            {
                firstRespawn = false;
            }else
            {
                va.PlayerDeath();
            }
            
            cc.SetRespawning(true);
            input.resetInput();
            input.LockedMovement = true;
            //input.LockedRotation = true;
            deathPanel.transform.parent.gameObject.SetActive(true);
            
        }
        healthBarCanvas.SetActive(false);
        GetComponent<CapsuleCollider>().center = colliderPos + Vector3.up * 1000f;
        respawnTime = Time.time;
        respawning = true;
        
    }

    [ClientRpc]
    public void RpcSetHealth(float health, Vector3 pos)
    {
        regenTimeout = 0f;
        currentHealth = health;

        HealthBar.value = currentHealth;
        HealthBarPlayer.value = currentHealth;
        //healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
        //healthBarPlayer.sizeDelta = new Vector2(health * 3, healthBarPlayer.sizeDelta.y);

        if (isLocalPlayer)
        {
            currentDmgDirection = pos;

            //PlayDMGSound();
            va.GetHit();
        }

        //if (audioS != null && ouchSound != null && !audioS.isPlaying)
        //       audioS.PlayOneShot(ouchSound, 0.5f);
        
    }
    //void OnChangeHealth(float health)
    //{
    //    healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    //    healthBarPlayer.sizeDelta = new Vector2(health * 3, healthBarPlayer.sizeDelta.y);

    //    // SOUND EFFECT!


    //}
}

