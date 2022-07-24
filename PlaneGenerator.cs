using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaneGenerator : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public Material material;

    public float squareSize = 1f;
    public int resolution = 10;

    public float noiseOffset = 1;
    public float[] noiseAmplitude = {1f};
    public float[] noiseScale = {.1f};
    public int noiseOctaves = 2;

    private void Awake() {
        GenerateMesh();
    }

    private void Update() {
        GenerateMesh();
    }

    public void GenerateMesh(){

        if(resolution < 1){resolution = 1;}
        if(squareSize < 0.1f){squareSize = 0.1f;}
        
        if(mesh == null){
            mesh = new Mesh();
        }

        if(meshFilter == null){meshFilter = GetComponent<MeshFilter>();
            if(meshFilter == null){meshFilter = gameObject.AddComponent<MeshFilter>();}
            meshFilter.mesh = mesh;
        }

        if(meshRenderer == null){meshRenderer = GetComponent<MeshRenderer>();
            if(meshRenderer == null){meshRenderer = gameObject.AddComponent<MeshRenderer>();}
        }

        int totalVertices = (resolution + 1) * (resolution + 1);
        int totalTriangles = resolution * resolution * 6;

        float verticeDistance = squareSize/resolution;

        Vector3[] vertices = new Vector3[totalVertices];
        Vector2[] uv = new Vector2[totalVertices];
        int[] triangles = new int[totalTriangles];

        int vIndex = 0;

        for(int x = 0;x <= resolution; x++) {

            for(int y = 0;y <= resolution; y++){

                float xPos = (x * verticeDistance);
                float zPos = (y * verticeDistance);

                float yPos = GetHeight(xPos,zPos);

                Vector3 pos = new Vector3(xPos,yPos,zPos);

                vertices[vIndex] = pos;
                uv[vIndex] = new Vector2(xPos,zPos);

                vIndex++;

            }
        }

        int vert = 0;
        int tri = 0;

        for(int x = 0;x < resolution; x++) {
            for(int y = 0;y < resolution; y++){
                
                triangles[tri + 0] = vert + resolution + 1;
                triangles[tri + 1] = vert;
                triangles[tri + 2] = vert + 1;
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + resolution + 2;
                triangles[tri + 5] = vert + resolution + 1;

                vert++;
                tri += 6;
            }
            
            vert++;
            
        }

        mesh.Clear();
       
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
    
        mesh.RecalculateNormals();

        if(material != null)
        {
            meshRenderer.material = material;
        }    
    }

    public float GetHeight(float x,float z)
    {
        float h = 0;

        for (int i = 0; i < noiseOctaves; i++)
        {
            
            h += Mathf.PerlinNoise((transform.position.x + x) * (noiseScale[i]),(transform.position.z + z) * (noiseScale[i]));
            
            h *= noiseAmplitude[i];
      
        }

   

      

        return h;
    }


}


[CustomEditor(typeof(PlaneGenerator))]
public class PlaneGeneratorGui : Editor{

    public override void OnInspectorGUI() {
        
        base.OnInspectorGUI();

        foreach(PlaneGenerator plane in targets)
        {
            plane.GenerateMesh();
        }
    }



}