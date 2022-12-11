using UnityEngine;
public class TerrainGeneration : MonoBehaviour 
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject myPrefab;

    public Vector2 Size = new Vector2(5,5);
    [SerializeField] public Vector2 TerrainSize = new Vector2(50, 50);

    public GameObject[][] terrainList;
    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        terrainList = new GameObject[(int)Size.x][];
        for (int i = 0; i < terrainList.Length; i++)
        {
            terrainList[i] = new GameObject[(int)Size.y];
            for (int j = 0; j < terrainList[0].Length; j++)
            {
                terrainList[i][j] = Instantiate(myPrefab, new Vector3(TerrainSize.x * i, 0, TerrainSize.y * j), Quaternion.identity) ;
                terrainList[i][j].name = i.ToString()+"-"+j.ToString() ;


            }
        }
    }


    public void changeAmplitude(int level, float newvalue)
    {
        for (int i = 0; i < terrainList.Length; i++)
        {
            for (int j = 0; j < terrainList[0].Length; j++)
            {
                terrainList[i][j].GetComponent<MeshGenerator>().Amplitude[level] = newvalue;

                terrainList[i][j].GetComponent<MeshGenerator>().UpdateTopology();
            }
        }
    }

    public void changeFrequency(int level, float newvalue)
    {

        for (int i = 0; i < terrainList.Length; i++)
        {
            for (int j = 0; j < terrainList[0].Length; j++)
            {

                terrainList[i][j].GetComponent<MeshGenerator>().Frequency[level] = newvalue;
                terrainList[i][j].GetComponent<MeshGenerator>().UpdateTopology();
            }
        }
    }
}