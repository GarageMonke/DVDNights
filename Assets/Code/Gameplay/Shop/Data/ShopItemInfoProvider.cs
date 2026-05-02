using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class ShopItemInfoProvider : MonoBehaviour, IShopItemInfoProvider
    {
        private IDiskLevelController _diskLevelController;
        private IDisksController _disksController;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IShopItemInfoProvider>(this);
        }

        private void Start()
        {
            _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
            _disksController = ServiceLocator.GetService<IDisksController>();
        }

        public string GetInfoByItemId(int shopItemId)
        {
            string shopItemInfo = "";
            switch (shopItemId)
            {
                //Buy White Disk
                case 0:
                    shopItemInfo = "+ 1";
                    break;
                //Disk Base Bonus Level
                case 1:
                    int currentDiskBaseBonusLevel = _diskLevelController.DiskBorderBonusLevel + 1;
                    int nextDiskBaseBonusLevel =  currentDiskBaseBonusLevel + 1;
                    shopItemInfo = $"{currentDiskBaseBonusLevel} -> {nextDiskBaseBonusLevel}";
                    break;
                //Disk Speed Level
                case 2:
                    int currentDiskSpeedBonusLevel = _diskLevelController.DiskSpeedBonusLevel + 1;
                    int nextDiskSpeedBonusLevel =  currentDiskSpeedBonusLevel + 1;
                    shopItemInfo = $"{currentDiskSpeedBonusLevel} -> {nextDiskSpeedBonusLevel}";
                    break;
                //Disk Corner Bonus Level
                case 3:
                    int currentDiskCornerBonusLevel = _diskLevelController.DiskCornerBonusLevel + 1;
                    int nextDiskCornerBonusLevel =  currentDiskCornerBonusLevel + 1;
                    shopItemInfo = $"{currentDiskCornerBonusLevel} -> {nextDiskCornerBonusLevel}";
                    break;
            }

            return shopItemInfo;
        }
        
        public int GetCostByItemId(int shopItemId)
        {
            int shopItemCost = 0;
            switch (shopItemId)
            {
                //Buy White Disk
                case 0:
                    shopItemCost = GameProgression.GetDiscCost((int)DiskType.WHITE, _disksController.DisksRegistered - 1);
                    break;
                //Disk Base Bonus Level
                case 1:
                    shopItemCost = GameProgression.GetBorderBonusCost(_diskLevelController.DiskBorderBonusLevel);
                    break;
                //Disk Speed Level
                case 2:
                    shopItemCost = GameProgression.GetSpeedBonusCost(_diskLevelController.DiskSpeedBonusLevel);
                    break;
                //Disk Corner Bonus Level
                case 3:
                    shopItemCost = GameProgression.GetCornerBonusCost(_diskLevelController.DiskCornerBonusLevel);
                    break;
            }

            return shopItemCost;
        }
    }

    public interface IShopItemInfoProvider
    {
        public string GetInfoByItemId(int shopItemId);
        public int GetCostByItemId(int shopItemId);
    }
}