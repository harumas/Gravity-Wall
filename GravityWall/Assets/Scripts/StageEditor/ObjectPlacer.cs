using GravityWall;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using EditorUtility = UnityEditor.ProBuilder.EditorUtility;

namespace StageEditor
{
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
            Debug.Log(proBuilderMesh);
            GameObject obj = proBuilderMesh.gameObject;
            obj.transform.SetParent(rootParent);
            obj.tag = Tag.Wall;
        }

        public void PlaceObject(GameObject prefab)
        {
            gameObject = Object.Instantiate(prefab, rootParent);
            gameObject.layer = Layer.IgnoreRaycast;

            renderer = gameObject.GetComponent<Renderer>();

            SceneView.duringSceneGui += SequenceObjectPlace;
        }

        public void SetNewScene(Scene scene)
        {
            GameObject root = new GameObject("Level");
            SceneManager.MoveGameObjectToScene(root, scene);

            rootParent = root.transform;
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
                var mousePos = new Vector3(ev.mousePosition.x,
                    Camera.current.pixelHeight - ev.mousePosition.y,
                    0);

                Ray ray = Camera.current.ScreenPointToRay(mousePos);
                float distance = 30f;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, distance))
                {
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

            if (isSet || ev.shift)
            {
                if (isSet)
                {
                    GameObject newObject = Object.Instantiate(gameObject);
                    newObject.name = "New Object";
                    newObject.layer = Layer.Default;

                    Undo.RegisterCreatedObjectUndo(newObject, "New Object");
                }

                SceneView.duringSceneGui -= SequenceObjectPlace;
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}