using System;
using Rulebound;
using UnityEngine;

namespace CheatCodes.InputSource
{
    [Serializable]
    public class CheatInputMapping
    {
        [SerializeField] private InputActionSO inputActionSo;
        [SerializeField] private CheatInputData cheatInput;

        public InputActionSO InputActionSO => inputActionSo;
        public CheatInputData CheatInput => cheatInput;
    }
}