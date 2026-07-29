using CorePatterns.Managers;
using UnityEngine;

namespace Code.TestOnly
{
    public class CanvasRegisterer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]private Canvas canvas;

        private void Awake()
        {
            WindowManager.Instance.SetContainerCanvas(canvas);
        }
    }
}