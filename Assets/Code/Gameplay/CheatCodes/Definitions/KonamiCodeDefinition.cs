using CorePatterns.ServiceLocator;
using Rulebound;
using UnityEngine;

namespace CheatCodes.Definitions
{
    [CreateAssetMenu(fileName = "CheatCode-KonamiCode", menuName = "ScriptableObjects/CheatCodes/KonamiCode", order = 0)]
    public class KonamiCodeDefinition : CheatCodeDefinition
    {
        public override void Execute()
        {
            ICheatCodesController cheatCodesController = ServiceLocator.GetService<ICheatCodesController>();
            cheatCodesController.ActivateCodeByName(CodeName);
        }
    }
}