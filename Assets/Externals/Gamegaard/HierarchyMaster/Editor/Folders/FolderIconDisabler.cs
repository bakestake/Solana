using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

namespace Gamegaard.HierarchyMaster.Attributes.Editor
{
    [InitializeOnLoad]
    public static class HierarchyIconManager
    {
        static HierarchyIconManager()
        {
            EditorApplication.delayCall += ApplyIconSettings;
        }

        private static void ApplyIconSettings()
        {
            Type annotationType = Type.GetType("UnityEditor.AnnotationUtility,UnityEditor");
            if (annotationType == null) return;

            MethodInfo getAnnotations = annotationType.GetMethod("GetAnnotations", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo setIconEnabled = annotationType.GetMethod("SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Static);

            if (getAnnotations == null || setIconEnabled == null) return;

            object annotations = getAnnotations.Invoke(null, null);
            if (annotations == null) return;

            var markedClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Select(type => new
                {
                    TypeName = type.Name,
                    Attribute = type.GetCustomAttribute<HierarchyIconAttribute>()
                })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => x.TypeName, x => x.Attribute.IsEnabled);

            foreach (object annotation in (Array)annotations)
            {
                Type annotationClass = annotation.GetType();
                FieldInfo classIdField = annotationClass.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                FieldInfo scriptClassField = annotationClass.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                FieldInfo iconEnabledField = annotationClass.GetField("iconEnabled", BindingFlags.Public | BindingFlags.Instance);

                if (classIdField == null || scriptClassField == null || iconEnabledField == null) continue;

                int classId = (int)classIdField.GetValue(annotation);
                string scriptClass = (string)scriptClassField.GetValue(annotation);
                int iconEnabled = (int)iconEnabledField.GetValue(annotation);

                if (markedClasses.TryGetValue(scriptClass, out bool shouldEnable))
                {
                    int newState = shouldEnable ? 1 : 0;
                    if (iconEnabled != newState)
                    {
                        setIconEnabled.Invoke(null, new object[] { classId, scriptClass, newState });
                    }
                }
            }
        }
    }
}
