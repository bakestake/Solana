using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.FarmSystem
{
    public class SoilInteractor : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Animator interactionAnimation;
        [SerializeField] private AnimatorOverrideController sowingAnimation;
        [SerializeField] private AnimatorOverrideController harvestingAnimation;
        [SerializeField] private AnimatorOverrideController fertilizingAnimation;
        [SerializeField] private AnimatorOverrideController wateringAnimation;

        [SerializeField] private UnityEvent OnSowing;
        [SerializeField] private UnityEvent OnHarvesting;
        [SerializeField] private UnityEvent OnFertilizing;
        [SerializeField] private UnityEvent OnWatering;

        private ToolbarController toolbarController;
        private Soil soil;
        private SoilModulesHandler modulesHandler;

        private void Reset()
        {
            player = FindObjectOfType<PlayerController>();
        }

        private void Awake()
        {
            toolbarController = player.GetComponent<ToolbarController>();
            soil = GetComponent<Soil>();
            modulesHandler = GetComponent<SoilModulesHandler>();
        }

        public void OnInteract()
        {
            if (!soil.HasCondition("Fertilized"))
            {
                Item item = toolbarController.SelectedItem;
                if (toolbarController.HasSelectedItem && item.IsFertilizer)
                {
                    foreach (ISoilModule module in modulesHandler.Modules)
                    {
                        if (module is IFertilizable fertilizable)
                        {
                            OnFertilizing?.Invoke();
                            PlayInteractionAnimation(fertilizingAnimation);
                            fertilizable.SetFertilized(item.FertilizerData);
                            LocalGameManager.Instance.inventoryContainer.Remove(item);
                            return;
                        }
                    }
                }
            }
            if (soil.HasCondition("Dry"))
            {
                Item item = toolbarController.SelectedItem;
                if (toolbarController.HasSelectedItem && item.IsWaterCan)
                {
                    foreach (ISoilModule module in modulesHandler.Modules)
                    {
                        if (module is IWaterable waterable)
                        {
                            OnWatering?.Invoke();
                            PlayInteractionAnimation(wateringAnimation);
                            waterable.Water(30);
                            return;
                        }
                    }
                }
            }

            if (soil.HasPlant)
            {
                if (soil.Plant.TryHarvest(player))
                {
                    PlayInteractionAnimation(harvestingAnimation);
                    OnHarvesting?.Invoke();
                }
            }
            else
            {
                Item item = toolbarController.SelectedItem;
                if (toolbarController.HasSelectedItem && item.IsPlantable)
                {
                    soil.PlantOnSoil(item.PlantData);
                    OnSowing?.Invoke();
                    PlayInteractionAnimation(sowingAnimation);
                    LocalGameManager.Instance.inventoryContainer.Remove(item);
                }
            }
        }

        private void PlayInteractionAnimation(AnimatorOverrideController overrideController)
        {
            interactionAnimation.runtimeAnimatorController = overrideController;
            interactionAnimation.gameObject.SetActive(true);
            interactionAnimation.SetTrigger("OnPlay");
        }
    }
}