namespace DVDNights
{
    public abstract  class CorruptibleInteractableObject : InteractableObject, ICorruptibleObject
    {
        protected bool _isCorrupted; 
        
        public virtual void Corrupt()
        {
            _isCorrupted = true;
        }

        public virtual void ClearCorruption()
        {
            _isCorrupted = false;
        }

        public abstract bool CanBeCorrupted();
    }
}