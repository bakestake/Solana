using System;
using UnityEngine;

namespace Gamegaard.Singleton
{
    /// <summary>
    /// Defines a base class for creating a MonoBehaviour singleton pattern in Unity. This class ensures that only one instance of type T exists within the application.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        /// <summary>
        /// Check if the singleton instance will be not destroyed btween scene transitions (DontDestroyOnLoad)
        /// </summary>
        [Tooltip("If true, this instance will persist between scene transitions (DontDestroyOnLoad).")]
        [SerializeField] private bool _isPersistent;
        private static T _instance;

        /// <summary>
        /// Indicates whether an instance of this Singleton already exists within the application.
        /// This property is used to check if the Singleton instance needs to be created or if it already exists, helping manage singleton integrity throughout the application lifecycle.
        /// </summary>
        public static bool HasInstance => _instance != null;

        /// <summary>
        /// Indicates whether the Awake() method of this Singleton has already been called by Unity.
        /// Returns <c>true</c> if already called; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsAwakened { get; private set; }

        /// <summary>
        /// Indicates whether the Start() method of this Singleton has already been called by Unity.
        /// Returns <c>true</c> if already called; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsStarted { get; private set; }

        /// <summary>
        /// Indicates whether the OnDestroy() method of this Singleton has already been called by Unity.
        /// Returns <c>true</c> if already called; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsDestroyed { get; private set; }

        /// <summary>
        /// Returns if the game is quitting;
        /// </summary>
        public static bool IsQuitting { get; private set; }

        /// <summary>
        /// Provides a globally accessible instance of type T. Can return null if not initialized.
        /// </summary>
        public static T Instance => _instance;

        /// <summary>
        /// Provides a globally accessible instance of type T. Checks for an existing instance of T in the scene.
        /// If no instance is found, a new GameObject is created and the T component is attached to it.
        /// </summary>
        public static T NotNullInstance
        {
            get
            {
                if (IsQuitting)
                {
                    Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting. Use IsQuitting for check before use NotNullInstance on denitialization methods.");
                    return _instance;
                }
                lock (_lock)
                {
                    if (!HasInstance)
                    {
                        SetInstance(FindOrCreateInstance());
                    }
                    return _instance;
                }
            }
        }

        private static readonly object _lock = new object();

        public static Action<T> OnInstanceCreated;

        protected virtual void Awake()
        {
            if (IsQuitting) return;

            if (!HasInstance)
            {
                SetInstance((T)this);
            }
            else if (_instance != this)
            {
                Destroy(this);
                return;
            }
        }

        protected virtual void Start()
        {
            if (IsStarted || _instance != this) return;
            IsStarted = true;
            if (_isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (IsDestroyed || _instance != this) return;
            IsDestroyed = true;

#if UNITY_EDITOR
            if (IsQuitting)
            {
                ClearStaticData();
            }
#endif
        }

        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }

#if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void ClearStaticData()
        {
            _instance = null;
            IsAwakened = false;
            IsStarted = false;
            IsDestroyed = false;
            IsQuitting = false;
            OnInstanceCreated = null;
        }
#endif

        protected static void SetInstance(T instance)
        {
            OnInstanceCreated?.Invoke(instance);
            _instance = instance;
            IsAwakened = true;
            IsStarted = false;
            IsDestroyed = false;
        }

        private static T FindOrCreateInstance()
        {
#if UNITY_2023_3_OR_NEWER
            T instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
            T instance = FindObjectOfType<T>(true);
#endif
            Debug.LogWarning("Singleton instance uninitialized. Executing FindObjectOfType to locate the instance within the entire scene. For optimal performance, consider waiting for the singleton's initialization.");
            if (instance == null)
            {
                GameObject singletonObject = new GameObject($"{typeof(T).Name} [Auto Generated]");
                instance = singletonObject.AddComponent<T>();
                Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] No existing instance found. A new singleton instance has been created.");
            }
            return instance;
        }
    }
}