using System;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ScriptableObjectParser : ITypeParser<ScriptableObject>
    {
        public ScriptableObject Parse(string value)
        {
            Type type = Type.GetType(value);
            if (type != null && typeof(ScriptableObject).IsAssignableFrom(type))
            {
#if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{type.Name} {value}");
                foreach (string guid in guids)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableObject scriptableObject = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                    if (scriptableObject != null && scriptableObject.name.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return scriptableObject;
                    }
                }
#endif
                return Resources.Load(value, type) as ScriptableObject;
            }
            return null;
        }
    }
}