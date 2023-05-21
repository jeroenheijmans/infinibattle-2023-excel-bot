using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public class StrategyData
    {
        public ICollection<StartPositionGrid> StartPositionGrids { get; set; } = new List<StartPositionGrid>();
        public ICollection<FixedStartGrid> FixedStartGrids { get; set; } = new List<FixedStartGrid>();
        // TODO: Strategy Variables

        // TODO: Unit tests
        public void TransposeAll()
        {
            foreach (var grid in StartPositionGrids)
            {
                grid.Transpose();
            }
        }

    }
}
