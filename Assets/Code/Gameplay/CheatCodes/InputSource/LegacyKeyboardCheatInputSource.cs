namespace CheatCodes.InputSource
{
    using System.Collections.Generic;
    using UnityEngine;

    namespace Code.Gameplay.Misc.Examples
    {
        public class LegacyKeyboardCheatInputSource : MonoBehaviour
        {
            [SerializeField] private List<CheatCodeDefinition> registeredCodes;

            private static readonly Dictionary<KeyCode, CheatInputData> KeyMap = new()
            {
                { KeyCode.UpArrow, CheatInputData.Up },
                { KeyCode.DownArrow, CheatInputData.Down },
                { KeyCode.LeftArrow, CheatInputData.Left },
                { KeyCode.RightArrow, CheatInputData.Right },
                { KeyCode.A, CheatInputData.A },
                { KeyCode.B, CheatInputData.B },
                { KeyCode.Return, CheatInputData.Start },
                { KeyCode.Backspace, CheatInputData.Select },
            };

            private CheatCodeHandler _handler;

            private void Awake()
            {
                _handler = new CheatCodeHandler(registeredCodes, () => Time.time);
            }

            private void Update()
            {
                foreach (KeyValuePair<KeyCode, CheatInputData> pair in KeyMap)
                {
                    if (Input.GetKeyDown(pair.Key))
                    {
                        _handler.RegisterInput(pair.Value);
                    }
                }
            }
        }
    }
}