using Gamegaard.SavingSystem.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.SavingSystem
{
    public static class SaveSystem
    {
        public const string defaultSaveName = "Save";
        private const string jsonExtension = ".json";
        private const string bkpExtension = ".bak";
        private readonly static string dataPath = Application.persistentDataPath;

        private static string GetExtension(string customExtension = null)
        {
            if (customExtension == null)
            {
                customExtension = jsonExtension;
            }
            return customExtension;
        }

        #region Save
        // Saves the value to a file with the given key.
        // If the file already exists, the key will be added to the file.
        // If the key already exists in the file, the key will be overwritten in the file.
        public static void Save<T>(string key, T data, string fileName = null, string customExtension = null)
        {
            if (fileName == null)
            {
                fileName = defaultSaveName;
            }

            try
            {
                string filePath = GetFullPath(fileName + GetExtension(customExtension));
                JObject dataObj;

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    dataObj = JObject.Parse(json);
                }
                else
                {
                    dataObj = new JObject();
                }

                if (typeof(T) == typeof(Color))
                {
                    Color color = (Color)(object)data;
                    dataObj[key] = JToken.FromObject(new SerializableColor(color));
                }
                else
                {
                    dataObj[key] = JToken.FromObject(data);
                }

                string newData = dataObj.ToString();
                File.WriteAllText(filePath, newData);
                //Debug.Log($"Data saved to file '{filePath}' with key '{key}'.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while saving data: {ex.Message}");
            }
        }

        // Saves a byte array as a file, overwriting any existing files.
        public static void SaveRaw(byte[] bytes, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, bytes);
                //Debug.Log($"Byte array saved to file '{filePath}' successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while saving byte array as file: {ex.Message}");
            }
        }

        // Appends a byte array to a file, or creates a new file if one does not already exist.
        public static void AppendRaw(byte[] bytes, string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Append))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                //Debug.Log($"Byte array appended to file '{filePath}' successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while appending byte array to file: {ex.Message}");
            }
        }

        // Saves a Texture2D image to a file as PNG format.
        public static void SaveImage(Texture2D texture, string imagePath, ImageExtension extension = ImageExtension.png)
        {
            try
            {
                byte[] bytes = EncodeTextureToImageFormat(texture, extension);
                File.WriteAllBytes(imagePath, bytes);
                //Debug.Log($"Texture2D image saved to file '{imagePath}' successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while saving Texture2D image as file: {ex.Message}");
            }
        }

        private static byte[] EncodeTextureToImageFormat(Texture2D texture, ImageExtension extension)
        {
            try
            {
                switch (extension)
                {
                    case ImageExtension.png:
                        return texture.EncodeToPNG();
                    case ImageExtension.jpg:
                        return texture.EncodeToJPG();
                    case ImageExtension.tga:
                        return texture.EncodeToTGA();
                }
                Debug.LogError("texture extension mistake. Please ensure the extension is corret.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while ecoding Texture2D to image format: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Load
        // Loads the value from a file with the given key.
        // Unless you specify the defaultValue parameter, a KeyNotFoundException or FileNotFoundException will be thrown if the data does not exist.In this case, you can use SaveSystem.KeyExists or SaveSystem.File.Exists to check if the data exists before loading.
        // When loading strings it is recommended to use named parameters to ensure that the correct argument is used.
        public static T Load<T>(string key, T defaultValue = default, string fileName = null, string customExtension = null)
        {
            if (fileName == null)
            {
                fileName = defaultSaveName;
            }

            string filePath = GetFullPath(fileName + GetExtension(customExtension));
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var dataObj = JObject.Parse(json);

                if (dataObj.TryGetValue(key, out JToken value))
                {
                    return value.ToObject<T>();
                }
            }

            return defaultValue;
        }

        // Loads the value from a file with the given key into an existing object, rather than creating a new instance.
        // A KeyNotFoundException or FileNotFoundException will be thrown if the data does not exist.You can use SaveSystem.KeyExists or SaveSystem.File.Exists to check if the data exists before loading.
        // LoadInto can also accept an array (or List) as a parameter, and will load the data into each object in the array. The array must contain the same number of items as the array we are loading.
        public static void LoadInto<T>(string key, string filePath, T data)
        {

        }

        // Loads the raw bytes from a file.
        // A FileNotFoundException will be thrown if the file does not exist.
        public static byte[] LoadRawBytes(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{filePath}' does not exist.");
                }

                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while loading raw bytes from file: {ex.Message}");
                return null;
            }
        }

        // Loads an entire file as a string.
        // It will use the text encoding in the default settings(UTF8 by default) unless an SaveSettings object with a different encoding is provided.
        // A FileNotFoundException will be thrown if the file does not exist.
        public static string LoadRawString(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{filePath}' does not exist.");
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while loading raw string from file: {ex.Message}");
                return null;
            }
        }

        // Loads a JPG or PNG image file as a Texture2D.
        // A FileNotFoundException will be thrown if the file does not exist.
        // An ArgumentException will be thrown if the file extension is not jpg, jpeg, or png.
        public static Texture2D LoadImage(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException($"The image file '{imagePath}' does not exist.");
                }

                byte[] fileData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                if (!texture.LoadImage(fileData))
                {
                    throw new ArgumentException($"Invalid image file format or data: '{imagePath}'.");
                }

                return texture;
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while loading image file: {ex.Message}");
                return null;
            }
        }

        // Loads an audio file as an AudioClip.
        // MP3 files are only supported on mobile, and Ogg Vorbis files are only supported on standalone platforms.
        // WAV, XM, IT, MOD and S3M files are supported on all platforms except WebGL.
        // As this method requires file access, this method is not supported on WebGL.
        // A FileNotFoundException will be thrown if the file does not exist.In this case, you can use SaveSystem.File.Exists to check if the data exists before loading.
        public static AudioClip LoadAudio(string audioFilePath)
        {
            throw new System.Exception();
        }
        #endregion

        #region Exists
        // Checks whether a key exists in a file.
        public static bool KeyExists(string key, string filePath, string customExtension = null)
        {
            if (filePath == null)
            {
                filePath = GetFullPath(defaultSaveName + GetExtension(customExtension));
            }

            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.Log($"Error: The file '{filePath}' does not exist.");
                    return false;
                }

                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (line.Contains(key))
                    {
                        Debug.Log($"Key '{key}' exists in file '{filePath}'.");
                        return true;
                    }
                }

                Debug.Log($"Key '{key}' does not exist in file '{filePath}'.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while checking key existence: {ex.Message}");
                return false;
            }
        }

        // Checks whether a file exists.
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        // Checks whether a directory exists.
        public static bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        // Checa se o valor condiz com o arquivo de referencia. Usado para checar se o usuario alterou dados do save.
        //public static void CheckValues(string saveFileName)
        //{
        //    string configFilePath = Path.Combine(Application.persistentDataPath, "config.cfg");
        //    string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        //    if (File.Exists(filePath))
        //    {
        //        byte[] saveFileHash = GetFileHash(filePath);
        //        byte[] lastSaveFileHash = File.ReadAllBytes(configFilePath);

        //        if (!lastSaveFileHash.SequenceEqual(saveFileHash))
        //        {
        //            Debug.Log("Save file has been modified outside of the game!");

        //            // TODO: encontrar uma maneira de recuperar o valor original. Possivelmente, apenas com uma cópia do save. ?
        //        }
        //        else
        //        {
        //            Debug.Log("Save file is not modified.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("File not founded.");
        //    }
        //}
        #endregion

        #region Delete
        // Deletes a key from a file.
        // If the key or file does not exist, no exceptions will be thrown.
        public static void DeleteKey(string key, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.Log("Error: The specified file does not exist.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                var dataObj = JObject.Parse(json);

                if (dataObj.Remove(key))
                {
                    File.WriteAllText(filePath, dataObj.ToString());
                    Debug.Log($"Key '{key}' successfully deleted from file '{filePath}'.");
                }
                else
                {
                    Debug.Log($"Key '{key}' not found in file '{filePath}'. No changes were made.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while deleting the key: {ex.Message}");
            }
        }

        // Deletes a SaveFile.
        // If the file does not exist, no exceptions will be thrown.
        public static void DeleteSaveFile(string fileName, string customExtension = null)
        {
            string filePath = GetFullPath(fileName + GetExtension(customExtension));
            DeleteFile(filePath);
        }

        // Deletes a file.
        // If the file does not exist, no exceptions will be thrown.
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"File '{filePath}' deleted successfully.");
                }
                else
                {
                    Debug.Log($"File '{filePath}' does not exist. No changes were made.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while deleting the file: {ex.Message}");
            }
        }

        // Deletes a directory and its contents.
        // If the directory does not exist, no exceptions will be thrown.
        public static void DeleteDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                    Debug.Log($"Directory '{directoryPath}' and its contents deleted successfully.");
                }
                else
                {
                    Debug.Log($"Directory '{directoryPath}' does not exist. No changes were made.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while deleting the directory: {ex.Message}");
            }
        }
        #endregion

        #region Key, File, Directories
        public static void RenameFile(string oldFilePath, string newFilePath)
        {
            try
            {
                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                    Debug.Log($"File '{oldFilePath}' renamed to '{newFilePath}' successfully.");
                }
                else
                {
                    Debug.LogError($"Error: The file '{oldFilePath}' does not exist. Cannot rename.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while renaming the file: {ex.Message}");
            }
        }

        public static void CopyFile(string oldFilePath, string newFilePath)
        {
            try
            {
                if (File.Exists(oldFilePath))
                {
                    File.Copy(oldFilePath, newFilePath);
                    Debug.Log($"File '{oldFilePath}' copied to '{newFilePath}' successfully.");
                }
                else
                {
                    Debug.LogError($"Error: The file '{oldFilePath}' does not exist. Cannot copy.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while copying the file: {ex.Message}");
            }
        }

        // Gets an array of key names from a JSON file.
        public static string[] GetKeys(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{filePath}' does not exist.");
                }

                string json = File.ReadAllText(filePath);
                var dataObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                return dataObj.Properties().Select(p => p.Name).ToArray();
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while getting keys from file: {ex.Message}");
                return new string[0];
            }
        }

        // Gets an array of file names (and their extensions) from a directory.
        public static string[] GetFiles(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
                }

                return Directory.GetFiles(directoryPath);
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while getting files from directory: {ex.Message}");
                return new string[0];
            }
        }

        // Gets an array of sub-directory names from a directory.
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
                }

                return Directory.GetDirectories(directoryPath);
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while getting directories from directory: {ex.Message}");
                return new string[0];
            }
        }

        // Gets the date and time a file was last updated in the UTC timezone.
        public static DateTime GetTimestamp(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file '{filePath}' does not exist.");
                }

                var fileInfo = new FileInfo(filePath);
                return fileInfo.LastWriteTimeUtc;
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while getting file timestamp: {ex.Message}");
                return DateTime.MinValue;
            }
        }
        #endregion

        #region Cache
        // Caches a locally-stored file to memory. This enables you to load multiple keys from the file without the overhead of random access or opening the file multiple times.
        // For more information see the Improving Performance guide.
        public static void CacheFile(string filePath)
        {
        }

        // Stores a file in the cache as a local file.This enables you to save multiple keys to a file and store it using a single write, eliminating the overhead of random access.
        // For more information see the Improving Performance guide.
        public static void StoreCachedFile(string filePath)
        {
        }
        #endregion

        #region Backup
        //TODO: Checar se funcionam corretamente.

        // Creates a backup of a file which can be restored using SaveSystem.RestoreBackup.
        // A backup is created by copying the file and giving it a.bak extension.
        // If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.
        // This method is useful if you’re making updates to your save file which you may want to roll back, or you want to have a way of recovering from data corruption due to hardware faults.
        public static void CreateBackup(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.Log("Error: The specified file does not exist.");
                    return;
                }

                string backupPath = Path.ChangeExtension(filePath, bkpExtension);

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Copy(filePath, backupPath);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error occurred while creating backup: {ex.Message}");
            }
        }

        // Restores a backup of a file created using SaveSystem.CreateBackup.
        // A backup is created by copying the file and giving it a .bak extension.
        // If a backup already exists it will be overwritten, so you will need to ensure that the old backup will not be required before calling this method.
        public static bool RestoreBackup(string filePath)
        {
            try
            {
                string backupPath = Path.ChangeExtension(filePath, bkpExtension);

                if (File.Exists(backupPath))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    File.Copy(backupPath, filePath);
                    Debug.Log($"Backup '{backupPath}' successfully restored to '{filePath}'.");
                    return true;
                }
                else
                {
                    Debug.Log($"Backup file '{backupPath}' does not exist. No changes were made.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred while restoring backup: {ex.Message}");
                return false;
            }
        }

        // Destroy all backup files.
        public static bool ClearBackup(string filePath)
        {
            try
            {
                string backupPath = Path.ChangeExtension(filePath, bkpExtension);

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    Debug.Log("Backup files cleared successfully.");
                }
                else
                {
                    Debug.Log("No backup files found to clear.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"Error occurred while clearing backup files: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Utils
        public static string GetFullPath(string fileName)
        {
            return Path.Combine(dataPath, fileName);
        }

        public static List<string> GetFilesWithExtension(string directoryPath = null, string extension = null)
        {
            if (directoryPath == null)
            {
                directoryPath = dataPath;
            }

            if (extension == null)
            {
                extension = jsonExtension;
            }

            try
            {
                if (string.IsNullOrEmpty(extension) || !extension.StartsWith("."))
                {
                    Debug.LogError("Invalid extension. Ensure it starts with a '.' character.");
                    return new List<string>();
                }

                if (!Directory.Exists(directoryPath))
                {
                    Debug.LogError($"The directory '{directoryPath}' does not exist.");
                    return new List<string>();
                }

                return Directory.GetFiles(directoryPath, $"*{extension}")
                                .Select(Path.GetFileName)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while retrieving files: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion
    }
}