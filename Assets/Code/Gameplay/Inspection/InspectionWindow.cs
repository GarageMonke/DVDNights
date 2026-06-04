using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class InspectionWindow : Window, IInspectionWindow
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI inspectableTitle;
        [SerializeField] private TextMeshProUGUI inspectableSubtitle;
        [SerializeField] private TextMeshProUGUI inspectableDescription;
        
        
        public void UpdateInspectableInfo(string newInspectableTitle, string newInspectableSubtitle, string newInspectableDescription)
        {
            inspectableTitle.text = newInspectableTitle;
            inspectableSubtitle.text = newInspectableSubtitle;
            inspectableDescription.text = newInspectableDescription;
        }
    }

    public interface IInspectionWindow : IWindow
    {
        public void UpdateInspectableInfo(string inspectableTitle, string inspectableSubtitle, string inspectableDescription);
    }
}