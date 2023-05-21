using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public class StrategyData
    {
        public ICollection<StartPositionGrid> StartPositionGrids { get; set; } = new List<StartPositionGrid>();

        // TODO: Unit tests
        public void TransposeAll()
        {
            foreach (var grid in StartPositionGrids)
            {
                grid.Transpose();
            }
        }

        // TODO: Fixed Starting Positions
        // TODO: Strategy Variables
    }
}
