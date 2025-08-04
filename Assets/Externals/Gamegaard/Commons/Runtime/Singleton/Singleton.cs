using System;

namespace Gamegaard.Singleton
{
    /// <summary>
    /// Provides a generic base class for implementing a singleton pattern.
    /// This class ensures that only one instance of type T exists within the application.
    /// The instance is created lazily when first accessed.
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;

        public static bool HasInstance { get; private set; }

        public static T Instance
        {
            get
            {
                if (!HasInstance)
                {
                    _instance = CreateInstance();
                }
                return _instance;
            }
        }

        private Singleton()
        {
            if (HasInstance)
            {
                throw new Exception("Attempting to create a duplicate instance of a singleton class.");
            }
            else
            {
                HasInstance = true;
            }
        }

        /// <summary>
        /// Creates an instance of type T using reflection.
        /// This method is called when an instance is needed and none exists.
        /// </summary>
        /// <returns>A new instance of type T.</returns>
        protected static T CreateInstance()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
    }
}