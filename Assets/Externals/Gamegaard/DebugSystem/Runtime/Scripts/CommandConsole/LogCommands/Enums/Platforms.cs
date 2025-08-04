using System;

namespace Gamegaard.RuntimeDebug
{
    [Flags]
    public enum Platforms : long
    {
        None = 0,
        All = ~0L,
        Android = 1 << 0,
        BlackBerryPlayer = 1 << 1,
        BuildPlatforms = 1 << 2,
        EditorPlatforms = 1 << 3,
        FlashPlayer = 1 << 4,
        IPhonePlayer = 1 << 5,
        LinuxEditor = 1 << 6,
        LinuxPlayer = 1 << 7,
        Lumin = 1 << 8,
        MetroPlayerARM = 1 << 9,
        MetroPlayerX64 = 1 << 10,
        MetroPlayerX86 = 1 << 11,
        MobilePlatforms = 1 << 12,
        NaCl = 1 << 13,
        OSXDashboardPlayer = 1 << 14,
        OSXEditor = 1 << 15,
        OSXPlayer = 1 << 16,
        OSXWebPlayer = 1 << 17,
        PS3 = 1 << 18,
        PS4 = 1 << 19,
        PSM = 1 << 20,
        PSP2 = 1 << 21,
        SamsungTVPlayer = 1 << 22,
        Switch = 1 << 23,
        TizenPlayer = 1 << 24,
        tvOS = 1 << 25,
        WebGLPlayer = 1 << 26,
        WiiU = 1 << 27,
        WindowsEditor = 1 << 28,
        WindowsPlayer = 1 << 29,
        WindowsWebPlayer = 1 << 30,
        WP8Player = 1L << 31,
        WSAPlayerARM = 1L << 32,
        WSAPlayerX64 = 1L << 33,
        WSAPlayerX86 = 1L << 34,
        XBOX360 = 1L << 35,
        XboxOne = 1L << 36,
    }
}