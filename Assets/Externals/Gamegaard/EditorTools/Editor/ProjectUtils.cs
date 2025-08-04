using UnityEditor;
using UnityEngine;

public static class ProjectUtils
{
    public static string googleDriveLink;
    public static string googlePlayLink;
    public static string appleLink;
    public static string steamPageLink;
    public static string androidPassword;

    [MenuItem("Tools/ProjectUtils/Open Google Drive", false, 0)]
    public static void OpenGoogleDrive()
    {
        Application.OpenURL(googleDriveLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Google Drive", true)]
    public static bool ValidateOpenGoogleDrive()
    {
        return !string.IsNullOrEmpty(googleDriveLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Google Play Console", false, 0)]
    public static void OpenGooglePlayConsole()
    {
        Application.OpenURL(googlePlayLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Google Play Console", true)]
    public static bool ValidateOpenGooglePlayConsole()
    {
        return !string.IsNullOrEmpty(googlePlayLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Apple Developer", false, 0)]
    public static void OpenAppleDeveloper()
    {
        Application.OpenURL(appleLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Apple Developer", true)]
    public static bool ValidateOpenAppleDeveloper()
    {
        return !string.IsNullOrEmpty(appleLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Steam Page", false, 0)]
    public static void OpenSteamPage()
    {
        Application.OpenURL(steamPageLink);
    }

    [MenuItem("Tools/ProjectUtils/Open Steam Page", true)]
    public static bool ValidateOpenSteamPage()
    {
        return !string.IsNullOrEmpty(steamPageLink);
    }

    [MenuItem("Tools/ProjectUtils/Android Password", false, 0)]
    public static void LogPassword()
    {
        Debug.Log(androidPassword);
    }

    [MenuItem("Tools/ProjectUtils/Android Password", true)]
    public static bool ValidateLogPassword()
    {
        return !string.IsNullOrEmpty(androidPassword);
    }
}
