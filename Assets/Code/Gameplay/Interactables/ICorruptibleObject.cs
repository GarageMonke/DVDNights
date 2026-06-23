namespace DVDNights
{
    public interface ICorruptibleObject
    {
        public void Corrupt();
        public void ClearCorruption();

        public bool CanBeCorrupted();
    }
}