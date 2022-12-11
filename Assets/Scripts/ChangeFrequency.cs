using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeAmplitude : MonoBehaviour
{
    public Text changeText;
    public GameObject Terrain;
    public int level;
    // Update is called once per frame
    public void changeValue(float newValue)
    {
        changeText.text = newValue.ToString();
        Terrain.GetComponent<TerrainGeneration>().changeAmplitude(level,newValue);
    }
}