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
                    double currentBonusAmountToAdd = GameProgression.DiscBaseBorderPoints * GameProgression.GetTierExtraMult((int) DiskType.WHITE) * GameProgression.GetBorderBonusMult(_diskLevelController.DiskBorderBonusLevel) + GameProgression.GetTierExtraPoints((int) DiskType.WHITE);
                    currentBonusAmountToAdd = Mathf.CeilToInt((float)currentBonusAmountToAdd);
                    int currentFormattedBonusAmountToAdd = Mathf.Max(1, (int)currentBonusAmountToAdd);
                    
                    double nextBonusAmountToAdd = GameProgression.DiscBaseBorderPoints * GameProgression.GetTierExtraMult((int) DiskType.WHITE) * GameProgression.GetBorderBonusMult(_diskLevelController.DiskBorderBonusLevel + 1) + GameProgression.GetTierExtraPoints((int) DiskType.WHITE);
                    nextBonusAmountToAdd = Mathf.CeilToInt((float)nextBonusAmountToAdd);
                    int nextFormattedBonusAmountToAdd = Mathf.Max(1, (int)nextBonusAmountToAdd);
                    
                    shopItemInfo = $"{currentFormattedBonusAmountToAdd} -> {nextFormattedBonusAmountToAdd}";
                    break;
                //Disk Corner Bonus Level
                case 2:
                    double currentCornerAmountToAdd = GameProgression.DiscBaseCornerPoints * GameProgression.GetTierExtraMult((int) DiskType.WHITE) * GameProgression.GetCornerBonusMult(_diskLevelController.DiskCornerBonusLevel + 1)  + GameProgression.GetTierExtraPoints((int) DiskType.WHITE);
                    currentCornerAmountToAdd = Mathf.CeilToInt((float)currentCornerAmountToAdd);
                    int currentCornerFormattedAmountToAdd = Mathf.Max(1, (int)currentCornerAmountToAdd);
                    
                    double nextCornerAmountToAdd = GameProgression.DiscBaseCornerPoints * GameProgression.GetTierExtraMult((int) DiskType.WHITE) * GameProgression.GetCornerBonusMult(_diskLevelController.DiskCornerBonusLevel + 2)  + GameProgression.GetTierExtraPoints((int) DiskType.WHITE);
                    nextCornerAmountToAdd = Mathf.CeilToInt((float)nextCornerAmountToAdd);
                    int nextFormattedCornerAmountToAdd = Mathf.Max(1, (int)nextCornerAmountToAdd);
                    
                    shopItemInfo = $"{currentCornerFormattedAmountToAdd} -> {nextFormattedCornerAmountToAdd}";
                    break;
                //FF Bonus Level
                case 3:
                    float currentDiskFFBonus = GameProgression.GetFFPoints(_diskLevelController.DiskFFBonusLevel);
                    
                    if (_diskLevelController.DiskFFBonusLevel == GameProgression.GetFFMaxLevel())
                    {
                        shopItemInfo = $"{currentDiskFFBonus} (MAX)";
                        break;
                    }
                    
                    int nextDiskFFBonus = GameProgression.GetFFPoints(_diskLevelController.DiskFFBonusLevel + 1);;
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
                    shopItemCost = GameProgression.GetDiscCost((int)DiskType.WHITE, _disksController.DisksRegistered - 1);
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
                    shopItemCost = GameProgression.GetFFCost(_diskLevelController.DiskFFBonusLevel);
                    break;
                //FF Mult Level
                case 4:
                    shopItemCost = GameProgression.GetFFCost(_diskLevelController.DiskFFMultLevel);
                    break;
                //FF Drain Rate
                case 5:
                    shopItemCost = GameProgression.GetFFCost(_diskLevelController.DiskFFDrainRateLevel);
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