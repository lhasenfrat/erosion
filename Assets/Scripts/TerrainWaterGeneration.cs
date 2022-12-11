using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainWaterGeneration : MonoBehaviour
{
    public GameObject[][] terrains;
 

    public void produceDroplet()
    {
        
            terrains = gameObject.GetComponent<TerrainGeneration>().terrainList;
            for (int i = 0; i < terrains.Length; i++)
            {
                for (int j = 0; j < terrains[0].Length; j++)
                {
                    terrains[i][j].GetComponent<WaterManager>().produceWater();
                }
            }
        
    }
    public void generateRain()
    {
        terrains = gameObject.GetComponent<TerrainGeneration>().terrainList;
        for (int i = 0; i < terrains.Length; i++)
        {
            for(int j = 0; j < terrains[0].Length; j++)
            {
                terrains[i][j].GetComponent<WaterManager>().produceRain();
            }
        }
    }
    
}
