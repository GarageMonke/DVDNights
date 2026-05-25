using UnityEngine;

namespace DVDNights
{
    public abstract class InteractableObject : MonoBehaviour, IInteractableObject
    {
        [Header("Configuration")] 
        [SerializeField] private Vector3 cameraPosition;
        [SerializeField] private Vector3 cameraRotation;
        [SerializeField] private bool overrideCamera;

        public Vector3 CameraPosition =>  cameraPosition;
        public Vector3 CameraRotation =>  cameraRotation;

        public bool OverrideCamera => overrideCamera;


        public abstract void Interact();

        public abstract void StopInteraction();

        public void Highlight()
        {
           
        }

        public void Unhighlight()
        {
            
        }
    }

    public interface IInteractableObject
    {
        public Vector3 CameraPosition { get; }
        public Vector3 CameraRotation { get; }
        public bool OverrideCamera { get; }
        public void Interact();
        public void StopInteraction();
        public void Highlight();
        public void Unhighlight();
    }
}