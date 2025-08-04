using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Gamegaard.Utils;

[RequireComponent(typeof(ScrollRect))]
public class InfiniteScrollView : MonoBehaviour
{
    private List<RectTransform> itemsList = new List<RectTransform>();
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private Vector2 oldVelocity;
    private float spacingWidth;
    private bool shouldRestoreVelocity;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        viewport = scrollRect.viewport;
        content = scrollRect.content;
        layoutGroup = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
        content.gameObject.GetComponentsInDirectChildren(itemsList);

        int itemsToAdd = GetItemsToAdd();
        CalculateSpacingAndRepopulate(itemsToAdd);
        SetInitialPosition(itemsToAdd);
    }

    private void Update()
    {
        if (shouldRestoreVelocity)
        {
            RestoreScrollVelocity();
        }

        CheckAndAdjustPosition();
    }

    private int GetItemsToAdd()
    {
        float size = itemsList[0].rect.width + layoutGroup.spacing;
        spacingWidth = itemsList.Count * size;
        return Mathf.CeilToInt(viewport.rect.width / size);
    }

    private void CalculateSpacingAndRepopulate(int itemsToAdd)
    {
        if (itemsList.Count == 0) return;

        PopulateItems(itemsToAdd);
        ReversePopulateItems(itemsToAdd);
    }

    private void SetInitialPosition(int itemsToAdd)
    {
        content.localPosition = new Vector3(0 - (itemsList[0].rect.width + layoutGroup.spacing) * itemsToAdd, content.localPosition.y, content.localPosition.z);
    }

    private void PopulateItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(itemsList[i % itemsList.Count], content);
        }
    }

    private void ReversePopulateItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index = itemsList.Count - i - 1;
            RectTransform rect = Instantiate(itemsList[index % itemsList.Count], content);
            rect.SetAsFirstSibling();
        }
    }

    private void RestoreScrollVelocity()
    {
        scrollRect.velocity = oldVelocity;
        shouldRestoreVelocity = false;
    }

    private void CheckAndAdjustPosition()
    {
        if (content.localPosition.x > 0)
        {
            UpdatePosition();
            content.localPosition -= new Vector3(spacingWidth, 0, 0);
        }
        else if (content.localPosition.x < -spacingWidth)
        {
            UpdatePosition();
            content.localPosition += new Vector3(spacingWidth, 0, 0);
        }
    }

    private void UpdatePosition()
    {
        Canvas.ForceUpdateCanvases();
        oldVelocity = scrollRect.velocity;
        shouldRestoreVelocity = true;
    }
}
