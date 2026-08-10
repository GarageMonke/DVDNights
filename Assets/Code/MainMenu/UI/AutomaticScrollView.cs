using System;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticScrollView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Scroll Settings")]
    [SerializeField] private float speed = 50f;

    public Action OnScrollEnded;

    private Coroutine _scrollRoutine;
    
    public void StartScrolling()
    {
        StopScrolling();
        _scrollRoutine = StartCoroutine(ScrollBySpeedRoutine());
    }
    
    public void StartScrolling(float duration, float holdTime = 1f)
    {
        StopScrolling();
        _scrollRoutine = StartCoroutine(ScrollByDurationRoutine(duration, holdTime));
    }

    public void StopScrolling()
    {
        if (_scrollRoutine != null)
        {
            StopCoroutine(_scrollRoutine);
            _scrollRoutine = null;
        }
    }

    private IEnumerator ScrollBySpeedRoutine()
    {
        scrollRect.verticalNormalizedPosition = 1f;

        RectTransform content = scrollRect.content;
        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollableHeight = Mathf.Max(contentHeight - viewportHeight, 0.01f);

        while (scrollRect.verticalNormalizedPosition > 0f)
        {
            float delta = (speed / scrollableHeight) * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - delta);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        _scrollRoutine = null;
        OnScrollEnded?.Invoke();
    }

    private IEnumerator ScrollByDurationRoutine(float duration, float holdTime = 1f)
    {
        scrollRect.verticalNormalizedPosition = 1f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;

        yield return new WaitForSeconds(holdTime);
        
        _scrollRoutine = null;
        OnScrollEnded?.Invoke();
    }
}
}