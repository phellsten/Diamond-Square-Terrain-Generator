using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    // Use this for initialization

    private float speedH = 2.0f;
    private float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 55.0f;

	private float rot = 0.0f;

    void Start()
    {

    }

    int speed = 25;
    // Update is called once per frame
    void Update()
    {
		// Rolling of camera.
		if (Input.GetKey("e")) {
			transform.RotateAroundLocal (-transform.forward, 0.025f);
		}
		if (Input.GetKey("q")) {
			transform.RotateAroundLocal (transform.forward, 0.025f);
		}

				// Mouse movement causes direction of camera to rotate.
		if (Input.GetAxis ("Mouse X") != 0f) {
			transform.Rotate (new Vector3 (0f, Input.GetAxis("Mouse X")));
		}
		if (Input.GetAxis ("Mouse Y") != 0f) {
			transform.Rotate (new Vector3 (-Input.GetAxis ("Mouse Y"), 0f));
		}

		// Shift to move faster.
		if (Input.GetKey(KeyCode.LeftShift)) {
			speed = 25 * 10;
		} else {
			speed = 25;
		}

		// Move relative to the direction faced.
		if (Input.GetKey("w"))
		{
			transform.position += this.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("s"))
		{
			transform.position -= this.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey ("a")) {
			transform.position -= this.transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey ("d")) {
			transform.position += this.transform.right * speed * Time.deltaTime;
		}
    }
}