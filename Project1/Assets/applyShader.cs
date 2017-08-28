using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyShader : MonoBehaviour {
    public Shader shader;
    public PointLight pointLight;
    // Use this for initialization
    void Start () {
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = shader;
    }
	
	// Update is called once per frame
	void Update () {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
    }
}
