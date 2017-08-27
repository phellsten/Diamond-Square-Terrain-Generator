using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour
{
    public Shader shader;
    public PointLight pointLight;

    // Use this for initialization
    void Start()
    {
        // Add a MeshFilter component to this entity. This essentially comprises of a
        // mesh definition, which in this example is a collection of vertices, colours 
        // and triangles (groups of three vertices). 
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        cubeMesh.mesh = this.CreateCubeMesh();

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = shader;
    }

    // Called each frame
    void Update()
    {
        // Get renderer component (in order to pass params to shader)
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
    }

    // Method to create a cube mesh with coloured vertices
    Mesh CreateCubeMesh()
    {
        Mesh m = new Mesh();
        m.name = "Cube";

        // Define the vertices. These are the "points" in 3D space that allow us to
        // construct 3D geometry (by connecting groups of 3 points into triangles).
        m.vertices = new[] {
            new Vector3(-1.0f, 1.0f, -1.0f), // Top
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, -1.0f),

            new Vector3(-1.0f, -1.0f, -1.0f), // Bottom
            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),

            new Vector3(-1.0f, -1.0f, -1.0f), // Left
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),

            new Vector3(1.0f, -1.0f, -1.0f), // Right
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),

            new Vector3(-1.0f, 1.0f, 1.0f), // Front
            new Vector3(1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 1.0f),
            new Vector3(1.0f, -1.0f, 1.0f),

            new Vector3(-1.0f, 1.0f, -1.0f), // Back
            new Vector3(1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f)
        };

        // Define the vertex colours
        m.colors = new[] {
            Color.red, // Top
            Color.red,
            Color.red,
            Color.red,
            Color.red,
            Color.red,

            Color.red, // Bottom
            Color.red,
            Color.red,
            Color.red,
            Color.red,
            Color.red,

            Color.yellow, // Left
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.yellow,

            Color.yellow, // Right
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.yellow,
            Color.yellow,
            
            Color.blue, // Front
            Color.blue,
            Color.blue,
            Color.blue,
            Color.blue,
            Color.blue,

            Color.blue, // Back
            Color.blue,
            Color.blue,
            Color.blue,
            Color.blue,
            Color.blue
        };

        // Define normals for each of the six faces of the cube
        /*Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
        Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
        Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);
        Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);

        m.normals = new[] {
            topNormal, // Top
            topNormal,
            topNormal,
            topNormal,
            topNormal,
            topNormal,

            bottomNormal, // Bottom
            bottomNormal,
            bottomNormal,
            bottomNormal,
            bottomNormal,
            bottomNormal,

            leftNormal, // Left
            leftNormal,
            leftNormal,
            leftNormal,
            leftNormal,
            leftNormal,

            rightNormal, // Right
            rightNormal,
            rightNormal,
            rightNormal,
            rightNormal,
            rightNormal,
            
            frontNormal, // Front
            frontNormal,
            frontNormal,
            frontNormal,
            frontNormal,
            frontNormal,

            backNormal, // Back
            backNormal,
            backNormal,
            backNormal,
            backNormal,
            backNormal
        };*/


        // Vertex normals
        Vector3 frontBottomLeftNormal = (new Vector3(-1.0f, -1.0f, 1.0f)).normalized;
        Vector3 frontTopLeftNormal = (new Vector3(-1.0f, 1.0f, 1.0f)).normalized;
        Vector3 frontTopRightNormal = (new Vector3(1.0f, 1.0f, 1.0f)).normalized;
        Vector3 frontBottomRightNormal = (new Vector3(1.0f, -1.0f, 1.0f)).normalized;
        Vector3 backBottomLeftNormal = (new Vector3(-1.0f, -1.0f, -1.0f)).normalized;
        Vector3 backBottomRightNormal = (new Vector3(1.0f, -1.0f, -1.0f)).normalized;
        Vector3 backTopLeftNormal = (new Vector3(-1.0f, 1.0f, -1.0f)).normalized;
        Vector3 backTopRightNormal = (new Vector3(1.0f, 1.0f, -1.0f)).normalized;

        m.normals = new[] {
            backTopLeftNormal, // Top
            frontTopLeftNormal,
            frontTopRightNormal,
            backTopLeftNormal,
            frontTopRightNormal,
            backTopRightNormal,

            backBottomLeftNormal, // Bottom
            frontBottomRightNormal,
            frontBottomLeftNormal,
            backBottomLeftNormal,
            backBottomRightNormal,
            frontBottomRightNormal,

            backBottomLeftNormal, // Left
            frontBottomLeftNormal,
            frontTopLeftNormal,
            backBottomLeftNormal,
            frontTopLeftNormal,
            backTopLeftNormal,

            backBottomRightNormal, // Right
            frontTopRightNormal,
            frontBottomRightNormal,
            backBottomRightNormal,
            backTopRightNormal,
            frontTopRightNormal,

            frontTopLeftNormal, // Front
            frontBottomRightNormal,
            frontTopRightNormal,
            frontTopLeftNormal,
            frontBottomLeftNormal,
            frontBottomRightNormal,

            backTopLeftNormal, // Back
            backTopRightNormal,
            backBottomRightNormal,
            backBottomLeftNormal,
            backTopLeftNormal,
            backBottomRightNormal
        };

        // Automatically define the triangles based on the number of vertices
        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
            triangles[i] = i;

        m.triangles = triangles;

        return m;
    }
}
