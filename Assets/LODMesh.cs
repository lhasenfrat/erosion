using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODMesh : MonoBehaviour
{
    Mesh mesh;
    public int xSize;
    public int zSize;
    public int OriginalFactor;
    public Vector3[] vertices;
    int[] faces;
    public float[] Amplitude;
    public float[] Frequency;
    public Vector2 center;
    public float scale;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateTerrain();
        GenerateFaces();
        UpdateMesh();
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[xSize * zSize - zSize / 2];

        for (int i = 0, z = 0; z < zSize; z++)
        {

            for (int x = 0; x < xSize - z % 2; x++)
            {
                float newX = (x + (z % 2) * 0.5f)* OriginalFactor;
                float newZ = z * 0.866f * OriginalFactor;
                float DisToCenter = Vector2.Distance(new Vector2(transform.position.x + newX*scale, transform.position.z + newZ*scale), new Vector2(center.x, center.y)) / 200f;
                float newY = CustomPerlinNoiseFunction(transform.position.x + newX, transform.position.z + newZ) * (1 - DisToCenter * 0.5f);
                vertices[i] = new Vector3(newX, newY, newZ);
                i++;
            }
        }

    }
    void GenerateFaces()
    {
        faces = new int[((xSize * 2 - 3) * (zSize - 1)) * 3];
        for (int i = 0, z = 0, vert = 0; z < zSize - 1; z++)
        {

            for (int x = 0; x < xSize - 1; x++)
            {

                if (z % 2 == 0)
                {
                    if (x == 0)
                    {
                        faces[i] = vert;
                        faces[i + 2] = vert + 1;
                        faces[i + 1] = vert + xSize;
                        i += 3;
                    }
                    else
                    {
                        faces[i] = vert;
                        faces[i + 1] = vert + xSize - 1;
                        faces[i + 2] = vert + xSize;
                        i += 3;
                        faces[i] = vert;
                        faces[i + 2] = vert + 1;
                        faces[i + 1] = vert + xSize;
                        i += 3;
                    }
                }
                else
                {
                    if (x == xSize - 2)
                    {
                        faces[i] = vert;
                        faces[i + 1] = vert + xSize - 1;
                        faces[i + 2] = vert + xSize;
                        i += 3;
                    }
                    else
                    {
                        faces[i] = vert;
                        faces[i + 1] = vert + xSize - 1;
                        faces[i + 2] = vert + xSize;
                        i += 3;
                        faces[i] = vert;
                        faces[i + 2] = vert + 1;
                        faces[i + 1] = vert + xSize;
                        i += 3;
                    }
                }
                vert++;

            }
            if (z % 2 == 0)
                vert++;

        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = faces;
        mesh.RecalculateNormals();
    }

    public void UpdateTopology()
    {
        GenerateTerrain();
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        

    }

    float CustomPerlinNoiseFunction(float x, float y)
    {
        return Mathf.PerlinNoise(x * Frequency[0], y * Frequency[0]) * Amplitude[0] + Mathf.PerlinNoise(x * Frequency[1], y * Frequency[1]) * Amplitude[1] + Mathf.PerlinNoise(x * Frequency[2], y * Frequency[2]) * Amplitude[2] + Mathf.PerlinNoise(x * Frequency[3], y * Frequency[3]) * Amplitude[3];
    }
}
