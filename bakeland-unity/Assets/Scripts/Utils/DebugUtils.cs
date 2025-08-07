#if DEBUG || DEVELOPMENT_BUILD || DEBUG_MODE
public static class DebugUtils
{
    public static void Log<T>(T message)
    {
        UnityEngine.Debug.Log(message?.ToString());
    }

    public static void LogWarning<T>(T message)
    {
        UnityEngine.Debug.LogWarning(message?.ToString());
    }

    public static void LogError<T>(T message)
    {
        UnityEngine.Debug.LogError(message?.ToString());
    }
}
#endif
