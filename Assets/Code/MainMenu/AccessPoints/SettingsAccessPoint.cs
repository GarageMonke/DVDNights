using UnityEngine;

namespace Rulebound
{
    public class SettingsAccessPoint : WindowAccessPoint<SettingsWindow>
    {
        [Header("Configuration")] 
        [SerializeField] private bool returnToMainMenu;
        
        public override void Access()
        {
           base.Access();
           
           if (returnToMainMenu)
           {
               _windowAccessed.SetGoBackToMenu(false);
           }
        }
    }
}