using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProBuilderMeshFinder
{
    [MenuItem("MyMenu/Do Something")]
    static void Start()
    {
        var allMeshFilters = Object.FindObjectsOfType<MeshFilter>();

        foreach (MeshFilter filter in allMeshFilters)
        {
            Mesh mesh = filter.mesh;

            //Debug.Log("Mesh on GameObject " + filter.name + " is called " + mesh.name);

            if (mesh.name == "pb_Mesh5917382")
            {
                Debug.Log("Gameobject " + filter.name + " seems to be the one you want!");
            }
        }
    }
}