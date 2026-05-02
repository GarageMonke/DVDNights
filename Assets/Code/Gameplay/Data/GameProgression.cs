using System;

namespace DVDNights
{
    public static class GameProgression
    {
        public static readonly int DiscBaseBorderPoints = 1;
        public static readonly int DiscBaseCornerPoints = 1;
        public static readonly int DiscBaseSpeed = 200;
        public static readonly long MaxPoints = 2_147_483_647;
        
        // Income per disc
        private static readonly int[] TierPoints = { 0, 1, 3, 9, 27, 81, 243, 729 };

        // Late game boost
        private static readonly int[] TierLateMult = { 1, 1, 1, 1, 1, 1, 5, 25 };

        // Disc base purchase costs per tier
        private static readonly long[] DiscBaseCost = { 0, 400, 2000, 10000, 50000, 250000, 1250000 };

        public static long GetDiscCost(int tier, int acquired) => (long)(DiscBaseCost[tier] * Math.Pow(1.45, acquired));
        public static int GetTierExtraPoints(int tier) => TierPoints[tier];
        public static int GetTierExtraMult(int tier) => TierLateMult[tier];

        public static double GetSpeedBonusMult(int level) => 1.0 + level * 0.06;
        public static long GetSpeedBonusCost(int level) => (long)(200 * Math.Pow(1.38, level));

        public static double GetBorderBonusMult(int level) => 1.0 + level * 0.18;
        public static long GetBorderBonusCost(int level) => (long)(300 * Math.Pow(1.43, level));

        public static double GetCornerBonusMult(int level) => 1.0 + level * 0.22;
        public static long GetCornerBonusCost(int level) => (long)(500 * Math.Pow(1.48, level));

        public static double DiscIncomePerMinute(int tier, int speedLvl, int borderLvl, int cornerLvl)
        {
            double hpm = 30.0 * GetSpeedBonusMult(speedLvl);
            double pts = TierPoints[tier] * TierLateMult[tier];
            double border = hpm * pts * GetBorderBonusMult(borderLvl);
            double corner = hpm * (1.0 / 12.0) * pts * 5.0 * GetCornerBonusMult(cornerLvl);
            return border + corner;
        }
    }
}