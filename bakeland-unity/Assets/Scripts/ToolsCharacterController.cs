using System.Collections;
using System.Collections.Generic;
using System.Security;
using DG.Tweening;
using UnityEngine;

public class ToolsCharacterController : MonoBehaviour
{
    Animator animator;
    ToolbarController toolbarController;
    [SerializeField] float maxDistance = 1.5f;
    [SerializeField] float disappearDistance = 3f;

    Vector3Int selectedTilePosition;
    int selectable;
    bool isOnCooldown;
    public bool canUse = true;
    [SerializeField] int cooldown;

    private void Awake()
    {
        toolbarController = GetComponent<ToolbarController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CanSelectCheck();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canUse)
            {
                if (!isOnCooldown)
                {
                    UseToolWorld();
                }
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }


    private void CanSelectCheck()
    {
        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;

        if (Vector2.Distance(characterPosition, cameraPosition) < maxDistance)
        {
            selectable = 0;
        }
        else if (Vector2.Distance(characterPosition, cameraPosition) > maxDistance && Vector2.Distance(characterPosition, cameraPosition) < disappearDistance)
        {
            selectable = 1;
        }
        else
        {
            selectable = 2;
        }
    }

    private void CalculateActDir()
    {
        animator.SetInteger("actHorizontal", 0);
        animator.SetInteger("actVertical", 0);

        Vector2 characterPosition = transform.position;
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = clickPosition - characterPosition;

        animator.SetTrigger("act");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                animator.SetInteger("actHorizontal", 1);
            }
            else
            {
                animator.SetInteger("actHorizontal", -1);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                animator.SetInteger("actVertical", 1);
            }
            else
            {
                animator.SetInteger("actVertical", -1);
            }
        }
    }

    private bool UseToolWorld()
    {
        Vector2 position = transform.position;

        Item item = toolbarController.GetItem;
        if (item == null) { return false; }
        if (item.onAction == null) { return false; }

        GameObject selectedButton = GameObject.FindAnyObjectByType<ItemToolbarPanel>().SelectedToolButton();
        selectedButton.transform.DOComplete();
        selectedButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f);
        selectedButton.transform.DOPunchPosition(Vector3.one * 5f, 0.2f);

        bool complete = item.onAction.OnApply(position);

        if (complete)
        {
            if (item.onItemUsed != null)
            {
                item.onItemUsed.OnItemUsed(item, LocalGameManager.Instance.inventoryContainer);
            }
        }

        return complete;
    }
}
