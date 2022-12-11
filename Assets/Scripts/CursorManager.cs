using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CursorManager : MonoBehaviour
{
    public GameObject selectedObject;
    public Text RadiusText;
    public Text StrengthText;

    public string mode = "add";
    public int radius = 2;
    Ray ray;
    RaycastHit hitData;
    Vector2 size;
    Vector2 TerrainSize;
    [SerializeField] float strength;
    private void Start()
    {
        size = Component.FindObjectOfType<TerrainGeneration>().Size; 
        TerrainSize = Component.FindObjectOfType<TerrainGeneration>().TerrainSize;

    }
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData) && Input.GetMouseButton(0)  )
        {
            selectedObject = hitData.transform.gameObject;
            string name = selectedObject.name;
            string[] coordonates = name.Split('-');
            List<GameObject> AdjacentCases = new List<GameObject>();
            if (Int64.Parse(coordonates[0]) > 0)
            {
                AdjacentCases.Add(GameObject.Find((Int64.Parse(coordonates[0]) - 1).ToString() + '-' + coordonates[1]));
            }
            if (Int64.Parse(coordonates[1]) > 0)
            {
                AdjacentCases.Add(GameObject.Find((coordonates[0] + '-' + (Int64.Parse(coordonates[1]) - 1).ToString())));
            }
            if (Int64.Parse(coordonates[0]) < size.x)
            {

                AdjacentCases.Add(GameObject.Find((Int64.Parse(coordonates[0]) + 1).ToString() + '-' + coordonates[1]));
            }
            if (Int64.Parse(coordonates[1]) < size.y)
            {

                AdjacentCases.Add(GameObject.Find((coordonates[0] + '-' + (Int64.Parse(coordonates[1]) + 1).ToString())));
            }

            Vector3 ImpactPoint = hitData.point;
            ImpactPoint-= new Vector3(Int64.Parse(coordonates[0])* TerrainSize[0], 0, Int64.Parse(coordonates[1])* TerrainSize[1]);
            Vector3 vgrid = new Vector3((int)ImpactPoint.x, ImpactPoint.y, (int)(ImpactPoint.z / 0.866f));
            if (mode == "add")
            {
                selectedObject.GetComponent<MeshGenerator>().MoveVertice(vgrid, strength, 1.1f, radius);

            } else if (mode == "flatten")
            {
                selectedObject.GetComponent<MeshGenerator>().FlattenVertice(vgrid, strength, radius);

            }
            selectedObject.GetComponent<MeshGenerator>().CalculateNormal();

        }
    }

    public void changeRadius(float newValue)
    {
        RadiusText.text = newValue.ToString();
        radius = (int)newValue;

    }

    public void changeStrengh(float newValue)
    {
        StrengthText.text = newValue.ToString();
        strength = newValue;

    }

    public void SetMode(string newmode)
    {
        mode = newmode; 
    }
}
