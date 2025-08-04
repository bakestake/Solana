using Gamegaard;
using UnityEngine;

namespace Bakeland
{
    public class ConditionalParticle : SingleComponentBehaviour<ParticleSystem>
    {
        [SerializeField] private bool isAutoPlayEnabled;

        private ParticleSystem.EmissionModule emission;
        private PersistentSettings persistentSettings;

        protected override void Awake()
        {
            base.Awake();
            emission = targetComponent.emission;
        }

        private void OnEnable()
        {
            persistentSettings = PersistentSettings.NotNullInstance;
            persistentSettings.vfxState.OnValueChanged += SetEmissionState;
        }

        private void OnDisable()
        {
            persistentSettings = PersistentSettings.NotNullInstance;
            persistentSettings.vfxState.OnValueChanged -= SetEmissionState;
        }

        private void Start()
        {
            SetEmissionState(persistentSettings.vfxState.Value);
        }

        public void SetAutoPlayState(bool isEnabled)
        {
            isAutoPlayEnabled = isEnabled;
        }

        private void SetEmissionState(bool isEnabled)
        {
            if (isEnabled)
            {
                emission.enabled = true;
                if (isAutoPlayEnabled)
                {
                    targetComponent.Play();
                }
            }
            else
            {
                emission.enabled = false;
                targetComponent.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}