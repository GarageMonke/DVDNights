using System;
using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class InteractableController : MonoBehaviour, IInteractableController
    {
        private Dictionary<string, IInteractableObject> _interactableObjects;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _interactableObjects = new Dictionary<string, IInteractableObject>();
            ServiceLocator.RegisterService<IInteractableController>(this);
        }

        public void RegisterInteractable(IInteractableObject interactableObject)
        {
            _interactableObjects.TryAdd(interactableObject.InteractableId, interactableObject);
        }

        public void EnableAllInteractables()
        {
            foreach (IInteractableObject interactableObject in _interactableObjects.Values)
            {
                interactableObject.EnableInteraction();
            }
        }

        public void DisableAllInteractables()
        {
            foreach (IInteractableObject interactableObject in _interactableObjects.Values)
            {
                interactableObject.DisableInteraction();
            }
        }

        public void EnableInteractable(string interactableId)
        {
            _interactableObjects.TryGetValue(interactableId, out IInteractableObject interactableObject);
            interactableObject?.EnableInteraction();
        }

        public void DisableInteractable(string interactableId)
        {
            _interactableObjects.TryGetValue(interactableId, out IInteractableObject interactableObject);
            interactableObject?.DisableInteraction();
        }
    }

    public interface IInteractableController
    {
        public void RegisterInteractable(IInteractableObject interactableObject);
        public void EnableAllInteractables();
        public void DisableAllInteractables();
        public void EnableInteractable(string interactableId);
        public void DisableInteractable(string interactableId);
    }
}