using ExcelBot.Runtime;
using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;

namespace ExcelBot.Tests
{
    public class MoveOptionTests
    {
        private Random random = new Random(123);
        private StrategyData strategyData = new StrategyData();

        [Fact]
        public void CalculateScore_can_add_decisive_victory_score()
        {
            var moveOption = new MoveOption { WillBeDecisiveVictory = true };
            strategyData.DecisiveVictoryPoints = 100;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(100);
        }

        [Fact]
        public void CalculateScore_can_add_decisive_loss_score()
        {
            var moveOption = new MoveOption { WillBeDecisiveLoss = true };
            strategyData.DecisiveLossPoints = -100;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-100);
        }

        [Fact]
        public void CalculateScore_can_add_unknown_own_half_battle_score()
        {
            var moveOption = new MoveOption { WillBeUnknownBattle = true, IsBattleOnOwnHalf = true };
            strategyData.UnknownBattleOwnHalfPoints = -200;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-200);
        }

        [Fact]
        public void CalculateScore_can_add_unknown_opponent_half_battle_score()
        {
            var moveOption = new MoveOption { WillBeUnknownBattle = true, IsBattleOnOpponentHalf = true };
            strategyData.UnknownBattleOpponentHalfPoints = -300;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-300);
        }

        [Fact]
        public void CalculateScore_can_add_move_towards_opponent_score()
        {
            var moveOption = new MoveOption { IsMoveTowardsOpponentHalf = true };
            strategyData.BonusPointsForMoveTowardsOpponent = 300;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(300);
        }

        [Fact]
        public void CalculateScore_can_add_move_within_opponent_score()
        {
            var moveOption = new MoveOption { IsMoveWithinOpponentHalf = true };
            strategyData.BonusPointsForMoveWithinOpponentArea = 400;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(400);
        }

        [Fact]
        public void CalculateScore_can_add_move_for_first_time_score()
        {
            var moveOption = new MoveOption { IsMovingForFirstTime = true };
            strategyData.BonusPointsForMovingPieceForTheFirstTime = -400;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-400);
        }

        [Fact]
        public void CalculateScore_can_add_move_unrevealed_piece_score()
        {
            var moveOption = new MoveOption { IsMoveForUnrevealedPiece = true };
            strategyData.BonusPointsForMovingUnrevealedPiece = -500;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-500);
        }

        [Theory]
        [InlineData(1, 750)]
        [InlineData(2, 1500)]
        [InlineData(6, 4500)]
        public void CalculateScore_can_add_distance_bonus_for_steps_with_scout_bonus(int steps, double expected)
        {
            var moveOption = new MoveOption
            {
                Steps = steps,
                NetChangeInManhattanDistanceToPotentialFlag = -1 * steps,
            };
            strategyData.BonusPointsForMovesGettingCloserToPotentialFlags = 750;
            strategyData.ScoutJumpsToPotentialFlagsMultiplication = true;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, 750)]
        [InlineData(2, 750)]
        [InlineData(6, 750)]
        public void CalculateScore_can_add_distance_bonus_for_steps_without_scout_bonus(int steps, double expected)
        {
            var moveOption = new MoveOption
            {
                Steps = steps,
                NetChangeInManhattanDistanceToPotentialFlag = -1 * steps,
            };
            strategyData.BonusPointsForMovesGettingCloserToPotentialFlags = 750;
            strategyData.ScoutJumpsToPotentialFlagsMultiplication = false;
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(expected);
        }

        [Theory]
        [InlineData("Spy")]
        [InlineData("Scout")]
        [InlineData("Miner")]
        [InlineData("General")]
        [InlineData("Marshal")]
        public void CalculateScore_can_boost_positive_score_for(string rank)
        {
            var moveOption = new MoveOption { Rank = rank, WillBeDecisiveVictory = true };
            strategyData.DecisiveVictoryPoints = 100;
            SetStrategyDataBoostForRank(rank, 25);
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(125);
        }

        [Theory]
        [InlineData("Spy")]
        [InlineData("Scout")]
        [InlineData("Miner")]
        [InlineData("General")]
        [InlineData("Marshal")]
        public void CalculateScore_can_boost_negative_score_for(string rank)
        {
            var moveOption = new MoveOption { Rank = rank, WillBeDecisiveLoss = true };
            strategyData.DecisiveLossPoints = -100;
            SetStrategyDataBoostForRank(rank, 25);
            moveOption.CalculateScore(strategyData, random);
            moveOption.Score.Should().Be(-75);
        }

        [Fact]
        public void CalculateScore_can_add_fuzzyness_multiplier_of_zero()
        {
            var moveOption = new MoveOption { WillBeDecisiveVictory = true };
            strategyData.DecisiveVictoryPoints = 100;
            strategyData.FuzzynessFactor = 0;
            moveOption.CalculateScore(strategyData, new Random(123));
            moveOption.Score.Should().Be(100);
        }

        [Fact]
        public void CalculateScore_can_add_fuzzyness_multiplier_of_nonzero()
        {
            var moveOption = new MoveOption { WillBeDecisiveVictory = true };
            strategyData.DecisiveVictoryPoints = 100;
            strategyData.FuzzynessFactor = 5;
            moveOption.CalculateScore(strategyData, new Random(123));
            moveOption.Score.Should().Be(104);
        }

        [Fact]
        public void ToMove_has_from_and_to()
        {
            var moveOption = new MoveOption { From = new Point(1, 2), To = new Point(3, 4) };
            var result = moveOption.ToMove();
            result.Should().NotBeNull();
            result.From.Should().Be(new Point(1, 2));
            result.To.Should().Be(new Point(3, 4));
        }

        private void SetStrategyDataBoostForRank(string rank, int boost)
        {
            if (rank == "Spy") strategyData.BoostForSpy = boost;
            else if (rank == "Scout") strategyData.BoostForScout = boost;
            else if (rank == "Miner") strategyData.BoostForMiner = boost;
            else if (rank == "General") strategyData.BoostForGeneral = boost;
            else if (rank == "Marshal") strategyData.BoostForMarshal = boost;
        }
    }
}
