using Gamegaard.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Gamegaard
{
    /// <summary>
    /// Manages the pausing and freezing of the game.
    /// </summary>
    public class PauseSystem : MonoBehaviourSingleton<PauseSystem>
    {
        private readonly Dictionary<object, Scene> pauseCasters = new Dictionary<object, Scene>();
        private TimeController timeController;
        private FreezeController freezeController;

        /// <summary>
        /// Gets the number of objects currently pausing the game.
        /// </summary>
        private int PauseCount => pauseCasters.Count;

        /// <summary>
        /// Checks if the game is currently paused by the unity system when Time.timescale is equal 0.
        /// </summary>
        public bool IsPaused => timeController.IsPaused;

        /// <summary>
        /// Checks if the game is currently paused by the PauseSystem.
        /// </summary>
        public bool IsPausedBySystem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game is currently frozen.
        /// </summary>
        public bool IsFrozen => freezeController.IsFrozen;

        /// <summary>
        /// Event triggered when the game is paused.
        /// </summary>
        public event Action OnGamePaused;

        /// <summary>
        /// Event triggered when the game is resumed.
        /// </summary>
        public event Action OnGameResumed;

        /// <summary>
        /// Subscribes to the scene unloading event when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            timeController.OnGamePaused += HandleGamePaused;
            timeController.OnGameResumed += HandleGameResumed;
        }

        /// <summary>
        /// Unsubscribes from the scene unloading event when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            timeController.OnGamePaused -= HandleGamePaused;
            timeController.OnGameResumed -= HandleGameResumed;
        }

        protected override void Start()
        {
            base.Start();
            timeController = new TimeController();
            freezeController = new FreezeController(this, timeController);
        }

        private void Update()
        {
            timeController.CheckForTimeScaleChange();
        }

        /// <summary>
        /// Handles the game paused event.
        /// </summary>
        private void HandleGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        /// <summary>
        /// Handles the game resumed event.
        /// </summary>
        private void HandleGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        /// <summary>
        /// Called when a scene is unloaded. Unpauses the game if it is currently paused and removes pause casters from the unloaded scene.
        /// </summary>
        /// <param name="scene">The scene that was unloaded.</param>
        private void OnSceneUnloaded(Scene scene)
        {
            RemoveCastersFromScene(scene);
        }

        /// <summary>
        /// Removes casters from the dictionary if they were associated with the unloaded scene.
        /// </summary>
        /// <param name="scene">The scene that was unloaded.</param>
        private void RemoveCastersFromScene(Scene scene)
        {
            List<object> castersToRemove = new List<object>();

            foreach (var kvp in pauseCasters)
            {
                if (kvp.Value == scene)
                {
                    castersToRemove.Add(kvp.Key);
                }
            }

            foreach (var caster in castersToRemove)
            {
                UnpauseByCaster(caster);
                pauseCasters.Remove(caster);
            }
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void Pause()
        {
            if (IsPaused) return;
            freezeController.UnfreezeTime();
            timeController.Pause();
            IsPausedBySystem = true;
        }

        /// <summary>
        /// Unpauses the game.
        /// </summary>
        public void Unpause()
        {
            if (!IsPaused) return;
            timeController.Unpause();
            IsPausedBySystem = false;
        }

        /// <summary>
        /// Pauses the game, associated with a specific object.
        /// </summary>
        /// <param name="caster">The object pausing the game.</param>
        public void PauseByCaster(object caster)
        {
            if (pauseCasters.ContainsKey(caster)) return;
            pauseCasters.Add(caster, SceneManager.GetActiveScene());

            if (PauseCount == 1)
            {
                Pause();
            }
        }

        /// <summary>
        /// Unpauses the game associated with a specific object. The game fully unpauses only when there are no more objects pausing it.
        /// </summary>
        /// <param name="caster">The object unpausing the game.</param>
        public void UnpauseByCaster(object caster)
        {
            if (!pauseCasters.ContainsKey(caster)) return;
            pauseCasters.Remove(caster);

            if (PauseCount == 0)
            {
                Unpause();
            }
        }

        /// <summary>
        /// Forces the game to be unpaused, regardless of who paused it.
        /// </summary>
        public void ForceUnpause()
        {
            pauseCasters.Clear();
            Unpause();
        }

        /// <summary>
        /// Freezes the game for a specified duration.
        /// </summary>
        /// <param name="timeInSeconds">The duration to freeze the game in seconds.</param>
        public void FreezeTime(float timeInSeconds)
        {
            freezeController.FreezeTime(timeInSeconds);
        }

        /// <summary>
        /// Unfreezes the game, if it was previously frozen.
        /// </summary>
        public void UnfreezeTime()
        {
            freezeController.UnfreezeTime();
        }
    }
}