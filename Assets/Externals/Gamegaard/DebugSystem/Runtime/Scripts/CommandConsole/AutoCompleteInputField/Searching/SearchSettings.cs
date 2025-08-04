namespace Gamegaard.RuntimeDebug
{
    public struct SearchSettings
    {
        public int StartsWithScore { get; set; }
        public int ContainsScore { get; set; }
        public int CharacterMatchScore { get; set; }
        public int MaxPenaltyFactor { get; set; }
        public int LengthPenaltyFactor { get; set; }
        public int MaxPrefixLength { get; set; }
        public double ScalingFactor { get; set; }

        public static SearchSettings Default => new SearchSettings
        {
            StartsWithScore = 30,
            ContainsScore = 20,
            CharacterMatchScore = 12,
            MaxPenaltyFactor = 3,
            LengthPenaltyFactor = 2,
            MaxPrefixLength = 4,
            ScalingFactor = 0.1
        };
    }
}