using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DeliveryController : MonoBehaviour, IDeliveryController
    {
        [Header("DVD-Boxes")]
        [SerializeField] private DVDBoxInteractableObject[] dvdBoxes;
        
        [Header("Rulesets")]
        [SerializeField] private GameRulesInteractableObject[] decoyGameRulesInteractableObjects;
        
        [Header("Door")]
        [SerializeField] private DoorInteractableObject doorInteractableObject;
        [SerializeField] private Light doorLight;
        
        private int _currentDeliveryIndex;
        private IGameProgressionController _gameProgressionController;
        private IDVDTrayController _dvdTrayController;
        private ICameraController _cameraController;
        private Sequence _deliverySequence;

        public int LastDeliveredIndex => _currentDeliveryIndex;

        private void Awake()
        {
            InstallService();
        }

        private void Start()
        {
            _dvdTrayController = ServiceLocator.GetService<IDVDTrayController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDeliveryController>(this);
        }

        private void LoadDeliveredObjects()
        {
            //Load Delivery Index
            _currentDeliveryIndex = 2;
            
            if (_currentDeliveryIndex <= 0)
            {
                return;
            }
            
            decoyGameRulesInteractableObjects[_currentDeliveryIndex].ShowRules();
            decoyGameRulesInteractableObjects[_currentDeliveryIndex].TeleportRulesToDesktop();

            for (int i = 0; i <= _currentDeliveryIndex; i++)
            {
                dvdBoxes[i].TeleportDvdBoxToDesktop();
            }
            
            _dvdTrayController.SetCurrentDVDBox(dvdBoxes[_currentDeliveryIndex]);
        }

        public void DeliverNextDvdBox()
        {
            if (_currentDeliveryIndex >= dvdBoxes.Length)
            {
                return;
            }
            
            doorInteractableObject.KnockDoor();
            
            doorLight.enabled = true;
            dvdBoxes[_currentDeliveryIndex].gameObject.SetActive(true);
            dvdBoxes[_currentDeliveryIndex].EnableInteraction();
            dvdBoxes[_currentDeliveryIndex].OnInteractionPerformed += OnDvdBoxDelivered;
            
            Sequence doorSequence = DOTween.Sequence();
            doorSequence.AppendInterval(1f);
            doorSequence.AppendCallback(() =>
            {
                _cameraController.TweenToRotation(Quaternion.Euler(new Vector3(0f, 180f, 0f)), 0.35f);
            });

            _deliverySequence = DOTween.Sequence().SetLoops(-1)
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    doorInteractableObject.KnockDoor();
                })
                .AppendInterval(3f);

        }

        public void DeliverNextRuleSet()
        {
            GameRulesInteractableObject gameRulesInteractableObject = decoyGameRulesInteractableObjects[_currentDeliveryIndex];
            gameRulesInteractableObject.SlipTroughDoor();
            _gameProgressionController = ServiceLocator.GetService<IGameProgressionController>();
            _gameProgressionController.SetLastDeliveredRules(gameRulesInteractableObject);
            
            _currentDeliveryIndex++;
        }

        private void OnDvdBoxDelivered()
        {
            dvdBoxes[_currentDeliveryIndex].OnInteractionPerformed -= OnDvdBoxDelivered;
            doorLight.enabled = false;
            _deliverySequence?.Kill();
        }
    }

    public interface IDeliveryController
    {
        public int LastDeliveredIndex { get; }
        public void DeliverNextDvdBox();
        public void DeliverNextRuleSet();
        
    }
}