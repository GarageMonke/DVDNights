using UnityEngine;

namespace CheatCodes.Definitions
{
    [CreateAssetMenu(fileName = "CheatCode-KonamiCode", menuName = "ScriptableObjects/CheatCodes/KonamiCode", order = 0)]
    public class KonamiCode : CheatCodeDefinition
    {
        public override void Execute()
        {
            Debug.Log($"[CheatCode] '{CodeName}' activated");
        }
    }
}