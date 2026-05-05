using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DiskLevelController : MonoBehaviour, IDiskLevelController
    {
        private int _diskBorderBonusLevel;
        private int _diskCornerBonusLevel;
        private int _diskSpeedBonusLevel;

        public int DiskBorderBonusLevel
        {
            get => _diskBorderBonusLevel;
            set => _diskBorderBonusLevel = value;
        }

        public int DiskCornerBonusLevel
        {
            get => _diskCornerBonusLevel;
            set => _diskCornerBonusLevel = value;
        }

        public int DiskSpeedBonusLevel
        {
            get => _diskSpeedBonusLevel;
            set => _diskSpeedBonusLevel = value;
        }

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDiskLevelController>(this);
            
            //Load Bonus Levels
            _diskBorderBonusLevel = 0;
            _diskCornerBonusLevel = 0;
            _diskSpeedBonusLevel = 0;
        }
    }

    public interface IDiskLevelController
    {
        public int DiskBorderBonusLevel { get; set; }
        public int DiskCornerBonusLevel { get; set; }
        public int DiskSpeedBonusLevel { get; set; }
    }
}