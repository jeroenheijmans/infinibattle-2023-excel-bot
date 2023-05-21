using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public class StrategyData
    {
        public ICollection<StartPositionGrid> StartPositionGrids { get; set; } = new List<StartPositionGrid>();
        public ICollection<FixedStartGrid> FixedStartGrids { get; set; } = new List<FixedStartGrid>();

        public int ChanceAtFixedStartingPosition { get; set; } = 0;
        
        public int DecisiveVictoryPoints { get; set; } = 0;
        public int DecisiveLossPoints { get; set; } = 0;
        public int UnknownBattleOwnHalfPoints { get; set; } = 0;
        public int UnknownBattleOpponentHalfPoints { get; set; } = 0;
        public int BonusPointsForMoveTowardsOpponent { get; set; } = 0;
        public int BonusPointsForMoveWithinOpponentArea { get; set; } = 0;
        public int BonusPointsForMovesGettingCloserToPotentialFlags { get; set; } = 0;

        public int FuzzynessFactor { get; set; } = 0;

        public int BoostForSpy { get; set; } = 0;
        public int BoostForScout { get; set; } = 0;
        public int BoostForMiner { get; set; } = 0;
        public int BoostForGeneral { get; set; } = 0;
        public int BoostForMarshal { get; set; } = 0;

        // TODO: Unit tests
        public void TransposeAll()
        {
            foreach (var grid in StartPositionGrids)
            {
                grid.Transpose();
            }

            foreach (var grid in FixedStartGrids)
            {
                grid.Transpose();
            }
        }

    }
}
