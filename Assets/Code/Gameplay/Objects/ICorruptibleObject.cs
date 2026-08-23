using System;

namespace Rulebound
{
    public interface ICorruptibleObject
    {
        public Action<string> OnCooldownFinished { get; set; }
        public string ObjectId { get; }
        public void Corrupt();
        public void ClearCorruption();
        public bool CanBeCorrupted();
        public void CooldownObject();

        public bool IsCorrupted();
    }
}