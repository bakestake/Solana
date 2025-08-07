using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarOfTheDeadQuestStep4 : QuestStep
{
    public GameObject succubusPrefab;
    public Transform succubusSpawnPoint;
    public GameObject amuletPrefab;
    private GameObject succubusInstance;
    private GameObject amuletInstance;
    private bool hasDialogueEnded = false;
    private bool hasPickedUpAmulet = false;

    private void Start()
    {
        UpdateState();
        SpawnSuccubus();
    }

    private void SpawnSuccubus()
    {
        if (succubusSpawnPoint != null && succubusPrefab != null)
        {
            succubusInstance = Instantiate(succubusPrefab, succubusSpawnPoint.position, Quaternion.identity);
            succubusInstance.name = "Succubus";
        }
        else
        {
            Debug.LogError("Missing succubus prefab or spawn point reference!");
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded += OnDialogueEnded;
        GameEventsManager.instance.miscEvents.onInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded -= OnDialogueEnded;
        GameEventsManager.instance.miscEvents.onInteracted -= OnInteracted;
    }

    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        if (!hasDialogueEnded &&
            trigger.gameObject.name == "Succubus" &&
            trigger.defaultDialogue.actors[0].Name.Contains("Succubus"))
        {
            hasDialogueEnded = true;
            StartCoroutine(TeleportSuccubusAndDropAmulet());
            UpdateState();
        }
    }

    private void OnInteracted(Interact interactable)
    {
        if (!hasPickedUpAmulet &&
            interactable.gameObject.name == "SuccubusAmulet")
        {
            hasPickedUpAmulet = true;
            // Optional: Destroy the amulet object if needed
            if (amuletInstance != null)
            {
                Destroy(amuletInstance);
            }
            FinishQuestStep();
        }
    }

    private IEnumerator TeleportSuccubusAndDropAmulet()
    {
        if (succubusInstance != null)
        {
            ParticleSpawner.instance.SpawnParticle(ParticleSpawner.instance.succubusDisappearParticle, succubusInstance.transform.position);
            // Get the sprite renderer
            SpriteRenderer spriteRenderer = succubusInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {

                SoundManager.instance.PlaySfx(SoundManager.instance.succubusDisappear);
                // Load the dissolve material from Resources
                Material dissolveMaterial = Resources.Load<Material>("Materials/SuccubusDissolve");
                if (dissolveMaterial != null)
                {
                    // Create instance of the material and assign it
                    Material dissolveInstance = new Material(dissolveMaterial);
                    spriteRenderer.material = dissolveInstance;

                    // Animate the dissolve effect over 1 second
                    float dissolveTime = 2f;
                    float startTime = Time.time;

                    while (Time.time - startTime < dissolveTime)
                    {
                        float normalizedTime = (Time.time - startTime) / dissolveTime;
                        dissolveInstance.SetFloat("_FadeAmount", normalizedTime);
                        yield return null;
                    }
                }
                else
                {
                    Debug.LogError("Could not load SuccubusDissolve material from Resources!");
                }
            }


            yield return new WaitForSeconds(0.1f); // Adjust timing as needed

            // Spawn amulet at succubus location before destroying her
            Vector3 amuletPosition = succubusInstance.transform.position;
            Destroy(succubusInstance);

            if (amuletPrefab != null)
            {
                amuletInstance = Instantiate(amuletPrefab, amuletPosition, Quaternion.identity);
                amuletInstance.name = "SuccubusAmulet";
            }
        }
    }

    private void UpdateState()
    {
        string state = "";
        string status = hasDialogueEnded ?
            "Pick up the amulet." :
            "Find and confront the Succubus.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // Not needed for this step
    }

    private void OnDestroy()
    {
        // Clean up if quest step is destroyed before completion
        if (succubusInstance != null)
        {
            Destroy(succubusInstance);
        }
        if (amuletInstance != null)
        {
            Destroy(amuletInstance);
        }
    }
}
