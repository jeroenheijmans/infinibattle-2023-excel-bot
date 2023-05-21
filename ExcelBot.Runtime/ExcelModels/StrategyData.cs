using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public class StrategyData
    {
        public ICollection<StartPositionGrid> StartPositionGrids { get; set; } = new List<StartPositionGrid>();
        public ICollection<FixedStartGrid> FixedStartGrids { get; set; } = new List<FixedStartGrid>();

        public int ChanceAtFixedStartingPosition { get; set; } = 0;
        public int FuzzynessFactor { get; set; } = 40; // TODO: From Excel

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
