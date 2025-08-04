using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Compilation;
using UnityEngine;

public static class TypeFinder
{
    public static List<Type> GetDerivedTypesWithValidConstructors(Type baseType)
    {
        List<Type> validTypes = new List<Type>();
        var customAssemblies = CompilationPipeline.GetAssemblies();

        foreach (var customAssembly in customAssemblies)
        {
            try
            {
                var loadedAssembly = System.Reflection.Assembly.Load(customAssembly.name);
                var types = loadedAssembly.GetTypes()
                    .Where(type => IsValidType(type, baseType));

                validTypes.AddRange(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                Debug.LogWarning($"Failed to load types from assembly {customAssembly.name}: {ex.Message}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Debug.LogError(loaderException.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error loading assembly {customAssembly.name}: {ex.Message}");
            }
        }

        return validTypes;
    }

    public static bool IsValidType(Type type, Type baseType)
    {
        return (type.IsClass || type.IsValueType) &&
               !type.IsAbstract &&
               !type.IsGenericType &&
               !type.ContainsGenericParameters &&
               type.IsPublic &&
               !type.Name.StartsWith("<") &&
               !IsEditorClass(type) &&
               (type.IsSubclassOf(baseType) || baseType.IsAssignableFrom(type)) &&
               (HasSerializableAttribute(type) || IsUnityObjectType(type)) &&
               (IsUnityObjectType(type) || type.IsValueType || HasValidConstructor(type));
    }

    public static bool HasValidConstructor(Type type)
    {
        return type.IsValueType ||
               type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                   .Any(ctor => ctor.GetParameters().Length == 0);
    }

    public static bool HasSerializableAttribute(Type type)
    {
        return type.IsSerializable || Attribute.IsDefined(type, typeof(SerializableAttribute));
    }

    public static bool IsUnityObjectType(Type type)
    {
        return typeof(UnityEngine.Object).IsAssignableFrom(type);
    }

    public static bool IsEditorClass(Type type)
    {
        return type.Namespace != null && (type.Namespace.StartsWith("UnityEditor") || type.Namespace.Contains(".Editor"));
    }
}
