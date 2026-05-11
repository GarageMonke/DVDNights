using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DiskLevelController : MonoBehaviour, IDiskLevelController
    {
        private int _diskBorderBonusLevel;
        private int _diskCornerBonusLevel;
        private int _diskFFBonusLevel;
        private int _diskFFMultLevel;
        private int _diskFFDrainRateLevel;

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

        public int DiskFFBonusLevel 
        {
            get => _diskFFBonusLevel;
            set => _diskFFBonusLevel = value;
        }
        
        public int DiskFFMultLevel
        {
            get => _diskFFMultLevel;
            set => _diskFFMultLevel = value;
        }
        
        public int DiskFFDrainRateLevel
        {
            get => _diskFFDrainRateLevel;
            set => _diskFFDrainRateLevel = value;
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
            _diskFFBonusLevel = 0;
            _diskFFMultLevel = 0;
            _diskFFDrainRateLevel = 0;
        }
    }

    public interface IDiskLevelController
    {
        public int DiskBorderBonusLevel { get; set; }
        public int DiskCornerBonusLevel { get; set; }
        public int DiskFFBonusLevel { get; set; }
        public int DiskFFMultLevel { get; set; }
        public int DiskFFDrainRateLevel { get; set; }
    }
}