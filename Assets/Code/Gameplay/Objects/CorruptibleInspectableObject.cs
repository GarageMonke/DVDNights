using UnityEngine;

namespace Rulebound
{
    public abstract class CorruptibleInspectableObject : CorruptibleInteractableObject
    {
        [Header("Inspectable-References")]
        [SerializeField] private InspectableObject inspectableObject;
        
        public override string GetInteractionAction()
        {
            if (!_isCorrupted)
            {
                return inspectableObject.GetInteractionAction();
            }
            
            return GetCorruptedAction();
        }

        public override void Interact()
        {
            if (!_isCorrupted)
            {
                inspectableObject.Inspect();
                return;
            }

            InteractWithCorruption();
        }

        public override void StopInteraction()
        {
            if (!_isCorrupted)
            {
                inspectableObject.StopInspection();
            }
        }

        public override bool CanBeCorrupted()
        {
            return !_isCorrupted;
        }

        protected abstract void InteractWithCorruption();
        protected abstract string GetCorruptedAction();
    }
}