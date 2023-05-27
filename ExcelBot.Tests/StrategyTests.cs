using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using ExcelBot.Runtime;
using Xunit;

namespace ExcelBot.Tests
{
    public class StrategyTests
    {
        private const string gameInitForRed = """"
        {
            "You": 0,
            "AvailablePieces": ["Scout", "Scout", "Bomb", "Flag", "Miner", "Marshal", "Spy", "General"]
        }
        """";
        private const string gameInitForBlue = """"
        {
            "You": 1,
            "AvailablePieces": ["Scout", "Scout", "Bomb", "Flag", "Miner", "Marshal", "Spy", "General"]
        }
        """";

        [Fact]
        public void Initialization_for_red_smoke_test_001()
        {
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData);
            var gameInit = GameInit.FromJson(gameInitForRed);

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
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData);
            var gameInit = GameInit.FromJson(gameInitForBlue);

            var result = strategy.initialize(gameInit);

            result.Pieces
                .Should().HaveCount(8)
                .And.OnlyHaveUniqueItems()
                .And.OnlyContain(p => p.Position.Y > 5);
            result.Pieces.Select(p => p.Rank).Distinct().Should().HaveCount(7);
        }

        [Theory]
        [InlineData("Flag", 0, 0)]
        [InlineData("Bomb", 1, 0)]
        [InlineData("Spy", 2, 1)]
        [InlineData("Scout", 3, 1)]
        [InlineData("Scout", 4, 2)]
        [InlineData("Miner", 5, 2)]
        [InlineData("General", 6, 3)]
        [InlineData("Marshal", 7, 3)]
        public void Initialization_can_set_fixed_start_for_each_piece_with_red(string rank, int x, int y)
        {
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData);
            var gameInit = GameInit.FromJson(gameInitForRed);

            strategyData.ChanceAtFixedStartingPosition = 100;

            var result = strategy.initialize(gameInit);


            result.Pieces.Should().Contain(p => p.Rank == rank && p.Position == new Point(x, y));
        }

        [Theory]
        [InlineData("Flag", 0, 0)]
        [InlineData("Bomb", 1, 0)]
        [InlineData("Spy", 2, 1)]
        [InlineData("Scout", 3, 1)]
        [InlineData("Scout", 4, 2)]
        [InlineData("Miner", 5, 2)]
        [InlineData("General", 6, 3)]
        [InlineData("Marshal", 7, 3)]
        public void Initialization_can_set_fixed_start_for_each_piece_with_blue(string rank, int x, int y)
        {
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData);
            var gameInit = GameInit.FromJson(gameInitForBlue);

            strategyData.ChanceAtFixedStartingPosition = 100;

            var result = strategy.initialize(gameInit);

            result.Pieces.Should().Contain(p => p.Rank == rank && p.Position == new Point(x, y).Transpose());
        }

        [Fact]
        public void Process_returns_null_if_not_your_turn()
        {
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData)
            {
                MyColor = Player.Red,
            };
            var state = new GameState { ActivePlayer = Player.Blue };
            strategy.initialize(GameInit.FromJson(gameInitForRed));

            var result = strategy.Process(state);

            result.Should().BeNull();
        }

        [Fact]
        public void Process_returns_move_smoke_test_001()
        {
            var strategyData = new StrategyData().WithDefaults();
            var strategy = new Strategy(new Random(123), strategyData)
            {
                MyColor = Player.Blue,
            };

            strategy.initialize(GameInit.FromJson(gameInitForBlue));

            var state = GameState.FromJson(""""
            {
                "ActivePlayer": 1,
                "Board": [
                    { "Rank": "Scout", "Owner": 0, "Coordinate": { X: 0, Y: 0 } },
                    { "Rank": "?", "Owner": 0, "Coordinate": { X: 1, Y: 1 } },
                    { "Rank": "Scout", "Owner": 1, "Coordinate": { X: 0, Y: 1 } }
                ]
            }
            """");

            var result = strategy.Process(state);

            result.Should()
                .BeAssignableTo<Move>()
                .And.Match<Move>(m => m.From.X == 0)
                .And.Match<Move>(m => m.From.Y == 1)
                .And.Match<Move>(m => m.To.X != 0 || m.To.Y != 1);
        }
    }

    public static class StrategyDataExtensions
    {
        private static readonly string[] allRanks =
        {
            "Scout", "Scout", "Bomb", "Flag", "Miner", "Marshal", "Spy", "General"
        };

        internal static readonly IList<(string, Point)> DefaultFixedStartingPositions = new List<(string, Point)>()
        {
            ("Flag", new Point(0, 0)),
            ("Bomb", new Point(1, 0)),
            ("Spy", new Point(2, 1)),
            ("Scout", new Point(3, 1)),
            ("Scout", new Point(4, 2)),
            ("Miner", new Point(5, 2)),
            ("General", new Point(6, 3)),
            ("Marshal", new Point(7, 3)),
        };

        public static StrategyData WithDefaults(this StrategyData data)
        {
            data.StartPositionGrids = allRanks.Select(rank =>
            {
                var grid = new StartPositionGrid { Rank = rank };
                for (int col = 0; col < 10; col++)
                {
                    grid.Probabilities.Add(new Point(col, 0), 100);
                }
                return grid;
            }
            ).ToList();

            data.FixedStartGrids.Clear();
            data.FixedStartGrids.Add(new FixedStartGrid
            {
                StartingPositions = DefaultFixedStartingPositions
            });

            return data;
        }
    }
}