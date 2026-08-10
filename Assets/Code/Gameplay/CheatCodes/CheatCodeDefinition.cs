using UnityEngine;

namespace CheatCodes
{
    public abstract class CheatCodeDefinition : ScriptableObject
    {
        [SerializeField] private string codeName = "New Cheat Code";
        [SerializeField] private CheatInputData[] sequence;
        
        [SerializeField] private float maxIntervalBetweenInputs = 1.5f;
 
        public string CodeName => codeName;
        public CheatInputData[] Sequence => sequence;
        public float MaxIntervalBetweenInputs => maxIntervalBetweenInputs;
 
        
        public abstract void Execute();
    }
}