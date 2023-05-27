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
        public void Can_load_grid_for(string Rank)
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            var grid = result.StartPositionGrids.FirstOrDefault(p => p.Rank == Rank);
            grid.Should().NotBeNull();
            grid?.Probabilities.Should().Contain(p => p.Value > 0);
        }

        [Fact]
        public void Can_load_grid_for_scout_smoke_test()
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            var grid = result.StartPositionGrids.FirstOrDefault(p => p.Rank == "Scout");
            grid.Should().NotBeNull();
            grid?.Probabilities.Should().Contain(p => p.Value > 0);
            grid?.Probabilities.Where(p => p.Value == 100).Should().HaveCount(10);
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
        public void Can_load_opponent_flag_probabilities()
        {
            var result = ExcelLoader.Load("strategy.xlsx");
            result.OpponentFlagProbabilities.Should().HaveCount(40);
            result.OpponentFlagProbabilities.Should().AllSatisfy(kvp =>
                kvp.Value.Should().BeGreaterThanOrEqualTo(0)
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
            result.BonusPointsForMovesGettingCloserToPotentialFlags.Should().Be(1000);
            result.ScoutJumpsToPotentialFlagsMultiplication.Should().Be(true);

            result.FuzzynessFactor.Should().Be(5);

            result.BoostForSpy.Should().Be(0);
            result.BoostForScout.Should().Be(5);
            result.BoostForMiner.Should().Be(10);
            result.BoostForGeneral.Should().Be(15);
            result.BoostForMarshal.Should().Be(20);
        }
    }
}
