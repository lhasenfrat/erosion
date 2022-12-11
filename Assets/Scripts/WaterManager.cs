using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.GraphicsBuffer;
using Color = UnityEngine.Color;

public class WaterManager : MonoBehaviour
{
    public float gravity;
    public float erodespeed;
    public float depositspeed;
    public float initialSpeed;
    public float initialWaterVolume;
    public float dropletstrength;
    public float flattenfactor;
    MeshGenerator meshGenerator;
    List<Vector3> printablePoints;
    List<float> amounts;
    List<Color> colors;
    public bool rainactive;
    void Start()
    {
        meshGenerator = gameObject.GetComponent<MeshGenerator>();
        printablePoints = new List<Vector3>();
        amounts = new List<float>();
        colors = new List<Color>(); 
    }

    public void produceRain()
    {
        rainactive = !rainactive;
        StartCoroutine(watercoroutine());
        
    }
    public IEnumerator watercoroutine()
    {
        while (rainactive)
        {
            produceWater();
            yield return null;
        }
    }
    public void produceWater()
    {
        Vector3 lastpoint = meshGenerator.RandomVertice();
        printablePoints.Add(meshGenerator.GetVertice(lastpoint));
        colors.Add(Color.blue);
        Vector3 nextpoint = meshGenerator.NextVertice(lastpoint);
        float speed = initialSpeed;
        float water = initialWaterVolume;
        float sediment = 0;
        float deltay = 0;
        float sedimentCapacity;
        float amountToDeposit;
        for (int i = 0; i < 50; i++)
        {
            if (nextpoint == lastpoint)
                break;
            if (nextpoint.y <= 0)
                break;
            //printablePoints.Add(meshGenerator.GetVertice(nextpoint));
            deltay =(nextpoint.y - lastpoint.y);
            sedimentCapacity = (-deltay * speed * water);

            if (sediment > sedimentCapacity)
            {

                amountToDeposit = DeltaHeight(lastpoint)>0?Mathf.Min(sediment, deltay) : Mathf.Max((sediment - sedimentCapacity)*depositspeed,deltay);
                
                /*
                if (DeltaHeight(lastpoint) > 0)
                {
                    colors.Add(Color.red);

                }
                else
                {
                    colors.Add(Color.yellow);

                }
                */

                sediment -= amountToDeposit;
                meshGenerator.MoveVertice(lastpoint, amountToDeposit, flattenfactor);
                amounts.Add(-amountToDeposit);

            }
            else
            {
                amountToDeposit = Mathf.Min((sedimentCapacity - sediment)*erodespeed, -deltay);
                sediment += amountToDeposit;
                meshGenerator.MoveVertice(lastpoint, -amountToDeposit, flattenfactor);
                //colors.Add(Color.blue);
                amounts.Add(amountToDeposit);

            }




            lastpoint = nextpoint;
            nextpoint = meshGenerator.NextVertice(lastpoint);
            water /= 1.1f;
        }

        meshGenerator.MoveVertice(lastpoint, sediment, flattenfactor);
        meshGenerator.CalculateNormal();
        
    }

    public float DeltaHeight(Vector3 point)
    {
        List<Vector3> surroundingpoints = meshGenerator.GetSurroundingVertices(point);
        float miny = Mathf.Infinity;

        foreach (Vector3 p in surroundingpoints)
        {
            if (p.y < miny)
            {
                miny = p.y;
            }
        }
        return miny-point.y;
       
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < amounts.Count; i++)
        {

            Gizmos.color = colors[i];

            Gizmos.DrawSphere(printablePoints[i], 1);

        }
                
        
    }
}
