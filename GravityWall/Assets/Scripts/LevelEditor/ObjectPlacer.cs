using GravityWall;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using EditorUtility = UnityEditor.ProBuilder.EditorUtility;
using HandleUtility = UnityEditor.HandleUtility;

namespace LevelEditor
{
    /// <summary>
    /// オブジェクトをマウス操作で設置できるようにするカメラ
    /// </summary>
    public class ObjectPlacer
    {
        private GameObject gameObject;
        private Renderer renderer;
        private Transform rootParent;

        public void Initialize()
        {
            EditorUtility.meshCreated += OnProBuilderMeshCreated;
        }

        private void OnProBuilderMeshCreated(ProBuilderMesh proBuilderMesh)
        {
            GameObject obj = proBuilderMesh.gameObject;
            obj.transform.SetParent(rootParent);
            obj.tag = Tag.Wall;
        }

        public void StartPlacingSequence(GameObject prefab)
        {
            //仮オブジェクトの生成
            gameObject = Object.Instantiate(prefab, rootParent);
            gameObject.layer = Layer.IgnoreRaycast;

            renderer = gameObject.GetComponent<Renderer>();

            SceneView.duringSceneGui += SequenceObjectPlace;
        }

        public void SetNewScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return;
            }

            GameObject levelObject = GameObject.Find("Level");

            if (levelObject == null)
            {
                levelObject = new GameObject("Level");
                SceneManager.MoveGameObjectToScene(levelObject, scene);
            }

            rootParent = levelObject.transform;
        }

        private void SequenceObjectPlace(SceneView sceneView)
        {
            Event ev = Event.current;

            if (gameObject == null)
            {
                SceneView.duringSceneGui -= SequenceObjectPlace;
                return;
            }

            if (ev.type == EventType.MouseMove)
            {
                //マウス座標からワールド座標のレイに変換
                Ray ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
                float distance = 30f;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, distance))
                {
                    //地面にBoundsがぴったりくっつくように修正
                    Vector3 correction = Vector3.Scale(renderer.bounds.extents, hitInfo.normal);
                    gameObject.transform.position = Snapping.Snap(hitInfo.point + correction, EditorSnapSettings.move);
                }
                else
                {
                    Vector3 point = ray.origin + ray.direction * distance;
                    gameObject.transform.position = point;
                }
            }

            bool isSet = ev.type == EventType.MouseDown && ev.button == 0;

            //設置した or Shiftキーでキャンセルした
            if (isSet || ev.shift)
            {
                if (isSet)
                {
                    PlaceObject();
                }

                SceneView.duringSceneGui -= SequenceObjectPlace;
                
                //仮オブジェクトの破棄
                Object.DestroyImmediate(gameObject);
            }
        }

        private void PlaceObject()
        {
            GameObject newObject = Object.Instantiate(gameObject, rootParent);
            newObject.name = "New Object";
            newObject.layer = Layer.Default;

            Undo.RegisterCreatedObjectUndo(newObject, "New Object");
        }
    }
}