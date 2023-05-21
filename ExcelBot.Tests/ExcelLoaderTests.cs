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

            result.ChanceAtFixedStartingPosition.Should().Be(50);

            result.DecisiveVictoryPoints.Should().Be(250);
            result.DecisiveLossPoints.Should().Be(-1000);
            result.UnknownBattleOwnHalfPoints.Should().Be(-250);
            result.UnknownBattleOpponentHalfPoints.Should().Be(500);
            result.BonusPointsForMoveTowardsOpponent.Should().Be(50);
            result.BonusPointsForMoveWithinOpponentArea.Should().Be(100);

            result.FuzzynessFactor.Should().Be(25);

            result.BoostForSpy.Should().Be(0);
            result.BoostForScout.Should().Be(5);
            result.BoostForMiner.Should().Be(10);
            result.BoostForGeneral.Should().Be(15);
            result.BoostForMarshal.Should().Be(20);
        }
    }
}
