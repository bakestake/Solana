using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerPrefsOptions : Editor
{
    [MenuItem("Tools/Save/PlayerPref/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared!");
    }
}