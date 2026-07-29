using UnityEngine;

namespace Code.MainMenu.AccessPoints
{
    public abstract class AccessPoint : MonoBehaviour, IAccessPoint
    {
        public abstract void Access();
    }

    public interface IAccessPoint
    {
        public void Access();
    }
}