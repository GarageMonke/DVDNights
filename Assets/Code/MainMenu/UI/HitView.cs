using System.Collections;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class HitView : MonoBehaviour, IHitView
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI hitText;
        [SerializeField] private RectTransform hitRectTransform;
        [SerializeField] private CanvasGroup hitCanvasGroup;
        
        [Header("Configuration")]
        [SerializeField] private Color cornerColor;
        
        public void InitializeView(string hitMessage, bool isCorner, DiskDataSO diskData)
        {
            hitText.text = hitMessage;

            Color diskColor = diskData.DiskColor;
            
            hitText.color = isCorner ? cornerColor : diskColor;
            
            StartCoroutine(FloatAndFade());
        }

        public RectTransform GetRectTransform()
        {
            return hitRectTransform;
        }

        private IEnumerator FloatAndFade()
        {
            float duration = 1f;
            float elapsed = 0f;
            Vector3 startPos = transform.localPosition;
            float tickInterval = 1f / GameFeel.TvFrameRate;

            while (elapsed < duration)
            {
                if (GameFeel.IgnoreTvFrameRate)
                {
                    elapsed += Time.deltaTime;
                }
                else
                {
                    elapsed += tickInterval;
                }
         
                float t = Mathf.Clamp01(elapsed / duration);

                transform.localPosition = startPos + Vector3.up * (50f * t);
                hitCanvasGroup.alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);

                if (GameFeel.IgnoreTvFrameRate)
                    yield return null;
                else
                    yield return new WaitForSeconds(tickInterval);
            }

            Destroy(gameObject);
        }
    }

    public interface IHitView
    {
        public void InitializeView(string hitMessage, bool isCorner, DiskDataSO diskData);
        public RectTransform GetRectTransform();
    }
}