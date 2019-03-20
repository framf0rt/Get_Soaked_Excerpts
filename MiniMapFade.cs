using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFade : MonoBehaviour {


    private Color color;
    public float FadeSpeed = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        color = GetComponent<Renderer>().material.color;
        color.a -= 0.0f;
        GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, color, FadeSpeed * Time.deltaTime);
    }
}
