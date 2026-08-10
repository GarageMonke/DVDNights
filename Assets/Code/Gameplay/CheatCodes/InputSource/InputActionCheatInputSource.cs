using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CheatCodes.InputSource
{
    public class InputSystemCheatInputSource : MonoBehaviour
    {
        [Header("Cheat codes to listen for")] 
        [SerializeField] private List<CheatCodeDefinition> registeredCodes;
        [SerializeField] private List<CheatInputMapping> inputMappings;

        private CheatCodeHandler _handler;

   
        private readonly List<(InputAction action, Action<InputAction.CallbackContext> callback)> _bindings = new();

        private void Awake()
        {
            _handler = new CheatCodeHandler(registeredCodes, () => Time.time);
        }

        private void OnEnable()
        {
            foreach (CheatInputMapping mapping in inputMappings)
            {
                InputAction action = mapping.InputActionSO.GetInputAction();
                CheatInputData cheatInput = mapping.CheatInput;

                void Callback(InputAction.CallbackContext ctx) => _handler.RegisterInput(cheatInput);

                action.performed += Callback;
                action.Enable();

                _bindings.Add((action, Callback));
            }
        }

        private void OnDisable()
        {
            foreach (var (action, callback) in _bindings)
            {
                action.performed -= callback;
            }

            _bindings.Clear();
        }
    }
}