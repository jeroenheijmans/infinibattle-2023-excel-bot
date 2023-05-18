using ExcelBot.Models;

namespace ExcelBot.Tests
{
    public class StrategyTests
    {
        [Fact]
        public void Initialization_for_red_smoke_test_001()
        {
            var strategy = new Strategy(new Random(123));
            var gameInit = GameInit.FromJson(""""
            {
                "You": 0,
                "AvailablePieces": ["Scout", "Scout", "Bomb", "Flag", "Miner", "Marshal", "Spy", "General"]
            }
            """");

            var result = strategy.initialize(gameInit);

            result.Pieces
                .Should().HaveCount(8)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(p => p.Position.Y < 4);
            result.Pieces.Select(p => p.Rank).Distinct().Should().HaveCount(7);
        }

        [Fact]
        public void Initialization_for_blue_smoke_test_001()
        {
            var strategy = new Strategy(new Random(123));
            var gameInit = GameInit.FromJson(""""
            {
                "You": 1,
                "AvailablePieces": ["Scout", "Scout", "Bomb", "Flag", "Miner", "Marshal", "Spy", "General"]
            }
            """");

            var result = strategy.initialize(gameInit);

            result.Pieces
                .Should().HaveCount(8)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(p => p.Position.Y > 5);
            result.Pieces.Select(p => p.Rank).Distinct().Should().HaveCount(7);
        }

        [Fact]
        public void Process_returns_null_if_not_your_turn()
        {
            var strategy = new Strategy(new Random(123))
            {
                MyColor = Player.Red,
            };
            var state = new GameState { ActivePlayer = Player.Blue };

            var result = strategy.Process(state);

            result.Should().BeNull();
        }

        [Fact]
        public void Process_returns_move_smoke_test_001()
        {
            var strategy = new Strategy(new Random(123))
            {
                MyColor = Player.Blue,
            };
            var state = GameState.FromJson(""""
            {
                "ActivePlayer": 1,
                "Board": [
                    { "Rank": "Scout", "Owner": 0, "Coordinate": { X: 0, Y: 0 } },
                    { "Rank": "Scout", "Owner": 1, "Coordinate": { X: 0, Y: 1 } }
                ]
            }
            """");

            var result = strategy.Process(state);

            result.Should()
                .BeOfType<Move>()
                .And.Match<Move>(m => m.From.X == 0)
                .And.Match<Move>(m => m.From.Y == 1)
                .And.Match<Move>(m => m.To.X != 0 || m.To.Y != 1);
        }
    }
}