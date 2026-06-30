using UnityEngine;

namespace DVDNights
{
    public class ArtInteractableObject : CorruptibleInspectableObject
    {
        protected override void InteractWithCorruption()
        {
            if (!_isCorrupted)
            {
                return;
            }
            
            ClearCorruption();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            
        }

        protected override string GetCorruptedAction()
        {
            return "Something is wrong...";
        }
    }
}