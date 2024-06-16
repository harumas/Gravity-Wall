using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GravityWall;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.Rendering;
using EditorUtility = UnityEditor.ProBuilder.EditorUtility;

public class EditorCameraController 
{
    private static readonly HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
    private static readonly HashSet<GameObject> hittingObjects = new HashSet<GameObject>();
    private static readonly HashSet<GameObject> calculateSets = new HashSet<GameObject>();
    private static readonly RaycastHit[] hitBuffer = new RaycastHit[32];

    [InitializeOnLoadMethod]
    private static void Init()
    {
        EditorApplication.update += ManualUpdate;
    }

    private static void ManualUpdate()
    {
        OnSceneCameraGUI(SceneView.lastActiveSceneView);
    }

    private static void OnSceneCameraGUI(SceneView sceneView)
    {
        Vector3 origin = sceneView.camera.transform.position;
        Vector3 forward = sceneView.camera.transform.forward;

        const int allMask = ~0;
        int count = Physics.SphereCastNonAlloc(origin, 0.1f, forward, hitBuffer, float.PositiveInfinity, allMask);

        if (count == 0)
        {
            hittingObjects.Clear();
            return;
        }

        detectedObjects.Clear();

        GameObject farthest = null;
        float farthestDistance = 0f;
        for (int i = 0; i < count; i++)
        {
            GameObject hit = hitBuffer[i].transform.gameObject;

            if (!hit.CompareTag(Tag.Wall))
            {
                continue;
            }

            float distance = (hit.transform.position - origin).sqrMagnitude;

            if (distance > farthestDistance)
            {
                farthest = hit;
                farthestDistance = distance;
            }

            detectedObjects.Add(hit);
        }

        foreach (GameObject hitObject in detectedObjects)
        {
            hitObject.layer = Layer.IgnoreRaycast;
            hitObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        if (farthest != null)
        {
            farthest.layer = Layer.Default;
            farthest.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
        }

        calculateSets.Clear();
        foreach (GameObject detectedObject in hittingObjects)
        {
            calculateSets.Add(detectedObject);
        }

        calculateSets.ExceptWith(detectedObjects);
        foreach (GameObject hitObject in calculateSets)
        {
            farthest.layer = Layer.Default;
            hitObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
        }

        hittingObjects.Clear();
        foreach (GameObject detectedObject in detectedObjects)
        {
            hittingObjects.Add(detectedObject);
        }
    }
}