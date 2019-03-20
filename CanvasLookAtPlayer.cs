using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        Canvas.willRenderCanvases += OnPreRenderCanvas;
    }

    private void OnDisable()
    {
        Canvas.willRenderCanvases -= OnPreRenderCanvas;
    }

    private void OnPreRenderCanvas()
    {
        if (Camera.current != null)
        {
            transform.LookAt(Camera.main.transform.position, -Vector3.up);
        }
        
    }
}
