using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamegaard.RuntimeDebug
{
    public static class DefaultCommands
    {

    }

    public static class SceneCommands
    {
        [Command]
        private static void SetActiveScene([SceneNameSuggestion] string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                CommandConsole.Instance.SendTextMessage($"Set {sceneName} as active scene.");
            }
            else
            {
                CommandConsole.Instance.SendTextMessage($"Scene not found: {sceneName}");
            }
        }

        [Command]
        private static void LoadScene([SceneNameSuggestion] string sceneName, [Suggestions(LoadSceneMode.Single, LoadSceneMode.Additive)] string loadSceneMode)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        [Command]
        private static void UnloadScene([SceneNameSuggestion] string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        [Command]
        private static void LoadScene(int sceneID)
        {
            SceneManager.LoadScene(sceneID);
        }

        [Command]
        private static void UnloadScene(int sceneID)
        {
            SceneManager.UnloadSceneAsync(sceneID);
        }

        [Command]
        private static void LoadedScenes()
        {
            string message = "Loaded Scenes:";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                message += $"\n - {scene.name} (Index: {scene.buildIndex}, Active: {scene == SceneManager.GetActiveScene()})";
            }
            CommandConsole.Instance.SendTextMessage(message);
        }

        [Command]
        private static void Restart()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(buildIndex);
        }
    }

    public static class ScreenCommands
    {
        [Command]
        private static void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
        }

        [Command]
        private static void SetFullscreen([Suggestions(true, false)] bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }
    }

    public static class GraphicsCommands
    {
        [Command]
        private static void SetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }

        [Command]
        private static void SetVsync([Suggestions(true, false)] bool isEnabled)
        {
            QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        }

        [Command]
        private static void GetMSAA()
        {
            string msaa = QualitySettings.antiAliasing.ToString();
            CommandConsole.Instance.SendTextMessage(msaa);
        }

        [Command]
        private static void SetQualityLevel(int level)
        {
            QualitySettings.SetQualityLevel(level);
        }
    }

    public static class UtilCommands
    {
        [Command]
        private static void SetActive(string objectName, [Suggestions(true, false)] bool isEnabled)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                obj.SetActive(isEnabled);
            }
        }

        [Command]
        private static void SetParent(string target, string parentName)
        {
            GameObject child = GameObject.Find(target);
            GameObject parent = GameObject.Find(parentName);
            if (child != null)
            {
                child.transform.SetParent(parent != null ? parent.transform : null);
            }
        }

        [Command]
        private static void SetRotation(string target, Vector3 eulerAngles)
        {
            GameObject obj = GameObject.Find(target);
            if (obj != null)
            {
                obj.transform.eulerAngles = eulerAngles;
            }
        }

        [Command]
        private static void SetPosition(string target, Vector3 position)
        {
            GameObject obj = GameObject.Find(target);
            if (obj != null)
            {
                obj.transform.position = position;
            }
        }

        [Command]
        private static void SetScale(string target, Vector3 scale)
        {
            GameObject obj = GameObject.Find(target);
            if (obj != null)
            {
                obj.transform.localScale = scale;
            }
        }

        [Command]
        private static void Destroy(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }

        [Command]
        private static void DestroyComponent(string objectName, string componentName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                var component = obj.GetComponent(componentName);
                if (component != null)
                {
                    Object.Destroy(component);
                }
            }
        }

        [Command]
        private static void GameObjectInfo(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            string text;
            if (obj != null)
            {
                text = $"GameObject: {obj.name}";
                text += $"\n Active: {obj.activeInHierarchy}";

                Component[] components = obj.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    text += $"\n - Component: \" {comp.GetType().Name}";
                }
            }
            else
            {
                text = "GameObject not found";
            }
            CommandConsole.Instance.SendTextMessage(text);
        }
    }

    public static class TypeCommands
    {
        //enum‑info Gets all of the numeric values and value names for the specified enum type
    }

    public static class TimeCommands
    {
        [Command]
        private static void SetTimescale(float timescale)
        {
            Time.timeScale = timescale;
        }

        [Command]
        private static void GetTimeScale()
        {
            float currentScale = Time.timeScale;
            CommandConsole.Instance.SendTextMessage($"Current TimeScale: {currentScale}");
        }
    }

    public static class KeyBinderModule
    {
        //bind Binds a given command to a given key, so that every time the key is pressed, the command is invoked
        //unbind  Removes every binding for the given key
        //unbind‑all Unbinds every existing key binding
        //display‑bindings Displays all existing bindings on the key binder
    }

    public static class FileCommands
    {
        [Command]
        public static void WriteFile(string path, string data)
        {
            try
            {
                File.WriteAllText(path, data);
                CommandConsole.Instance.SendTextMessage($"Data written successfully to {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to write to {path}: {e.Message}");
            }
        }

        [Command]
        public static void ReadFile(string path)
        {
            try
            {
                string data = File.ReadAllText(path);
                CommandConsole.Instance.SendTextMessage(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to read from {path}: {e.Message}");
            }
        }

        [Command]
        public static void ExistsFile(string path)
        {
            if (File.Exists(path))
            {
                CommandConsole.Instance.SendTextMessage($"File exists: {path}");
            }
            else
            {
                Debug.LogError($"File does not exist: {path}");
            }
        }
    }

    public static class ApplicationCommands
    {
        [Command]
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        [Command]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            CommandConsole.Instance.SendTextMessage("All PlayerPrefs have been cleared.");
        }

        [Command]
        public static void GetVersion()
        {
            string version = Application.version;
            CommandConsole.Instance.SendTextMessage("Current application version: " + version);
        }
    }

    public static class HttpCommands
    {
        //http.get Sends a GET request to the specified URL
        //http.delete Sends a DELETE request to the specified URL
        //http.post Sends a POST request to the specified URL.A body may be sent with the request, with a default mediaType of text/plain
        //http.put Sends a PUT request to the specified URL.A body may be sent with the request, with a default mediaType of text/plain
    }
}