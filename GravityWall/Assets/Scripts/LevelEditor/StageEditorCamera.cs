using GravityWall;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace LevelEditor
{
    /// <summary>
    /// 壁を透過してステージの見通しを良くするカメラ
    /// </summary>
    public static class StageEditorCamera
    {
        private static GameObject currentObject;
        private static bool perspectiveMode;
        private static bool previousFramePressed;
        private static bool doUpdate;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += EditorUpdate;

            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    //プレイモード中は透過を行わないようにする
                    ShowObject(currentObject);
                    currentObject = null;
                    doUpdate = false;
                }
                else if (state == PlayModeStateChange.EnteredEditMode)
                {
                    doUpdate = true;
                }
            };
        }

        private static void EditorUpdate()
        {
            if (!doUpdate)
            {
                return;
            }

            bool currentFramePressed = Keyboard.current.cKey.IsPressed();

            // Cキーを押した瞬間
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

            //前の壁オブジェクトを取得
            if (Physics.Raycast(origin, forward, out RaycastHit hitInfo, 1000f, Layer.Mask.Base | Layer.Mask.IgnoreRaycast))
            {
                hitObject = hitInfo.transform.gameObject;
            }

            if (hitObject != currentObject)
            {
                //今のフレームで取得したオブジェクトは非表示にする
                HideObject(hitObject);

                //前のフレームで取得したオブジェクトはもとに戻す
                ShowObject(currentObject);

                currentObject = hitObject;
            }
        }

        private static void HideObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.layer = Layer.IgnoreRaycast;
                gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        private static void ShowObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.layer = Layer.Base;
                gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }
}