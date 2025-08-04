using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    
    [Header("Settings")]
    public float minDelay = 0.1f; // Minimum delay before playing animation
    public float maxDelay = 2f; // Maximum delay before playing animation
    public bool useRandomStart = true; // If true, Animation Starts on Random Delay.
    
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object.");
            return;
        }

        if (useRandomStart)
        {
            StartCoroutine(PlayFlickerAnimationAfterRandomTime());
        }
        else
        {
            animator.Play("ANIM_Light_Flickering");  
        }
    }

    private IEnumerator PlayFlickerAnimationAfterRandomTime()
    {
        float randomDelay = Random.Range(minDelay, maxDelay); 
        yield return new WaitForSeconds(randomDelay);         
        animator.Play("ANIM_Light_Flickering");             
    }
}
