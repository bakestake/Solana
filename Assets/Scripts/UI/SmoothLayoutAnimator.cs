using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SmoothLayoutAnimator : MonoBehaviour
{
    public float animationDuration = 0.3f;

    public void RemoveElement(GameObject element)
    {
        StartCoroutine(AnimateRemoval(element));
    }

    private IEnumerator AnimateRemoval(GameObject element)
    {
        // Step 1: Capture current positions
        Dictionary<RectTransform, Vector3> initialPositions = new Dictionary<RectTransform, Vector3>();
        foreach (RectTransform child in transform)
        {
            initialPositions[child] = child.anchoredPosition;
        }

        // Step 2: Remove the target element
        Destroy(element);

        // Step 3: Wait for end of frame to allow layout to update
        yield return new WaitForEndOfFrame();

        // Step 4: Capture new positions
        Dictionary<RectTransform, Vector3> finalPositions = new Dictionary<RectTransform, Vector3>();
        foreach (RectTransform child in transform)
        {
            finalPositions[child] = child.anchoredPosition;
            // Reset to initial position
            if (initialPositions.ContainsKey(child))
            {
                child.anchoredPosition = initialPositions[child];
            }
        }

        // Step 5: Animate to new positions
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            foreach (RectTransform child in transform)
            {
                if (initialPositions.ContainsKey(child))
                {
                    Vector3 startPos = initialPositions[child];
                    Vector3 endPos = finalPositions[child];
                    child.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
                }
            }

            yield return null;
        }

        // Ensure final positions are set
        foreach (RectTransform child in transform)
        {
            if (finalPositions.ContainsKey(child))
            {
                child.anchoredPosition = finalPositions[child];
            }
        }
    }
}
