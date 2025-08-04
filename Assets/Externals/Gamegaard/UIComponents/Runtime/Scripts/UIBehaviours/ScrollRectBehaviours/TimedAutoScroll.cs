using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TimedAutoScroll : MonoBehaviour, IDragHandler
{
    [SerializeField] private bool isVerticalEnabled;
    [SerializeField] private bool isHorizontalEnabled;
    [SerializeField] private bool pauseOnInteract;
    [SerializeField] private float timeToScroll = 2.0f;
    [SerializeField] private float autoScrollRestartDelay = 2.0f;
    [SerializeField] private float interactionRestartDelay = 5.0f;

    private bool isMovingUp;
    private bool isMovingLeft;
    private ScrollRect scrollView;
    private Coroutine verticalAutoScrollCoroutine;
    private Coroutine horizontalAutoScrollCoroutine;
    private Coroutine interactionCoroutine;
    private WaitForSeconds autoScrollWaitForSeconds;
    private WaitForSeconds interactionWaitForSeconds;

    private void Awake()
    {
        scrollView = GetComponent<ScrollRect>();
        autoScrollWaitForSeconds = new WaitForSeconds(autoScrollRestartDelay);
        interactionWaitForSeconds = new WaitForSeconds(interactionRestartDelay);
    }

    private void OnEnable()
    {
        StartAutoScroll();
    }

    private void OnDisable()
    {
        StopAutoScroll();
        TryStopCoroutine(interactionCoroutine);
    }

    private void StartAutoScroll()
    {
        TryStopCoroutine(verticalAutoScrollCoroutine);
        TryStopCoroutine(horizontalAutoScrollCoroutine);
        verticalAutoScrollCoroutine = StartCoroutine(AutoScrollVertical());
        horizontalAutoScrollCoroutine = StartCoroutine(AutoScrollHorizontal());
    }

    private void StopAutoScroll()
    {
        TryStopCoroutine(verticalAutoScrollCoroutine);
        TryStopCoroutine(horizontalAutoScrollCoroutine);
    }

    private void TryStopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    public void OnUserInteraction()
    {
        if (pauseOnInteract)
        {
            TryStopCoroutine(interactionCoroutine);
            interactionCoroutine = StartCoroutine(HandleInteraction());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnUserInteraction();
    }

    private IEnumerator AutoScrollHorizontal()
    {
        while (true)
        {
            if (!isHorizontalEnabled)
            {
                yield return null;
                continue;
            }

            if (scrollView.horizontalNormalizedPosition <= 0 && !isMovingUp)
            {
                isMovingLeft = true;
                yield return autoScrollWaitForSeconds;
            }
            else if (scrollView.horizontalNormalizedPosition >= 1 && isMovingUp)
            {
                isMovingLeft = false;
                yield return autoScrollWaitForSeconds;
            }

            float direction = isMovingLeft ? Time.deltaTime / timeToScroll : Time.deltaTime / timeToScroll * -1;
            scrollView.horizontalNormalizedPosition += direction;

            yield return null;
        }
    }

    private IEnumerator AutoScrollVertical()
    {
        while (true)
        {
            if (!isVerticalEnabled)
            {
                yield return null;
                continue;
            }

            if (scrollView.verticalNormalizedPosition <= 0 && !isMovingUp)
            {
                isMovingUp = true;
                yield return autoScrollWaitForSeconds;
            }
            else if (scrollView.verticalNormalizedPosition >= 1 && isMovingUp)
            {
                isMovingUp = false;
                yield return autoScrollWaitForSeconds;
            }

            float direction = isMovingUp ? Time.deltaTime / timeToScroll : Time.deltaTime / timeToScroll * -1;
            scrollView.verticalNormalizedPosition += direction;

            yield return null;
        }
    }

    private IEnumerator HandleInteraction()
    {
        StopAutoScroll();
        yield return interactionWaitForSeconds;
        StartAutoScroll();
    }
}