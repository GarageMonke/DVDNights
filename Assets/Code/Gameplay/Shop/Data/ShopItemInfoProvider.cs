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
                    int currentDiskBaseBonusLevel = _diskLevelController.DiskBorderBonusLevel;
                   
                    if (_diskLevelController.DiskBorderBonusLevel == GameProgression.GetBonusMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskBaseBonusLevel} (MAX)";
                        break;
                    }
                    
                    int nextDiskBaseBonusLevel =  currentDiskBaseBonusLevel + 1;
                    shopItemInfo = $"{currentDiskBaseBonusLevel} -> {nextDiskBaseBonusLevel}";
                    break;
                //Disk Corner Bonus Level
                case 2:
                    int currentDiskCornerBonusLevel = _diskLevelController.DiskCornerBonusLevel;
                    
                    if (_diskLevelController.DiskCornerBonusLevel == GameProgression.GetBonusMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskCornerBonusLevel} (MAX)";
                        break;
                    }
                  
                    int nextDiskCornerBonusLevel =  currentDiskCornerBonusLevel + 1;
                    shopItemInfo = $"{currentDiskCornerBonusLevel} -> {nextDiskCornerBonusLevel}";
                    break;
                //FF Bonus Level
                case 3:
                    float currentDiskFFBonus = GameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel);
                    
                    if (_diskLevelController.DiskFFBonusLevel == GameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFBonus} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFBonus = GameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel + 1);;
                    shopItemInfo = $"{currentDiskFFBonus} -> {nextDiskFFBonus}";
                    break;
                //FF Speed Level
                case 4:
                    float currentDiskFFMult = GameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel);
                    if (_diskLevelController.DiskFFMultLevel == GameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFMult} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFMult = GameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel + 1);;
                    shopItemInfo = $"{currentDiskFFMult} -> {nextDiskFFMult}";
                    break;
                //FF Drain Rate
                case 5:
                    float currentDiskFFDrainRate = GameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel);
                    
                    if (_diskLevelController.DiskFFDrainRateLevel == GameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFDrainRate} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFDrainRate = GameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel + 1);;
                    shopItemInfo = $"{currentDiskFFDrainRate} -> {nextDiskFFDrainRate}";
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
                    shopItemCost = GameProgression.GetDiscCost(_disksController.DisksRegistered - 1);
                    break;
                //Disk Base Bonus Level
                case 1:
                    shopItemCost = GameProgression.GetBorderBonusCost(_diskLevelController.DiskBorderBonusLevel);
                    break;
                //Disk Corner Bonus Level
                case 2:
                    shopItemCost = GameProgression.GetCornerBonusCost(_diskLevelController.DiskCornerBonusLevel);
                    break;
                //FF Bonus Level
                case 3:
                    shopItemCost = GameProgression.GetFFBonusCost(_diskLevelController.DiskFFBonusLevel);
                    break;
                //FF Mult Level
                case 4:
                    shopItemCost = GameProgression.GetFFBonusCost(_diskLevelController.DiskFFMultLevel);
                    break;
                //FF Drain Rate
                case 5:
                    shopItemCost = GameProgression.GetFFBonusCost(_diskLevelController.DiskFFDrainRateLevel);
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