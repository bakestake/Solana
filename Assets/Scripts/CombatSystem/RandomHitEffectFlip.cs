using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHitEffectFlip : MonoBehaviour
{
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (Random.value > 0.5f)
        {
            spriteRenderer.flipX = true;
        }
    }
}

