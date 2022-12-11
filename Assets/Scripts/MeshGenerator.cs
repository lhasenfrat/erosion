using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class CustomMeshGenerator : MonoBehaviour
{
    public int xSize;
    public int zSize;
    public float Amplitude;
    public float Frequency;
    public float Amplitude1;

    public float Frequency1;
    public float Amplitude2;
    public float Frequency2;
    public float Amplitude3;
    public float Frequency3;
    public Gradient gradient;
    Mesh mesh;
    public Vector3[] vertices;
    int[] faces;
    // Start is called before the first frame update


    private void OnValidate()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateTerrain();
        UpdateMesh();
    }
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        BoxCollider collider = GetComponent<BoxCollider>();

        collider.size = new Vector3(xSize,1 ,zSize*0.83f) ;
        float amplsum = Amplitude+ Amplitude1+ Amplitude2+ Amplitude3;

        
        collider.center = new Vector3(xSize/2, amplsum/2, (zSize * 0.866f)/2);
        GenerateTerrain();
        GenerateFaces();
        UpdateMesh();

    }

    public Vector3 RandomVertice()
    {
        int randomz = Random.Range(0,zSize);
        int randomx= Random.Range(0, xSize - randomz % 2);
        
        return new Vector3(randomx, GetVertice(new Vector3(randomx,0,randomz)).y, randomz);
    }


    public Vector3 GetVertice(Vector3 vertice)
    {
        return vertices[GetVerticeId(vertice)];
    }
    public int GetVerticeId(Vector3 vertice)
    {
        int xbase = (int)(vertice.x);
        int zeven = ((int)((vertice.z+1)/2)) *xSize;
        int zodd = ((int)((vertice.z) / 2) ) * (xSize - 1);
        int target = xbase + zeven + zodd ;
        return target;
    }

    public List<Vector3> GetSurroundingVertices(Vector3 vertice)
    {
        List < Vector3 > result = new List<Vector3>();
        int verticeid = GetVerticeId(vertice);

        if (vertice.z % 2 == 0)
        {
            if (vertice.x != 0)
            {
                result.Add(new Vector3(vertice.x - 1, vertices[verticeid - 1].y, vertice.z));
                if (vertice.z != 0)
                {
                    result.Add(new Vector3(vertice.x - 1, vertices[verticeid - xSize].y, vertice.z - 1));
                }
                if (vertice.z != zSize - 1)
                {
                    result.Add(new Vector3(vertice.x - 1, vertices[verticeid + xSize - 1].y, vertice.z + 1));
                }

            }
            if (vertice.x != xSize - 1)
            {
                result.Add(new Vector3(vertice.x + 1, vertices[verticeid + 1].y, vertice.z));

                if (vertice.z != 0)
                {
                    result.Add(new Vector3(vertice.x, vertices[verticeid - xSize + 1].y, vertice.z - 1));
                }
                if (vertice.z != zSize - 1)
                {
                    result.Add(new Vector3(vertice.x, vertices[verticeid + xSize].y, vertice.z + 1));
                }

            }
        }
        else
        {
            if (vertice.z != 0)
            {

                result.Add(new Vector3(vertice.x, vertices[verticeid - xSize ].y, vertice.z - 1));
                result.Add(new Vector3(vertice.x + 1, vertices[verticeid - xSize + 1].y, vertice.z - 1));
            }
            if (vertice.z != zSize - 1)
            {
                result.Add(new Vector3(vertice.x, vertices[verticeid + xSize - 1].y, vertice.z + 1));
                result.Add(new Vector3(vertice.x + 1, vertices[verticeid + xSize].y, vertice.z + 1));
            }
            if (vertice.x != 0)
            {
                result.Add(new Vector3(vertice.x - 1, vertices[verticeid - 1].y, vertice.z));
            }
            if (vertice.x != xSize - 2)
            {
                result.Add(new Vector3(vertice.x + 1, vertices[verticeid + 1].y, vertice.z));

            }
        }
        
        return result;
    }
    public Vector3 NextVertice(Vector3 vertice)
    {
        int verticeid = GetVerticeId(vertice);
        float targety = vertices[verticeid].y;
        float miny = targety;
        Vector3 minvertice =vertice;
        List<Vector3> verts = GetSurroundingVertices(vertice);
        foreach(var vert in verts)
        {
            if (vert.y < miny)
            {
                miny = vert.y;
                minvertice = vert;
            }
        }
        return minvertice;
    }

    public bool IsOnEdge(Vector3 vertice)
    { 
        if(vertice.x <= 1)
            return true;
        if (vertice.z <= 1)
            return true;
        if (vertice.x >= xSize -2 )
            return true;
        if (vertice.z >= zSize-1)
            return true;
        return false;

    }

    public void MoveVertice(Vector3 vertice, float deltaY, float flattenfactor,int brushsize = 1)
    {

        /*
        int verticeid = GetVerticeId(vertice);
        vertices[verticeid].y += deltaY;
        List<Vector3> sv = GetSurroundingVertices(vertice);
        foreach(Vector3 v in sv)
        {
            int newverticeid = GetVerticeId(v);
            vertices[newverticeid].y += deltaY/ flattenfactor;
        }
        */
        float currentY = deltaY;
        List<Vector3> Queue = new List<Vector3>();
        List<Vector3> NewQueue = new List<Vector3>();
        List<Vector3> Memory = new List<Vector3>();

        Queue.Add(vertice);

        for (int i = 0; i < brushsize; i++)
        {
            NewQueue.Clear();

          
            foreach (Vector3 v in Queue)
            {

                if (Memory.Contains(v) || IsOnEdge(v))
                {
                    continue;
                }
                int verticeid = GetVerticeId(v);
                
                Memory.Add(v);
                
                vertices[verticeid].y += currentY;
                NewQueue.AddRange(GetSurroundingVertices(v));
            }
            Queue.Clear();
            Queue.AddRange(NewQueue);
            
            currentY /= flattenfactor;
        }
        
        mesh.vertices = vertices;
    }

    public void FlattenVertice(Vector3 vertice, float deltaY, int brushsize = 1)
    {
        /*
        int verticeid = GetVerticeId(vertice);
        vertices[verticeid].y += deltaY;
        List<Vector3> sv = GetSurroundingVertices(vertice);
        foreach(Vector3 v in sv)
        {
            int newverticeid = GetVerticeId(v);
            vertices[newverticeid].y += deltaY/ flattenfactor;
        }
        */
        if (deltaY < 0)
        {
            return;
        }
        List<Vector3> Queue = new List<Vector3>();
        List<Vector3> NewQueue = new List<Vector3>();
        List<Vector3> Memory = new List<Vector3>();
        float totalY = 0;
        Queue.Add(vertice);

        for (int i = 0; i < brushsize; i++)
        {
            NewQueue.Clear();


            foreach (Vector3 v in Queue)
            {

                if (Memory.Contains(v) || IsOnEdge(v))
                {
                    continue;
                }
                int verticeid = GetVerticeId(v);

                Memory.Add(v);

                totalY += vertices[verticeid].y;
                NewQueue.AddRange(GetSurroundingVertices(v));
            }
            Queue.Clear();
            Queue.AddRange(NewQueue);

        }

        float targetY = totalY / Memory.Count;
        
        float ratio = deltaY;
        Queue = new List<Vector3>();
        NewQueue = new List<Vector3>();
        Memory = new List<Vector3>();

        Queue.Add(vertice);

        for (int i = 0; i < brushsize; i++)
        {
            NewQueue.Clear();


            foreach (Vector3 v in Queue)
            {

                if (Memory.Contains(v) || IsOnEdge(v))
                {
                    continue;
                }
                int verticeid = GetVerticeId(v);

                Memory.Add(v);

                vertices[verticeid].y += (targetY- vertices[verticeid].y) *ratio;
                NewQueue.AddRange(GetSurroundingVertices(v));
            }
            Queue.Clear();
            Queue.AddRange(NewQueue);
            ratio *= (brushsize-i)/(float)brushsize;
        }

        mesh.vertices = vertices;
        
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[xSize*zSize-zSize/2];
        print("yu");

        for (int i = 0, z = 0; z < zSize; z++)
        {

            for (int x = 0; x < xSize-z%2; x++)
            {
                float newX =  x + (z % 2) * 0.5f;
                float newZ =  z * 0.866f;
                // try cos
                float newY = CustomPerlinNoiseFunction(transform.position.x + newX, transform.position.z + newZ)  ;
                vertices[i]=new Vector3(newX, newY, newZ);
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

    public void CalculateNormal()
    {
        mesh.RecalculateNormals();
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
        return Mathf.PerlinNoise(x*Frequency,y*Frequency)*Amplitude + Mathf.PerlinNoise(x * Frequency1, y * Frequency1) * Amplitude1 + Mathf.PerlinNoise(x * Frequency2, y * Frequency2) * Amplitude2 + Mathf.PerlinNoise(x * Frequency3, y * Frequency3) * Amplitude3;
    }
    private void OnDrawGizmos()
    {
        foreach (var point in vertices)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }



}
