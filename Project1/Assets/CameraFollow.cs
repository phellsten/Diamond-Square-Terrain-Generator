using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public GameObject target;

    private TerrainGenerator tg = null;


    // Use this for initialization
    void Start()
    {
        GameObject terrain = GameObject.Find("Terrain");
        tg = terrain.GetComponent<TerrainGenerator>();
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

        if (tg == null)
            return;

        if (this.transform.position.x < 1)
            this.transform.position = new Vector3(2, this.transform.position.y, this.transform.position.z);
        if (this.transform.position.x > tg.size - 1)
            this.transform.position = new Vector3(tg.size-2, this.transform.position.y, this.transform.position.z);

        if (this.transform.position.z < 1)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 2);
        if (this.transform.position.z > tg.size - 1)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, tg.size-2);


        float? h = tg.getHeight(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z));
        if (h == null) return;

        if (this.transform.position.y < h+3)
            this.transform.position = new Vector3(this.transform.position.x, h.Value+4, this.transform.position.z);
        if (this.transform.position.y > tg.maxHeight * 1.5f - 1)
            this.transform.position = new Vector3(this.transform.position.x, tg.maxHeight * 1.5f - 2, this.transform.position.z);


    }
}