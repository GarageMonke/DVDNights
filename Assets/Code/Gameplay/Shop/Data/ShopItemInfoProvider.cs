using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
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
                    float currentDiskBaseBonus = BounceGameProgression.GetBorderBonusMult(_diskLevelController.DiskBorderBonusLevel);
                   
                    if (_diskLevelController.DiskBorderBonusLevel == BounceGameProgression.GetBonusMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskBaseBonus.ToKMB()} (MAX)";
                        break;
                    }
                    
                    int nextDiskBaseBonusLevel =  BounceGameProgression.GetBorderBonusMult(_diskLevelController.DiskBorderBonusLevel + 1);
                    shopItemInfo = $"{currentDiskBaseBonus.ToKMB()} -> {nextDiskBaseBonusLevel.ToKMB()}";
                    break;
                //Disk Corner Bonus Level
                case 2:
                    float currentDiskCornerBonus = BounceGameProgression.GetBorderBonusMult(_diskLevelController.DiskCornerBonusLevel);
                    
                    if (_diskLevelController.DiskCornerBonusLevel == BounceGameProgression.GetBonusMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskCornerBonus.ToKMB()} (MAX)";
                        break;
                    }
                  
                    int nextDiskCornerBonusLevel = BounceGameProgression.GetCornerBonusMult(_diskLevelController.DiskCornerBonusLevel + 1);
                    shopItemInfo = $"{currentDiskCornerBonus.ToKMB()} -> {nextDiskCornerBonusLevel.ToKMB()}";
                    break;
                //FF Bonus Level
                case 3:
                    float currentDiskFFBonus = BounceGameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel);
                    
                    if (_diskLevelController.DiskFFBonusLevel == BounceGameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFBonus.ToKMB()} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFBonus = BounceGameProgression.GetFFClickBonus(_diskLevelController.DiskFFBonusLevel + 1);;
                    shopItemInfo = $"{currentDiskFFBonus.ToKMB()} -> {nextDiskFFBonus.ToKMB()}";
                    break;
                //FF Speed Level
                case 4:
                    float currentDiskFFMult = BounceGameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel);
                    if (_diskLevelController.DiskFFMultLevel == BounceGameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFMult} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFMult = BounceGameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel + 1);;
                    shopItemInfo = $"{currentDiskFFMult} -> {nextDiskFFMult}";
                    break;
                //FF Drain Rate
                case 5:
                    float currentDiskFFDrainRate = BounceGameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel);
                    
                    if (_diskLevelController.DiskFFDrainRateLevel == BounceGameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFDrainRate} (MAX)";
                        break;
                    }
                    
                    float nextDiskFFDrainRate = BounceGameProgression.GetFFDrainRate(_diskLevelController.DiskFFDrainRateLevel + 1);;
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
                    shopItemCost = BounceGameProgression.GetDiscCost(_disksController.DisksRegistered);
                    break;
                //Disk Base Bonus Level
                case 1:
                    shopItemCost = BounceGameProgression.GetBorderBonusCost(_diskLevelController.DiskBorderBonusLevel);
                    break;
                //Disk Corner Bonus Level
                case 2:
                    shopItemCost = BounceGameProgression.GetCornerBonusCost(_diskLevelController.DiskCornerBonusLevel);
                    break;
                //FF Bonus Level
                case 3:
                    shopItemCost = BounceGameProgression.GetFFBonusCost(_diskLevelController.DiskFFBonusLevel);
                    break;
                //FF Mult Level
                case 4:
                    shopItemCost = BounceGameProgression.GetFFBonusCost(_diskLevelController.DiskFFMultLevel);
                    break;
                //FF Drain Rate
                case 5:
                    shopItemCost = BounceGameProgression.GetFFBonusCost(_diskLevelController.DiskFFDrainRateLevel);
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