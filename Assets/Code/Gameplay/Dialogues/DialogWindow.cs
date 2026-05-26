using DVDNights;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Dialogues
{
    public class DialogWindow : Window, IDialogWindow
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dialogText;
        
        public void UpdateDialog(string dialog)
        {
            dialogText.text = dialog;
        }
    }

    public interface IDialogWindow : IWindow
    {
        public void UpdateDialog(string dialog);
    }
}