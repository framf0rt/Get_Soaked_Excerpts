using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DominationSpawn : NetworkBehaviour {

    public GameObject domPad;
  
    public override void OnStartServer()
    {
     
        var dominationPad = Instantiate(domPad);
        dominationPad.transform.position = new Vector3(0f, 0f,0f);
        
        //NetworkServer.SpawnWithClientAuthority(domPad,connectionToClient);
        NetworkServer.Spawn(dominationPad);


    }
  
	
	
}
