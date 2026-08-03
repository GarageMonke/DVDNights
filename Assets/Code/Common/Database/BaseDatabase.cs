using System.Collections.Generic;
using UnityEngine;

namespace Code.Common.Database
{
    public abstract class BaseDatabase<T> : ScriptableObject
    {
        [SerializeField] private List<T> database = new();

        public IReadOnlyList<T> Database => database;

#if UNITY_EDITOR
        public void SetWindows(List<T> newDatabase)
        {
            database = newDatabase;
        }
#endif
    }
}