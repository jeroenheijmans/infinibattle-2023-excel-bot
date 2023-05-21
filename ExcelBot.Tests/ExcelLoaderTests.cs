using ExcelBot.Runtime.ExcelModels;

namespace ExcelBot.Tests
{
    public class ExcelLoaderTests
    {
        [Theory]
        [InlineData("Scout")]
        [InlineData("Bomb")]
        [InlineData("Flag")]
        [InlineData("Miner")]
        [InlineData("Marshal")]
        [InlineData("Spy")]
        [InlineData("General")]
        public void Can_load_flag_grid(string Rank)
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            var grid = result.StartPositionGrids.FirstOrDefault(p => p.Rank == Rank);
            grid.Should().NotBeNull();
            grid?.Probabilities.Should().Contain(p => p.Value > 0);
        }

        [Fact]
        public void Can_load_fixed_grids()
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            result.FixedStartGrids.Should().HaveCount(2);
            result.FixedStartGrids.Should().AllSatisfy(g =>
                g.StartingPositions.Should().HaveCount(8)
            );
        }

        [Fact]
        public void Can_load_strategy_variables()
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            result.ChanceAtFixedStartingPosition.Should().BeGreaterThan(0);
        }
    }
}
