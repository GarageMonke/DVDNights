using System;

namespace DVDNights
{
    public static class BounceGameProgression
    {
        // BASE CONSTANTS 
        public static readonly int DiscBaseBorderPoints = 1;
        public static readonly int DiscBaseCornerPoints = 10;
        public static readonly int DiscBaseSpeed = 200;
        public static readonly int DiscMergeAmount = 3;
        public static readonly long MaxPoints = 2_147_483_647;
        public static readonly int GoldenDiscsToCollect = 3;

        // Disc base purchase costs per tier
        private static int DiscBaseCost => 10;
        
        private static readonly float[] FFClickBonus = { 1f, 2f, 2.5f, 4f, 5f, 10f, 12.5f, 15f, 20f, 50f, 100f };
        private static readonly float[] FFMult =  { 1.5f, 2f, 2.5f, 3f, 5f, 8f, 10f, 12f, 15f, 20f, 25f};
        public static readonly float[] FFDrainRate = { 25f, 20f, 15f, 12.5f, 10f, 8f, 5f, 2.5f, 2f, 1f, 0.1f };
        
        private static readonly int[] BorderBonusCosts = {
            10, 120, 600, 2500, 15000, 35000,
            60000, 150000, 260000, 520000, 1000000,
            5000000, 10000000, 22000000, 310000000, 440000000,
            550000000, 650000000, 750000000, 1200000000, -1
        };

        private static readonly int[] CornerBonusCosts = {
            100, 500, 2500, 15000, 40000, 150000,
            520000, 1000000, 5000000, 10000000, 22000000,
            310000000, 440000000, 550000000, 650000000, 750000000,
            1000000000, 1100000000, 1350000000, 1500000000, -1
        };

        private static readonly int[] FFBonusCosts = {
            100, 1000, 10000, 250000, 565000, 1200000,
            5000000, 20000000, 50000000, 100000000, -1
        };

        public static int GetDiscCost(int acquired)
        {
            if (acquired <= 3)
            {
                return DiscBaseCost;
            }
            
            double cost = acquired * DiscBaseCost * Math.Pow(1.1, acquired + 1);
            return (int)Math.Min(MaxPoints, cost);
        }
        
        public static int GetBonusMaxLevel() => 20;

        public static float GetFFClickBonus(int level) => FFClickBonus[level];
        
        public static int GetBorderBonusCost(int level) => BorderBonusCosts[level];
        public static int GetCornerBonusCost(int level) => CornerBonusCosts[level];
        
        public static int GetFFBonusCost(int level) => FFBonusCosts[level];
        
        public static float GetFFLevelMult(int level) => FFMult[level];
        
        public static float GetFFDrainRate(int level) => FFDrainRate[level];
        public static int GetFFMaxLevel() => FFClickBonus.Length - 1;

        public static int GetBorderBonusMult(int level) => BorderBonusMultipliers[level];
        public static int GetCornerBonusMult(int level) => CornerBonusMultipliers[level];
        
        
        // Border: starts strong, doubles roughly every 3-4 levels, explodes at the end
        private static readonly int[] BorderBonusMultipliers = {
            1,      // 0 
            4,      // 1 
            8,      // 2
            20,     // 3
            60,     // 4
            100,    // 5
            250,    // 6
            450,    // 7
            650,    // 8
            1800,   // 9
            5000,   // 10
            7000,   // 11
            12000,  // 12
            20000,  // 13
            35000,  // 14
            60000,  // 15
            100000, // 16
            175000, // 17
            300000, // 18
            520000, // 19
            1000000 // 20
        };
        
        private static readonly int[] CornerBonusMultipliers = {
            5,       // 0
            15,      // 1
            50,      // 2
            120,     // 3
            280,     // 4
            600,     // 5
            1300,    // 6
            2800,    // 7
            6000,    // 8
            13000,   // 9
            28000,   // 10
            60000,   // 11
            130000,  // 12
            280000,  // 13
            600000,  // 14
            1300000, // 15
            2800000, // 16
            6000000, // 17
            13000000,// 18
            28000000,// 19
            60000000 // 20
        };
    }
}