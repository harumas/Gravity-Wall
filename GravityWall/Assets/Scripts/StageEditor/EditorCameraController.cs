using System.Collections.Generic;
using System.Runtime.InteropServices;
using GravityWall;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace StageEditor
{
    public class EditorCameraController
    {
        private static GameObject currentObject;
        private static bool perspectiveMode;
        private static bool previousFramePressed;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += ManualUpdate;
        }

        private static void ManualUpdate()
        {
            bool currentFramePressed = Keyboard.current.cKey.IsPressed();

            if (currentFramePressed && !previousFramePressed)
            {
                SwitchPerspectiveMode();
            }

            previousFramePressed = currentFramePressed;

            if (perspectiveMode)
            {
                OnSceneCameraGUI(SceneView.lastActiveSceneView);
            }
        }

        private static void SwitchPerspectiveMode()
        {
            perspectiveMode = !perspectiveMode;

            if (!perspectiveMode && currentObject != null)
            {
                currentObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
                currentObject = null;
            }
        }

        private static void OnSceneCameraGUI(SceneView sceneView)
        {
            Vector3 origin = sceneView.camera.transform.position;
            Vector3 forward = sceneView.camera.transform.forward;

            GameObject hitObject = null;

            if (Physics.Raycast(origin, forward, out RaycastHit hitInfo, 1000f, Layer.Mask.Base))
            {
                hitObject = hitInfo.transform.gameObject;
            }

            if (hitObject != currentObject)
            {
                if (hitObject != null)
                {
                    hitObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }

                if (currentObject != null)
                {
                    currentObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
                }

                currentObject = hitObject;
            }
        }
    }
}