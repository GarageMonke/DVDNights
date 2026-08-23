using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rulebound
{
    [CreateAssetMenu(fileName = "InputAction_", menuName = "ScriptableObjects/Input/Data/InputAction", order = 0)]
    public class InputActionSO : ScriptableObject
    {
        [Header("References")] 
        [SerializeField] private InputActionAsset _inputActionAsset;

        [Header("Configuration")] 
        [SerializeField] private string _inputActionName;

        private InputAction _inputAction;

        public InputAction GetInputAction()
        {
            _inputAction = _inputActionAsset.FindAction(_inputActionName);

            if (ReferenceEquals(_inputAction, null))
            {
                throw new Exception("There isn't an action with name: " + _inputActionName +
                                    " on the InputActionAsset with name: " + _inputActionAsset.name);
            }

            return _inputAction;
        }
    }
}