using UnityEngine;

namespace Code.MainMenu.AccessPoints
{
    public abstract class AccessPoint : MonoBehaviour, IAccessPoint
    {
        [Header("Configuration")]
        [SerializeField] private bool accessOnStart;

        private void Start()
        {
            if (accessOnStart)
            {
                Access();
            }
        }

        public abstract void Access();
    }

    public interface IAccessPoint
    {
        public void Access();
    }
}