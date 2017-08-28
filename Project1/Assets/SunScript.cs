using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {

	// Use this for initialization

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.RotateAround (new Vector3(512, 512, 512/2), Vector3.forward, 50 * Time.deltaTime);
	}
}
