using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JumpPad : MonoBehaviour {
    

   
    public float jumpHeight;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                other.GetComponent<CharacterController3D>().PadJump(jumpHeight);
                other.GetComponent<VoiceActing>().Laugh();
            }else
            {
                other.GetComponent<VoiceActing>().Laugh();
            }
        }
    }
}
