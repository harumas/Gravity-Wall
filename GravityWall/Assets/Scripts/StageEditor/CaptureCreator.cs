using System.IO;
using UnityEngine;

namespace StageEditor
{
    public static class CaptureCreator
    {
        private const int ThumbnailWidth = 512;
        private const int ThumbnailHeight = 512;
        private const string SavePath = "Assets/Thumbnails/";

        private static Camera thumbnailCamera;
        private static RenderTexture renderTexture;

        public static Texture2D[] GetThumbnails(GameObject[] objects)
        {
            Texture2D[] thumbnails = new Texture2D[objects.Length];

            InitializeCamera();

            for (var i = 0; i < objects.Length; i++)
            {
                thumbnails[i] = GenerateThumbnail(objects[i]);
            }

            CleanupCamera();

            return thumbnails;
        }

        private static void InitializeCamera()
        {
            // カメラを生成
            GameObject cameraObject = new GameObject("ThumbnailCamera");
            thumbnailCamera = cameraObject.AddComponent<Camera>();

            // RenderTextureの設定
            renderTexture = new RenderTexture(ThumbnailWidth, ThumbnailHeight, 24);
            thumbnailCamera.targetTexture = renderTexture;
            thumbnailCamera.clearFlags = CameraClearFlags.Depth;
            thumbnailCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            thumbnailCamera.fieldOfView = 50;
            thumbnailCamera.allowMSAA = true;
            thumbnailCamera.allowHDR = false;
            RenderTexture.active = renderTexture;
        }

        private static void CleanupCamera()
        {
            // 後片付け
            thumbnailCamera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(renderTexture);

            // カメラを破棄
            Object.DestroyImmediate(thumbnailCamera.gameObject);
        }

        private static Texture2D GenerateThumbnail(GameObject prefab)
        {
            // Prefabのインスタンスを生成
            GameObject instance = Object.Instantiate(prefab, new Vector3(10000, 10000, 10000), Quaternion.Euler(-20f, 30f, -10f));

            // カメラの位置と向きをターゲットオブジェクトに向ける
            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                thumbnailCamera.transform.position = bounds.center + new Vector3(0, 0, -2.3f); // 適切な位置に設定
                thumbnailCamera.transform.LookAt(instance.transform);
            }
            else
            {
                Debug.LogError("Prefab does not have a Renderer component: " + prefab.name);
            }

            // カメラでレンダリング
            thumbnailCamera.Render();

            // Texture2DにRenderTextureの内容をコピー
            Texture2D thumbnail = new Texture2D(ThumbnailWidth, ThumbnailHeight, TextureFormat.RGBA32, false);
            thumbnail.ReadPixels(new Rect(0, 0, ThumbnailWidth, ThumbnailHeight), 0, 0);
            thumbnail.filterMode = FilterMode.Bilinear;
            thumbnail.alphaIsTransparency = true;
            thumbnail.Apply();

            // サムネイルをPNG形式で保存
            byte[] bytes = thumbnail.EncodeToPNG();
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            string fileName = prefab.name + "_thumbnail.png";
            File.WriteAllBytes(Path.Combine(SavePath, fileName), bytes);

            // インスタンスを破棄
            Object.DestroyImmediate(instance);

            return thumbnail;
        }
    }
}