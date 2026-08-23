using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class InspectableObject : MonoBehaviour, IInspectableObject
    {
        [Header("Configuration")]
        [SerializeField] private InspectableDataSO inspectableDataSO;
        
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClip inspectionAudioClip;
        
        private IInspectionController _inspectionController;
        
        private void Start()
        {
            _inspectionController = ServiceLocator.GetService<IInspectionController>();
        }

        public string GetInteractionAction()
        {
            return "Inspect";
        }

        public void Inspect()
        {
           _inspectionController.Inspect(inspectableDataSO);
           AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, inspectionAudioClip, volume: 1f, pitch: 2.5f);
        }

        public void StopInspection()
        {
            _inspectionController.StopInspection();
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, inspectionAudioClip, volume: 1f, pitch: 1.5f);
        }
    }

    public interface IInspectableObject
    {
        public void Inspect();
        public void StopInspection();
    }
}