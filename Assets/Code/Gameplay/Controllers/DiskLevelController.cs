using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DiskLevelController : MonoBehaviour, IDiskLevelController
    {
        private int _diskBorderBonusLevel;
        private int _diskCornerBonusLevel;
        private int _diskSpeedBonusLevel;

        public int DiskBorderBonusLevel => _diskBorderBonusLevel;
        public int DiskCornerBonusLevel => _diskCornerBonusLevel; 
        public int DiskSpeedBonusLevel =>  _diskSpeedBonusLevel;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IDiskLevelController>(this);
            
            //Load Bonus Levels
            UpdateDiskBorderBonusLevel(1);
            UpdateDiskCornerBonusLevel(1);
            UpdateDiskSpeedBonusLevel(1);
        }

        public void UpdateDiskBorderBonusLevel(int updatedDiskBorderBonusLevel)
        {
            _diskBorderBonusLevel = updatedDiskBorderBonusLevel;
        }

        public void UpdateDiskCornerBonusLevel(int updatedDiskCornerBonusLevel)
        {
            _diskCornerBonusLevel = updatedDiskCornerBonusLevel;
        }

        public void UpdateDiskSpeedBonusLevel(int updatedDiskSpeedBonusLevel)
        {
            _diskSpeedBonusLevel = updatedDiskSpeedBonusLevel;
        }
    }

    public interface IDiskLevelController
    {
        public int DiskBorderBonusLevel { get; }
        public int DiskCornerBonusLevel { get; }
        public int DiskSpeedBonusLevel { get; }

        public void UpdateDiskBorderBonusLevel(int updatedDiskBorderBonusLevel);
        public void UpdateDiskCornerBonusLevel(int updatedDiskCornerBonusLevel);
        public void UpdateDiskSpeedBonusLevel(int updatedDiskSpeedBonusLevel);
    }
}