using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ToolsCharacterController : MonoBehaviour
{
    //[SerializeField] private float maxDistance = 1.5f;
    //[SerializeField] private float disappearDistance = 3f;
    [SerializeField] private int cooldown;

    //Vector3Int selectedTilePosition;
    private Animator animator;
    private ToolbarController toolbarController;
    private ItemToolbarPanel itemToolbarPanel;
    //private int selectable;
    private bool isOnCooldown;
    public bool canUse = true;

    private void Awake()
    {
        toolbarController = GetComponent<ToolbarController>();
        animator = GetComponent<Animator>();
        itemToolbarPanel = FindAnyObjectByType<ItemToolbarPanel>();
    }

    void Update()
    {
        CanSelectCheck();

        if (Input.GetKeyDown(KeyCode.E) && PlayerController.canInteract)
        {
            UseToolWorld();
        }
    }

    private void CanSelectCheck()
    {
        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;

        //if (Vector2.Distance(characterPosition, cameraPosition) < maxDistance)
        //{
        //    selectable = 0;
        //}
        //else if (Vector2.Distance(characterPosition, cameraPosition) > maxDistance && Vector2.Distance(characterPosition, cameraPosition) < disappearDistance)
        //{
        //    selectable = 1;
        //}
        //else
        //{
        //    selectable = 2;
        //}
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

    public void UseToolWorld()
    {
        if (!canUse || isOnCooldown) return;

        Item item = toolbarController.SelectedItem;
        if (item == null || item.OnAction == null) return;

        Vector2 position = transform.position;
        GameObject selectedButton = itemToolbarPanel.SelectedToolButton();
        selectedButton.transform.DOComplete();
        selectedButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f);
        selectedButton.transform.DOPunchPosition(Vector3.one * 5f, 0.2f);

        bool complete = item.OnAction.OnApply(position);

        if (complete && item.OnItemUsed != null)
        {
            item.OnItemUsed.OnItemUsed(item, LocalGameManager.Instance.inventoryContainer);
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}