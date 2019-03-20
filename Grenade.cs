using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : NetworkBehaviour {

    public float ExplodeTimer = 3;
    public float radius = 15f;
    public float dmg;
    public int dmgRollOffZones;
    public bool isBounceable;
    public LayerMask collisionLayer;
    public LayerMask dmgLayerMask2;
    private bool exploded;
    private AudioSource audioS;
    public AudioClip audioCExplosion;
    public AudioClip audioCBounce;
    public GameObject ExplosionPrefab;
    public int Id;
    RaycastHit s;
  
    public NetworkInstanceId spawnedByPlayer;
    public byte team;
    public LayerMask ignoreLayers;


    // Use this for initialization
    void Start () {
        audioS = GetComponent<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
  
        if (!isBounceable&&isServer)
        {
            if (ExplodeTimer < 0 && exploded != true)
            {
                OnExplosion(this.transform.gameObject, false);


            }
            else
            {
                ExplodeTimer -= Time.deltaTime;
            }
        }
	}
    private void OnTriggerEnter(Collider collision)
    {
      
        if (!isServer)
        {
            return;
        }
        if (((1 << collision.gameObject.layer) & ignoreLayers) != 0)
        {
            return;
        }

            int player = -1;
        if (collision.transform.CompareTag("Player"))
        {

            player = (int)collision.transform.GetComponent<NetworkIdentity>().netId.Value;
            //Debug.Log("Player hit är " + player + " och player som spawna är " + spawnedByPlayer);
        }
        if (isBounceable)
        {
            if (audioS != null & audioCBounce != null && exploded != true)
            {
                audioS.PlayOneShot(audioCBounce);
            }
        }
        else if (!exploded && player != spawnedByPlayer.Value)
        {
            //Debug.Log("Objekt som orsaker explosion är " + collision.transform.gameObject.name);
            OnExplosion(collision.gameObject, true);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        //if (!isServer)
        //{
        //    return;
        //}
       
        //int player = -1;
        //if (collision.transform.CompareTag("Player"))
        //{
           
        //    player = (int)collision.transform.GetComponent<NetworkIdentity>().netId.Value;
        //    Debug.Log("Player hit är " + player + " och player som spawna är " + spawnedByPlayer);
        //}
        //if (isBounceable)
        //{
        //    if (audioS != null & audioCBounce != null && exploded != true)
        //    {
        //        audioS.PlayOneShot(audioCBounce);
        //    }
        //} else if (!exploded&&player!=spawnedByPlayer)
        //{
        //    Debug.Log("Objekt som orsaker explosion är " + collision.transform.gameObject.name);
        //    OnExplosion();
        //}
    }
    [ClientRpc]
    public void RpcExplosionEffect()
    {
        GetComponent<MeshRenderer>().enabled = false;
        var explosion = (GameObject)Instantiate(
          ExplosionPrefab,
        this.transform.position,
        this.transform.rotation);
        Destroy(explosion, 1f);
        if (audioS != null & audioCExplosion != null)
        {
            audioS.PlayOneShot(audioCExplosion);
        }
    }
    [ClientRpc]
    public void RpcPlaySound()
    {
       
    }

    public virtual void OnExplosion(GameObject go, bool directHit)
    {
        exploded = true;
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 7.5f, collisionLayer);
        Debug.Log("Amount of players hit" + hitColliders.Length);
        if (directHit)
        {
            if (go.tag.Equals("Player"))
            {
                if (go.GetComponent<WeaponHandler>().CurrentTeam != team && (int)go.GetComponent<WeaponHandler>().netID.Value != Id)
                {
                    go.GetComponent<Health>().TakeDamage(110f, spawnedByPlayer, transform.position);
                }
            }
        }

        //RpcPlaySound();
        RpcExplosionEffect();
        

        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                //if (hitColliders[i].tag.Equals("Player"))
                //{
                //    if (hitColliders[i].GetComponent<WeaponHandler>().CurrentTeam != team && hitColliders[i].GetComponent<WeaponHandler>().netID != Id)
                //    {
                //        hitColliders[i].GetComponent<Health>().TakeDamage(110f, Id, transform.position);
                //    }
                //}

                Vector3 raycastDir = hitColliders[i].transform.position - transform.position;
                if (Physics.Raycast(this.transform.position, raycastDir, out s, dmgLayerMask2))
                {
                    Debug.Log("Tagen är " + s.collider.tag + " och namnet på objektet är " + s.collider.name);                                                                           // CHECK TO SEE IF PLAYER IS BEHIND WALL
                }
                if (s.collider.tag.Equals("Player"))
                {
                    
                    //Debug.Log("team that spawned" + team);
                    //Debug.Log("current team är " + s.collider.transform.GetComponent<WeaponHandler>().CurrentTeam);
                    if (s.collider.transform.GetComponent<WeaponHandler>().CurrentTeam != team)
                    {
                        
                        float distanceFromCenter = Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position);
                        dmg = dmg / distanceFromCenter;
                        Debug.Log("Nu borde jag ta " + dmg);
                        s.collider.GetComponent<Health>().TakeDamage(dmg, spawnedByPlayer, transform.position);
                    }
                    
                }


            }
        }


        
    }


}
