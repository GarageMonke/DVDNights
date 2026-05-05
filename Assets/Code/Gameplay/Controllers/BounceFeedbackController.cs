using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class BounceFeedbackController : MonoBehaviour, IBounceFeedbackController
    {
        [Header("Configuration")] 
        [SerializeField] private HitView hitViewPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform tvScreenRect;
        [SerializeField] private Transform bounceArea;
        
        private IDisksController _disksController;
        private IPointsController _pointsController;
        private Vector2 _areaHalfSize;
        private Bounds _areaBounds;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _areaBounds = bounceArea.GetComponent<MeshCollider>().bounds;
            _areaHalfSize = new Vector2(_areaBounds.extents.x, _areaBounds.extents.y);
            ServiceLocator.RegisterService<IBounceFeedbackController>(this);
        }

        private void Start()
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            _pointsController = ServiceLocator.GetService<IPointsController>();
        }

        public void ListenToBouncer(IBouncerDisk diskToListenTo)
        {
            diskToListenTo.OnHit += HandleHit;
        }

        public void RemoveBouncer(IBouncerDisk diskToRemove)
        {
            diskToRemove.OnHit -= HandleHit;
        }
        
        private void HandleHit(DiskDataSO diskData, Vector3 hitPosition, bool isCorner)
        {
            Vector2 localPoint = WorldToTvPanelLocal(hitPosition);

            IHitView hitView = Instantiate(hitViewPrefab, tvScreenRect);
            hitView.GetRectTransform().localPosition = localPoint;

            int amountEarned;
            
            if (isCorner)
            {
                amountEarned = _pointsController.GetCornerPoints(diskData);
                hitView.InitializeView("+" + amountEarned, true);
                return;
            }

            amountEarned = _pointsController.GetBorderPoints(diskData);
            hitView.InitializeView("+" + amountEarned, false);
        }

        private Vector2 WorldToTvPanelLocal(Vector3 normalizedPos)
        {
            float x = normalizedPos.x * (tvScreenRect.rect.width * 0.5f);
            float y = normalizedPos.y * (tvScreenRect.rect.height * 0.5f);

            return new Vector2(x, y);
        }
    
    }

    public interface IBounceFeedbackController
    {
        public void ListenToBouncer(IBouncerDisk diskToListenTo);
        public void RemoveBouncer(IBouncerDisk diskToRemove);
    }
}