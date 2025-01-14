using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreModule.Save
{
    /// <summary>
    /// セーブ機能を提供するクラス
    /// </summary>
    public static class SaveUtility
    {
        //セーブ場所
        private static readonly string SaveDir = Path.Combine(Application.persistentDataPath, "SaveData");

        private static string NameToFilePath(string name) => Path.Combine(SaveDir, name + ".json");

        public static bool FileExists(string name)
        {
            return File.Exists(NameToFilePath(name));
        }

        public static bool DirExists(string name)
        {
            return Directory.Exists(Application.dataPath + name);
        }

        //オブジェクトをJsonに変換してセーブ
        public static async Task Save(object obj, string name, bool format = false)
        {
            if (!DirExists("SaveData"))
            {
                Directory.CreateDirectory(SaveDir);
            }

            string text = obj.GetType().IsPrimitiveOrString() ? obj.ToString() : JsonUtility.ToJson(obj, format);

            await using (StreamWriter sw = new StreamWriter(NameToFilePath(name), false))
            {
                try
                {
                    await sw.WriteLineAsync(text);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to save the file!");
                    Debug.LogException(e);
                    throw;
                }
            }
        }

        //セーブデータを削除
        public static void Delete(string name)
        {
            if (FileExists(name))
            {
                File.Delete(NameToFilePath(name));
            }
        }

        //System.Stringで読み込む場合はこっちだけ
        public static async Task<string> LoadRaw(string name)
        {
            string rawData = null;

            try
            {
                using FileStream fs = new FileStream(NameToFilePath(name), FileMode.Open);

                using (StreamReader sr = new StreamReader(fs))
                {
                    rawData = await sr.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load the file!");
                Debug.LogException(e);
            }

            return rawData;
        }

        //System.String, ScriptableObject, MonoBehaviourクラス以外で利用可能なロード機能
        public static async Task<T> Load<T>(string name)
        {
            T deserializedData = default;

            try
            {
                using FileStream fs = new FileStream(NameToFilePath(name), FileMode.Open);

                using (StreamReader sr = new StreamReader(fs))
                {
                    string result = await sr.ReadToEndAsync();

                    bool isPrimitive = typeof(T).IsPrimitive;
                    deserializedData = isPrimitive ? GenericParser.Parse<T>(result) : JsonUtility.FromJson<T>(result);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load the file!");
                Debug.LogException(e);
            }

            return deserializedData;
        }

        //ScriptableObjectを保存する場合はこっちを利用する
        public static async Task LoadOverwrite<T>(string name, T scriptableObject) where T : ScriptableObject
        {
            try
            {
                using FileStream fs = new FileStream(NameToFilePath(name), FileMode.Open);

                using (StreamReader sr = new StreamReader(fs))
                {
                    string result = await sr.ReadToEndAsync();

                    JsonUtility.FromJsonOverwrite(result, scriptableObject);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load the file!");
                Debug.LogException(e);
            }
        }

        public static bool IsPrimitiveOrString(this Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }
    }

    public static class GenericParser
    {
        /// <summary>
        /// stringをTのオブジェクトに変換する
        /// </summary>
        /// <param name="input">オブジェクトに変換したいstring</param>
        /// <typeparam name="T">変換後のオブジェクトの型</typeparam>
        /// <returns>変換後のオブジェクト</returns>
        public static T Parse<T>(string input)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(input);
        }

        public static bool TryParse<T>(string input, ref T result)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                result = (T)converter.ConvertFromString(input);
                return true;
            }
            catch
            {
                Debug.Log($"指定された型{typeof(T)}にはConverterがありません！");
                return false;
            }
        }
    }
}