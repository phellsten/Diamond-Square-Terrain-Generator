using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    // Initialisation of game variables
    private float maxX = 1024f;
    private float maxZ = 1024f;
    private float minZ = 0;
    private float minX = 0;
    private int speed = 25;

    void Update() {
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
		}
        else {
			speed = 25;
		}

		// Move relative to the direction faced.
		if (Input.GetKey("w")) {
            transform.position += this.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("s")) {
			transform.position -= this.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey ("a")) {
			transform.position -= this.transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey ("d")) {
			transform.position += this.transform.right * speed * Time.deltaTime;
		}

        // Restricting camera movement to above the terrain
        if(transform.position.x > maxX) {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < minX) {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
        if (transform.position.z > maxZ) {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
        else if (transform.position.x < minZ) {
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }
    }
}