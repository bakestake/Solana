using Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Bakeland.Editor
{
    public static class ZPositionFixer
    {
        [MenuItem("Tools/Gamegaard/Bakeland/Fix Z Position")]
        private static void FixZPositions()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
            int fixedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (!obj.scene.IsValid() || !obj.activeInHierarchy || obj.TryGetComponent(out Camera _) || obj.TryGetComponent(out ICinemachineCamera _))
                    continue;

                Transform transform = obj.transform;
                Vector3 position = transform.position;

                if (!Mathf.Approximately(position.z, 0f))
                {
                    position.z = 0f;
                    transform.position = position;
                    EditorSceneManager.MarkSceneDirty(obj.scene);
                    fixedCount++;
                }
            }

            Debug.Log($"Fix Z Position: {fixedCount} object(s) adjusted.");
        }
    }
}
