using Gamegaard;
using Gamegaard.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace Bakeland
{
    public sealed class PersistentSettings : MonoBehaviourSingleton<PersistentSettings>
    {
        [SerializeField] private AudioMixer audioMixer;

        private const string SettingsName = "Settings";
        private const string MasterVolume = "MasterVolume";

        public readonly JsonValue<float> volume = new JsonValue<float>("Volume", 1, SettingsName);
        public readonly JsonValue<bool> vfxState = new JsonValue<bool>("isVFXActive", true, SettingsName);

        protected override void Awake()
        {
            base.Awake();
            volume?.Load(1);
            vfxState?.Load(true);
            if (audioMixer != null) audioMixer.SetFloat(MasterVolume, volume.Value);
        }
    }
}
