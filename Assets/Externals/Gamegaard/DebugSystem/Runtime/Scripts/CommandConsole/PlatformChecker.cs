using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public static class PlatformChecker
    {
        private static Dictionary<Platforms, bool> platformCheckMap;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            platformCheckMap = new Dictionary<Platforms, bool>
            {
                { Platforms.Android, Application.platform == RuntimePlatform.Android },
                { Platforms.IPhonePlayer, Application.platform == RuntimePlatform.IPhonePlayer },
                { Platforms.LinuxEditor, Application.platform == RuntimePlatform.LinuxEditor },
                { Platforms.LinuxPlayer, Application.platform == RuntimePlatform.LinuxPlayer },
                { Platforms.MetroPlayerARM, Application.platform == RuntimePlatform.WSAPlayerARM },
                { Platforms.MetroPlayerX64, Application.platform == RuntimePlatform.WSAPlayerX64 },
                { Platforms.MetroPlayerX86, Application.platform == RuntimePlatform.WSAPlayerX86 },
                { Platforms.OSXEditor, Application.platform == RuntimePlatform.OSXEditor },
                { Platforms.OSXPlayer, Application.platform == RuntimePlatform.OSXPlayer },
                { Platforms.PS4, Application.platform == RuntimePlatform.PS4 },
                { Platforms.Switch, Application.platform == RuntimePlatform.Switch },
                { Platforms.tvOS, Application.platform == RuntimePlatform.tvOS },
                { Platforms.WebGLPlayer, Application.platform == RuntimePlatform.WebGLPlayer },
                { Platforms.WindowsEditor, Application.platform == RuntimePlatform.WindowsEditor },
                { Platforms.WindowsPlayer, Application.platform == RuntimePlatform.WindowsPlayer },
                { Platforms.WSAPlayerARM, Application.platform == RuntimePlatform.WSAPlayerARM },
                { Platforms.WSAPlayerX64, Application.platform == RuntimePlatform.WSAPlayerX64 },
                { Platforms.WSAPlayerX86, Application.platform == RuntimePlatform.WSAPlayerX86 },
                { Platforms.XboxOne, Application.platform == RuntimePlatform.XboxOne },
            #if !UNITY_5_3_OR_NEWER
                { Platforms.PSM, Application.platform == RuntimePlatform.PSM },
                { Platforms.WP8Player, Application.platform == RuntimePlatform.WP8Player },
            #endif
            #if !UNITY_5_4_OR_NEWER
                { Platforms.FlashPlayer, Application.platform == RuntimePlatform.FlashPlayer },
                { Platforms.OSXWebPlayer, Application.platform == RuntimePlatform.OSXWebPlayer },
                { Platforms.WindowsWebPlayer, Application.platform == RuntimePlatform.WindowsWebPlayer },
                { Platforms.BlackBerryPlayer, Application.platform == RuntimePlatform.BlackBerryPlayer },
            #endif
            #if !UNITY_2017_3_OR_NEWER
                { Platforms.TizenPlayer, Application.platform == RuntimePlatform.TizenPlayer },
                { Platforms.SamsungTVPlayer, Application.platform == RuntimePlatform.SamsungTVPlayer },
            #endif
            #if !UNITY_2018_1_OR_NEWER
                { Platforms.WiiU, Application.platform == RuntimePlatform.WiiU },
            #endif
            #if !UNITY_2018_4_OR_NEWER
                { Platforms.PSP2, Application.platform == RuntimePlatform.PSP2 },
            #endif
            #if !UNITY_2020_1_OR_NEWER
                { Platforms.XBOX360, Application.platform == RuntimePlatform.XBOX360 },
                { Platforms.PS3, Application.platform == RuntimePlatform.PS3 },
            #endif
            };
        }

        public static bool CheckPlatform(this Platforms platforms)
        {
            if (platforms == Platforms.All) return true;
            if (platforms == Platforms.None) return false;

            return platformCheckMap.ContainsKey(platforms) && platformCheckMap[platforms];
        }
    }
}