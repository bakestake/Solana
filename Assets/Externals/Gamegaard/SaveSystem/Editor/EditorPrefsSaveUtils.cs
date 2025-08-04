using System;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.SavingSystem
{
    namespace Gamegaard.SaveSystem
    {
        public static class EditorPrefsSaveUtils
        {
            public static void SavePref<T>(string key, T data)
            {
                Type type = typeof(T);
                object dataAsObject = data;

                if (type == typeof(int))
                {
                    EditorPrefs.SetInt(key, (int)dataAsObject);
                }
                else if (type == typeof(float))
                {
                    EditorPrefs.SetFloat(key, (float)dataAsObject);
                }
                else if (type == typeof(string))
                {
                    EditorPrefs.SetString(key, (string)dataAsObject);
                }
                else if (type == typeof(bool))
                {
                    EditorPrefs.SetBool(key, (bool)dataAsObject);
                }
                else if (type.IsEnum)
                {
                    EditorPrefs.SetString(key, data.ToString());
                }
                else
                {
                    Debug.LogWarning("Type not supported for saving in EditorPrefs: " + type.ToString());
                    return;
                }
            }

            public static T LoadPref<T>(string key, T defaultValue = default)
            {
                Type type = typeof(T);

                if (type == typeof(int))
                {
                    return (T)(object)EditorPrefs.GetInt(key, (int)(object)defaultValue);
                }
                else if (type == typeof(float))
                {
                    return (T)(object)EditorPrefs.GetFloat(key, (float)(object)defaultValue);
                }
                else if (type == typeof(string))
                {
                    return (T)(object)EditorPrefs.GetString(key, defaultValue.ToString());
                }
                else if (type == typeof(bool))
                {
                    return (T)(object)EditorPrefs.GetBool(key, (bool)(object)defaultValue);
                }
                else if (type.IsEnum)
                {
                    string enumValue = EditorPrefs.GetString(key, defaultValue.ToString());
                    if (Enum.TryParse(typeof(T), enumValue, out object parsedEnum))
                    {
                        return (T)parsedEnum;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse enum from EditorPrefs.");
                        return default;
                    }
                }
                else
                {
                    Debug.LogWarning("Type not supported for loading from EditorPrefs: " + type.ToString());
                    return default;
                }
            }
        }
    }
}
