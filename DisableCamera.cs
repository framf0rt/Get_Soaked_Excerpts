using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableCamera : NetworkBehaviour
{
    public Camera thisCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            thisCamera.enabled = false;
        }
    }
}
