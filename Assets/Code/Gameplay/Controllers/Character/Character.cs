using UnityEngine;

namespace Rulebound
{
    public class Character : MonoBehaviour, ICharacter
    {
        public Transform CharacterTransform => transform;
    }
    
    public interface ICharacter
    {
        public Transform CharacterTransform { get; }
    }
}