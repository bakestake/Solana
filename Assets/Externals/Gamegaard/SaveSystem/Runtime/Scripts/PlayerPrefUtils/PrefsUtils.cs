using System;
using UnityEngine;

namespace Gamegaard.SavingSystem
{
    public static class PrefsUtils
    {
        public static void Save<T>(string key, T data)
        {
            Type type = typeof(T);
            object dataAsObject = data;

            if (type == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)dataAsObject);
            }
            else if (type == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)dataAsObject);
            }
            else if (type == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)dataAsObject);
            }
            else if (type == typeof(bool))
            {
                int valueToSave = ((bool)dataAsObject) ? 1 : 0;
                PlayerPrefs.SetInt(key, valueToSave);
            }
            else if (type.IsEnum)
            {
                PlayerPrefs.SetString(key, (string)dataAsObject);
            }
            else
            {
                Debug.LogWarning("Type not supported for saving in PlayerPrefs: " + type.ToString());
                return;
            }

            PlayerPrefs.Save();
        }

        public static T Load<T>(string key, T defaultValue = default)
        {
            Type type = typeof(T);

            if (type == typeof(int))
            {
                return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            }
            else if (type == typeof(float))
            {
                return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            }
            else if (type == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(key, defaultValue.ToString());
            }
            else if (type == typeof(bool))
            {
                int loadedValue = PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue));
                return (T)(object)(loadedValue == 1);
            }
            else if (type.IsEnum)
            {
                string enumValue = PlayerPrefs.GetString(key, defaultValue.ToString());
                if (Enum.TryParse(typeof(T), enumValue, out object parsedEnum))
                {
                    return (T)parsedEnum;
                }
                else
                {
                    Debug.LogWarning("Failed to parse enum from PlayerPrefs.");
                    return default;
                }
            }
            else
            {
                Debug.LogWarning("Type not supported for loading from PlayerPrefs: " + type.ToString());
                return default;
            }
        }
    }
}
