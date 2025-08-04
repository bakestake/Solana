using UnityEngine;

namespace Gamegaard.Singleton
{
    /// <summary>
    /// Defines a base class for creating a ScriptableObject singleton pattern in Unity. This class ensures that only one instance of type T exists within the application.
    /// Instances must be placed in the default Resources folder to ensure they can be loaded properly.
    /// </summary>
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        protected static T _instance;

        /// <summary>
        /// Indicates whether an instance of this Singleton already exists within the application.
        /// This property is used to check if the Singleton instance needs to be created or if it already exists, helping manage singleton integrity throughout the application lifecycle.
        /// </summary>
        public static bool HasInstance { get; private set; }

        /// <summary>
        /// Provides a globally accessible instance of type T. Checks for an existing instance loaded from resources.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAsset();
                    HasInstance = _instance != null;
                }
                return _instance;
            }
        }

        private static T FindAsset()
        {
            T[] assets = Resources.LoadAll<T>("");

            if (assets != null && assets.Length > 0)
            {
                return assets[0];
            }

            Debug.LogWarning($"No instance of {typeof(T)} found in Resources.");
            return null;
        }
    }
}