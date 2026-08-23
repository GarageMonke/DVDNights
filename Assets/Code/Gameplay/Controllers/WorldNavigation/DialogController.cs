using System;
using Code.Gameplay.Dialogues;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class DialogController : MonoBehaviour, IDialogController
    {
        [Header("References")]
        [SerializeField] private DialogWindow dialogWindow;

        private IDialogWindow _dialogWindow;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _dialogWindow = dialogWindow;
            ServiceLocator.RegisterService<IDialogController>(this);
        }

        public void DisplayDialog(string dialog)
        {
            if (string.IsNullOrEmpty(dialog))
            {
                return;
            }

            _dialogWindow.Display();
            _dialogWindow.UpdateDialog(dialog);
        }

        public void HideDialog()
        {
            _dialogWindow.Hide();
        }
    }

    public interface IDialogController
    {
        public void DisplayDialog(string dialog);
        public void HideDialog();
    }
}