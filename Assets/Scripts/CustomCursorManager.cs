using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CustomCursorManager : MonoBehaviour
{
    public GameObject selectedObject;
    

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
            
            Vector3 ImpactPoint = hitData.point;
            ImpactPoint-= new Vector3(TerrainSize[0], 0,  TerrainSize[1]);
            Vector3 vgrid = new Vector3((int)ImpactPoint.x, ImpactPoint.y, (int)(ImpactPoint.z / 0.866f));
            if (mode == "add")
            {
                selectedObject.GetComponent<CustomMeshGenerator>().MoveVertice(vgrid, strength, 1.1f, radius);

            } else if (mode == "flatten")
            {
                selectedObject.GetComponent<CustomMeshGenerator>().FlattenVertice(vgrid, strength, radius);

            }
            selectedObject.GetComponent<CustomMeshGenerator>().CalculateNormal();

        }
    }

    public void changeRadius(float newValue)
    {
        radius = (int)newValue;

    }

    public void changeStrengh(float newValue)
    {
        strength = newValue;

    }

    public void SetMode(string newmode)
    {
        mode = newmode; 
    }
}
