using System.Collections;
using UnityEngine;

public class YeetBoothController : MonoBehaviour
{
    [SerializeField] private Animator confettiAnimator;
    [SerializeField] private AudioClip confettiSound;
    [SerializeField] private AudioClip celebrationAudio;
    [SerializeField] private PlaySound button1;
    [SerializeField] private PlaySound button2;
    [SerializeField] private Sprite[] sprites; // Array to hold the 32 sprites
    [SerializeField] private AudioClip[] soundEffects; // Array to hold sound effects
    [SerializeField] private AudioClip[] clickSfx; // Array to hold sound effects
    [SerializeField] private float blinkingResetTime = 5f; // Time to reset blinking
    [SerializeField] private float blinkingRate = 0.5f; // Time between blinks

    private AudioSource audioSource;
    private int currentSpriteIndex = 0;
    private int nextSpriteIndex = 0; // Added to track the next sprite
    private bool isBlinking = false;
    private SpriteRenderer spriteRenderer; // Added to manage sprite rendering

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Initialize sprite renderer
        // Randomly assign sound effects to buttons
        AssignRandomSoundEffects();
    }

    void Update()
    {
         #if UNITY_EDITOR // TODO: ENABLE FOR LIVE
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnTransactionConfirmed();
        }
         #endif
        // Handle blinking logic here if needed
    }

    public void OnButtonPressed()
    {
        if (isBlinking) return; // Prevent action if already blinking

        StartCoroutine(BlinkingEffect());
        // Request transaction logic here
    }

    private IEnumerator BlinkingEffect()
    {
        isBlinking = true;
        currentSpriteIndex = Mathf.Min(currentSpriteIndex, sprites.Length - 1);
        nextSpriteIndex = Mathf.Min(currentSpriteIndex + 1, sprites.Length - 1);

        float resetTimer = blinkingResetTime; // Timer for resetting blinking
        while (isBlinking)
        {
            // Show current sprite
            spriteRenderer.sprite = sprites[currentSpriteIndex];
            audioSource.PlayOneShot(clickSfx[0]);
            yield return new WaitForSeconds(blinkingRate); // Show for 1 second

            // Show next sprite
            spriteRenderer.sprite = sprites[nextSpriteIndex];
            audioSource.PlayOneShot(clickSfx[1]);
            yield return new WaitForSeconds(blinkingRate); // Show for 1 second

            // Decrease reset timer
            resetTimer -= blinkingRate * 2;
            if (resetTimer <= 0)
            {
                // Reset to current sprite
                isBlinking = false;
                spriteRenderer.sprite = sprites[currentSpriteIndex];
            }
        }
    }

    private void OnTransactionConfirmed()
    {
        if (!isBlinking) return; // Do nothing if not blinking

        // Logic for successful transaction
        isBlinking = false; // Stop blinking
        PlayCelebrationAudio();
        ShowConfetti();
        // Set the latest sprite
        currentSpriteIndex = nextSpriteIndex; // Update to the next sprite
        spriteRenderer.sprite = sprites[currentSpriteIndex]; // Update sprite renderer
    }

    private void OnTransactionFailed()
    {
        // Logic for failed transaction
        isBlinking = false; // Stop blinking
        spriteRenderer.sprite = sprites[currentSpriteIndex]; // Reset to current sprite
    }


    private void PlayCelebrationAudio()
    {
        audioSource.PlayOneShot(celebrationAudio); // Play celebration audio
    }

    private void AssignRandomSoundEffects()
    {
        button1.SetSFX(soundEffects[Random.Range(0, soundEffects.Length)]);
        // Get a different random sound effect for button2
        AudioClip button1Sfx = button1.Sfx;
        AudioClip button2Sfx;
        do
        {
            button2Sfx = soundEffects[Random.Range(0, soundEffects.Length)];
        } while (button2Sfx == button1Sfx);

        button2.SetSFX(soundEffects[Random.Range(0, soundEffects.Length)]);
    }

    private void ShowConfetti()
    {
        confettiAnimator.SetTrigger("pop");
        audioSource.PlayOneShot(confettiSound);
    }
}
