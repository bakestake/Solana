namespace Gamegaard
{
    /// <summary>
    /// Represents a contract for objects that can be paused and resumed.
    /// </summary>
    public interface IPauseable
    {
        /// <summary>
        /// Gets a value indicating whether the object is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Pauses the associated object or process.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the associated object or process.
        /// </summary>
        void Resume();
    }
}