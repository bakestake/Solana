using System.IO;
using UnityEngine;

namespace Gamegaard.RatingSystem.SaveSystem
{
    public static class BasicJSONSaveSystem
    {
        private const string extension = ".json";
        public static string DefaultPath => $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}";

        /// <summary>
        /// Saves the specified data to a JSON file with the given name and path.
        /// </summary>
        /// <typeparam name="T">The type of data being saved.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="fileName">The name of new save file.</param>
        /// <param name="path">The path where the file should be saved (default = DefaultPath).</param>
        public static void SaveData<T>(T data, string fileName, string path = "")
        {
            string basicPath = string.IsNullOrWhiteSpace(path) ? DefaultPath : path;
            string savePath = GetPathValue(basicPath, fileName, extension);
            string json = JsonUtility.ToJson(data);

#if UNITY_2020_3_OR_NEWER
            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
#else
            using (StreamWriter writer = new StreamWriter(savePath))
            {
                writer.Write(json);
            }
#endif
        }

        /// <summary>
        /// Loads the data from a file with the given name and path.
        /// </summary>
        /// <typeparam name="T">The type of data being loaded.</typeparam>
        /// <param name="fileName">The name of the file to load the data from.</param>
        /// <param name="saveData">The loaded data.</param>
        /// <param name="path">The path where the file is located (default = DefaultPath).</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file is not found.</exception>
        public static void LoadData<T>(string fileName, out T saveData, string path = "") where T : new()
        {
            string basicPath = string.IsNullOrWhiteSpace(path) ? DefaultPath : path;
            string loadPath = GetPathValue(basicPath, fileName, extension);
            try
            {
#if UNITY_2020_3_OR_NEWER
                using StreamReader reader = new StreamReader(loadPath);
                string json = reader.ReadToEnd();
#else
                string json;
                using (StreamReader reader = new StreamReader(loadPath))
                {
                    json = reader.ReadToEnd();
                }
#endif
                if (string.IsNullOrEmpty(json))
                {
                    saveData = new T();
                };
                saveData = JsonUtility.FromJson<T>(json);
            }
            catch
            {
                saveData = new T();
            }
        }

        public static void LoadUnsafeData<T>(string fileName, out T saveData, string path = "") where T : class, new()
        {
            string basicPath = string.IsNullOrWhiteSpace(path) ? DefaultPath : path;
            string loadPath = GetPathValue(basicPath, fileName, extension);

            try
            {
#if UNITY_2020_3_OR_NEWER
                using StreamReader reader = new StreamReader(loadPath);
                string json = reader.ReadToEnd();
#else
                string json;
                using (StreamReader reader = new StreamReader(loadPath))
                {
                    json = reader.ReadToEnd();
                }
#endif
                saveData = JsonUtility.FromJson<T>(json);
            }
            catch
            {
                saveData = null;
            }
        }

        /// <summary>
        /// Returns the full file path with the specified name and extension within the given path.
        /// </summary>
        /// <param name="path">The base path.</param>
        /// <param name="name">The file name.</param>
        /// <param name="extension">The file extension (e.g. ".txt", ".json").</param>
        /// <returns>The full file path with the specified name and extension within the given path.</returns>
        private static string GetPathValue(string path, string name, string extension)
        {
            return path + name + extension;
        }

        /// <summary>
        /// Deletes the data file with the given filename if it exists.
        /// </summary>
        /// <param name = "fileName" > The name of the data file to delete.</param>
        public static void DeleteData(string fileName)
        {
            string path = GetPathValue(DefaultPath, fileName, extension);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Checks if there is a data file with the given filename in the default directory.
        /// </summary>
        /// <param name = "fileName" > The name of the data file to check.</param>
        /// <returns>True if the data file exists, otherwise False.</returns>
        public static bool HasDataInProject(string fileName)
        {
            string path = GetPathValue(DefaultPath, fileName, extension);
            return File.Exists(path);
        }
    }
}