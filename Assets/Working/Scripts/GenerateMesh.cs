using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour {

    [SerializeField] List<Transform> ControlPointsList;
    [SerializeField] Material mat;
    [SerializeField] int GridSize = 10;


    Vector3[] vertices = new Vector3[4];
    Vector2[] uvs = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
    int[] tris = { 0, 1, 2, 0, 2, 3 };

	// Use this for initialization
	void Start () {
        var mf = this.gameObject.AddComponent<MeshFilter>();
        var mr = this.gameObject.AddComponent<MeshRenderer>();

        vertices[0] = ControlPointsList[0].position;
        vertices[1] = ControlPointsList[3].position;
        vertices[2] = ControlPointsList[2].position;
        vertices[3] = ControlPointsList[1].position;

        //Vector3[] vertices = new Vector3[(GridSize + 1) * (GridSize + 1)];
        //int i = 0;
        //for(int y = 0; y <= GridSize; y++) {
        //    for(int x = 0; x <= GridSize; x++) {

        //    }
        //}

        //Material mat = new Material(Shader.Find("Unlit/Texture"));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mf.mesh = mesh;
        mr.material = mat;
    }

}
