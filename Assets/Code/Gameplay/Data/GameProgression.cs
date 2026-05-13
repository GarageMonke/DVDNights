using System;

namespace DVDNights
{
    public static class GameProgression
    {
        // ── BASE CONSTANTS ───────────────────────────────────────────────────────
        public static readonly int DiscBaseBorderPoints = 1;
        public static readonly int DiscBaseCornerPoints = 10;
        public static readonly int DiscBaseSpeed = 200;
        public static readonly int DiscMergeAmount = 3;
        public static readonly long MaxPoints = 2_147_483_647;
        
        // Income per disc
        private static readonly int[] TierPoints = { 0, 1, 3, 9, 27, 81, 243, 729 };

        // Late game boost
        private static readonly int[] TierLateMult = { 1, 1, 1, 1, 1, 1, 5, 25 };

        // Disc base purchase costs per tier
        private static int DiscBaseCost => 10;
        
        private static readonly float[] FFClickBonus = { 1f, 2.5f, 5f, 10f, 15f, 20f, 25f, 35f, 50f, 100f };
        private static readonly float[] FFMult =  { 1.5f, 2f, 3f, 5f, 7f, 9f, 10f, 12f, 15f, 20f, 25f};
        public static readonly float[] FFDrainRate = { 10f, 8.5f, 6f, 4.5f, 3f, 2.5f, 2f, 1f, 0.5f, 0.1f };

        public static int GetDiscCost(int acquired)
        {
            if (acquired < 3) return DiscBaseCost;
            double cost = DiscBaseCost * Math.Pow(1.00864, acquired - 2);
            return (int)Math.Min(MaxPoints - 1, cost);
        }
        
        public static int GetBonusMaxLevel() => 20;
            
        public static int GetTierExtraPoints(int tier) => TierPoints[tier];
        public static int GetTierExtraMult(int tier) => TierLateMult[tier];

        public static float GetFFClickBonus(int level) => FFClickBonus[level];
        // L0=500  L3=7.3K  L6=108K  L9=1.6M
        
        public static int GetFFBonusCost(int level) => (int)(10000 * Math.Pow(2, level));
        
        public static float GetFFLevelMult(int level) => FFMult[level];
        
        public static float GetFFDrainRate(int level) => FFDrainRate[level];
        public static int GetFFMaxLevel() => FFClickBonus.Length - 1;
        
        
        // L0=1x  L5=20x  L10=80x  L15=250x  L20=600x
        public static double GetCornerBonusMult(int level)
        {
            if (level == 0)
            {
                return 1;
            }
            
            return Math.Pow(level + 1, 2.7);
        }

        public static int GetCornerBonusCost(int level) => (int)(150 * Math.Pow(2.0, level));
        // L0=150  L5=4.8K  L10=153K  L15=4.9M  L20=157M
        
        
        public static double GetBorderBonusMult(int level)
        {
            if (level == 0)
            {
                return 1;
            }
            
            return Math.Pow(level + 1, 3);
        }
        
              
        // L0=50  L5=700  L10=10K  L15=150K  L20=2M
        public static int GetBorderBonusCost(int level) => (int)(50 * Math.Pow(1.85, level));
    }
}