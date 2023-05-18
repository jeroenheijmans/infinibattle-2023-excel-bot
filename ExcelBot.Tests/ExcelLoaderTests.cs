using ExcelBot.ExcelModels;

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
    }
}
