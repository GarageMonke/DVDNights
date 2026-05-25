using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class ArtPreviewWindow : MonoBehaviour, IArtPreviewWindow
    {
        [SerializeField] private Image artPreviewImage;
        [SerializeField] private TextMeshProUGUI artistNameText;
        
        [Header("Image Bounds")]
        [SerializeField] private Vector2 maxSize = new(1000f, 1000f);
        
        public void UpdateWindow(ArtDataSO artData)
        {
            if (!artData)
            {
                return;
            }
            
            artistNameText.text = "ARTIST: " + artData.ArtistName;
            FitImageToSprite(artData.ArtSprite);
        }

        public void Display()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void FitImageToSprite(Sprite sprite)
        {
            if (sprite == null) return;

            float spriteWidth  = sprite.rect.width;
            float spriteHeight = sprite.rect.height;
            
            float scale = Mathf.Min(
                maxSize.x / spriteWidth,
                maxSize.y / spriteHeight,
                1f                      
            );
            
            artPreviewImage.sprite = sprite;
            artPreviewImage.rectTransform.sizeDelta = new Vector2(spriteWidth * scale, spriteHeight * scale);
        }
    }

    public interface IArtPreviewWindow : IWindow
    {
        public void UpdateWindow(ArtDataSO artData);
    }
}