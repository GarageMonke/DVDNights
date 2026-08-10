using UnityEngine;

namespace CheatCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CheatCodeHandler
    {
        private readonly List<CheatCodeDefinition> _codes;
        private readonly List<CheatInputData> _buffer = new List<CheatInputData>();
        private readonly Func<float> _timeProvider;

        private float _lastInputTime;
        private readonly int _maxSequenceLength;

        public event Action<CheatCodeDefinition> CodeActivated;


        public CheatCodeHandler(IEnumerable<CheatCodeDefinition> codes, Func<float> timeProvider)
        {
            _codes = codes.ToList();
            _timeProvider = timeProvider;
            _maxSequenceLength = _codes.Count > 0 ? _codes.Max(c => c.Sequence.Length) : 0;
        }

        public void RegisterInput(CheatInputData input)
        {
            float now = _timeProvider();

            if (_buffer.Count > 0 && HasTimedOut(now))
            {
                _buffer.Clear();
            }

            _lastInputTime = now;
            _buffer.Add(input);
            TrimBuffer();
            
            Debug.Log("Input Registered " + input);

            TryMatch();
        }

        public void Reset() => _buffer.Clear();

        private bool HasTimedOut(float now)
        {
            float timeout = _codes.Count > 0 ? _codes.Max(c => c.MaxIntervalBetweenInputs) : 1.5f;
            return now - _lastInputTime > timeout;
        }

        private void TryMatch()
        {
            foreach (CheatCodeDefinition code in _codes)
            {
                if (Matches(code.Sequence))
                {
                    _buffer.Clear();
                    CodeActivated?.Invoke(code);
                    code.Execute();
                    return;
                }
            }
        }

        private bool Matches(CheatInputData[] sequence)
        {
            if (sequence.Length == 0 || _buffer.Count < sequence.Length)
                return false;

            int offset = _buffer.Count - sequence.Length;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (_buffer[offset + i] != sequence[i])
                    return false;
            }

            return true;
        }

        private void TrimBuffer()
        {
            while (_buffer.Count > _maxSequenceLength)
                _buffer.RemoveAt(0);
        }
    }
}