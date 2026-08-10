using System.Collections.Generic;
using CheatCodes.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class CheatCodesController : MonoBehaviour, ICheatCodesController
    {
        private Dictionary<string, ICheatCodeImplementation> _cheatCodes;
        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _cheatCodes =  new Dictionary<string, ICheatCodeImplementation>();
            ServiceLocator.RegisterService<ICheatCodesController>(this);
        }

        public void RegisterCodeByImplementation(ICheatCodeImplementation cheatCodeImplementation)
        {
            _cheatCodes.TryAdd(cheatCodeImplementation.GetCheatName(), cheatCodeImplementation);   
        }

        public void ActivateCodeByName(string codeName)
        {
            if (!_cheatCodes.ContainsKey(codeName))
            {
                return;
            }
            
            _cheatCodes[codeName].ActivateCheat();
        }
    }

    public interface ICheatCodesController
    {
        public void RegisterCodeByImplementation(ICheatCodeImplementation cheatCodeImplementation);
        public void ActivateCodeByName(string codeName);
    }
}