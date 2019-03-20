using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class TestPlayer : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //gun.CmdFire();
        }


        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        float r = 0;
        if (Input.GetKey(KeyCode.R))
        {
            r = 1 *Time.deltaTime * 10;
            
          }
        if (Input.GetKey(KeyCode.F))
        {
            r = -1 * Time.deltaTime * 10;

        }

        transform.Rotate(r, x, 0);
        transform.Translate(0, 0, z);
    }


}
