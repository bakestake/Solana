public static class DebugUtils
{
    public static void Log<T>(T message)
    {
#if DEBUG || DEVELOPMENT_BUILD || DEBUG_MODE
        UnityEngine.Debug.Log(message?.ToString());
#endif
    }

    public static void LogWarning<T>(T message)
    {
#if DEBUG || DEVELOPMENT_BUILD || DEBUG_MODE
        UnityEngine.Debug.LogWarning(message?.ToString());
#endif
    }

    public static void LogError<T>(T message)
    {
#if DEBUG || DEVELOPMENT_BUILD || DEBUG_MODE
        UnityEngine.Debug.LogError(message?.ToString());
#endif
    }
}
