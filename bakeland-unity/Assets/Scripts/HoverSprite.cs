using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HoverSprite : MonoBehaviour
{
    public float hoverAmount;
    public float hoverSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Hover();
    }

    private void Hover()
    {
        transform.DOLocalMoveY(transform.localPosition.y + hoverAmount, hoverSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
