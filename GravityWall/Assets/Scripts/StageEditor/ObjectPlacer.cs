using Cysharp.Threading.Tasks;
using GravityWall;
using UnityEditor;
using UnityEngine;

namespace StageEditor
{
    public class ObjectPlacer
    {
        private GameObject gameObject;
        private Renderer renderer;

        public void PlaceObject(GameObject prefab)
        {
            gameObject = Object.Instantiate(prefab);
            gameObject.layer = Layer.IgnoreRaycast;

            renderer = gameObject.GetComponent<Renderer>();

            SceneView.duringSceneGui += SequenceObjectPlace;
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