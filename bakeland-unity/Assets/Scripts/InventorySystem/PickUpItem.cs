using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BakelandWalletInteraction;

public class PickUpItem : MonoBehaviour
{
    [Header("Auto Pickup")]
    public bool autoPickup;
    [Header("Give Currency")]
    public int giveCurrency;
    [Header("Give Item")]
    public Item item;
    public int count = 1;

    [Header("References")]
    [SerializeField] Interact interact;
    [SerializeField] Collider2D col2d;

    Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] float pickUpDistance = 1.5f;
    [SerializeField] float destroyDistance = 0.2f;
    private UaserWalletInteractions userWalletInteractions;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject userWalletObject = GameObject.FindGameObjectWithTag("UserWallet");
        if (userWalletObject != null)
        {
            userWalletInteractions = userWalletObject.GetComponent<UaserWalletInteractions>();
        }
        Sprite itemSprite = GetComponent<SpriteRenderer>().sprite;
        itemSprite = item.icon;

        if (autoPickup)
        {
            interact.enabled = false;
            col2d.enabled = false;
        }
        else
        {
            interact.enabled = true;
            col2d.enabled = true;
        }
        // transform.DOLocalMoveY(transform.position.y + 0.25f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
    // Update is called once per frame
    void Update()
    {
        if (autoPickup)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > pickUpDistance)
            {
                return;
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            if (distance < destroyDistance)
            {
                OnPickUp();
            }
        }
    }

    public void OnPickUp()
    {
        if (item != null)
        {
            if (LocalGameManager.Instance.inventoryContainer != null)
            {
                LocalGameManager.Instance.inventoryContainer.Add(item, count);
                // Getting the mint function
                PickUpItemFunctionName.mintFunctionName = item.mintFunction;
            }
            else
            {
                Debug.LogWarning("No inventory container attached to the GameManager!");
            }
        }

        if (giveCurrency > 0)
        {
            PlayerCurrency.instance.AddGold(giveCurrency);
        }

        transform.DOKill();
        SoundManager.instance.PlayRandomFromList(SoundManager.instance.popSounds);
        Destroy(gameObject);
    }
}
