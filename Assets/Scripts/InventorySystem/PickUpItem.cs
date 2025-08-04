using Bakeland;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class PickUpItem : MonoBehaviour
{
    [Header("UID")]
    [ContextMenuItem(nameof(GenerateUID), nameof(GenerateUID))]
    [SerializeField] private string uniqueID = Guid.NewGuid().ToString();
    [SerializeField] private bool ignoreSave;

    [Header("Give Currency")]
    [SerializeField] private int giveCurrency;

    [Header("Give Item")]
    [SerializeField] private Item item;
    [SerializeField] private int count = 1;

    [Header("References")]
    [SerializeField] private Interact interact;
    [SerializeField] private Collider2D col2d;
    [SerializeField] private UnityEvent OnPickUpEvent;

    [Header("Auto Pickup")]
    [SerializeField] private bool autoPickup;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float pickUpDistance = 1.5f;
    [SerializeField] private float destroyDistance = 0.2f;

    private Transform player;

    public string UniqueID => uniqueID;
    public bool IgnoreSave => ignoreSave;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            GenerateUID();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) UpdateVisual();
        };
#endif
    }

    private void UpdateVisual()
    {
        if (item != null)
        {
            SpriteRenderer itemSprite = GetComponent<SpriteRenderer>();
            itemSprite.sprite = item.Icon;
            name = $"Item [{item.ItemName}]";
        }
    }

    private void Start()
    {
        player = LocalGameManager.Instance.PlayerController.transform;

        SpriteRenderer itemSprite = GetComponent<SpriteRenderer>();
        itemSprite.sprite = item.Icon;

        if (interact != null)
        {
            interact.enabled = !autoPickup;
        }
        col2d.enabled = !autoPickup;
    }

    private void Update()
    {
        if (autoPickup && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > pickUpDistance) return;

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

    private void GenerateUID()
    {
        uniqueID = Guid.NewGuid().ToString();
    }

    private void OnPickUp()
    {
        if (item != null)
        {
            if (LocalGameManager.Instance.inventoryContainer != null)
            {
                PickUpItemFunctionName.mintFunctionName = item.MintFunction;
                LocalGameManager.Instance.inventoryContainer.Add(item, count);
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

        PickupsManager.ItemCollected(this);

        transform.DOKill();
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.popSounds);

        OnPickUpEvent?.Invoke();
        Destroy(gameObject);
    }
}
