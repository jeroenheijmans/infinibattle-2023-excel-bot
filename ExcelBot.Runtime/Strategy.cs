using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelBot.Runtime
{
    public class Strategy
    {
        private readonly Random random;
        private readonly StrategyData strategyData;

        public Strategy(Random random, StrategyData strategyData)
        {
            this.random = random;
            this.strategyData = strategyData;
        }

        public Player MyColor { get; set; }

        public BoardSetup initialize(GameInit data)
        {
            MyColor = data.You;

            if (MyColor == Player.Blue) strategyData.TransposeAll();

            var pieces = random.Next(100) < strategyData.ChanceAtFixedStartingPosition
                ? SetupBoardFromFixedPosition()
                : SetupBoardWithProbabilities();

            return new BoardSetup { Pieces = pieces.ToArray() };
        }

        private IEnumerable<Piece> SetupBoardFromFixedPosition()
        {
            return strategyData.FixedStartGrids
                .OrderBy(_ => Guid.NewGuid()) // quick and dirty shuffle
                .First()
                .StartingPositions
                .Select(tuple => new Piece
                {
                    Rank = tuple.Item1,
                    Position = tuple.Item2
                })
                .ToList();
        }

        private IEnumerable<Piece> SetupBoardWithProbabilities()
        {
            var pieces = new List<Piece>();

            foreach (var grid in strategyData.StartPositionGrids)
            {
                var maxIterations = 10000;
                for (int i = 0; i < maxIterations; i++)
                {
                    var piece = grid.PickStartingPosition(random.Next());
                    if (pieces.Any(x => x.Position == piece.Position)) continue;
                    pieces.Add(piece);
                    break;
                }
            }

            return pieces;
        }

        public Move? Process(GameState state)
        {
            if (state.ActivePlayer == MyColor)
            {
                return DecideNextMove(state);
            }
            else
            {
                ProcessOpponentMove(state);
                return null;
            }
        }

        private Move DecideNextMove(GameState state)
        {
            return state.Board
                .Where(c => c.Owner == MyColor) // only my pieces can be moved
                .SelectMany(c => GetPossibleMovesFor(c, state)) // all options from all starting points
                .OrderByDescending(move => move.Score)
                .First();
        }

        private IEnumerable<MoveWithDetails> GetPossibleMovesFor(Cell origin, GameState state)
        {
            if (origin.Rank == "Flag" || origin.Rank == "Bomb") return Enumerable.Empty<MoveWithDetails>();

            var deltas = new Point[] { new Point(-1, 0), new Point(+1, 0), new Point(0, -1), new Point(0, +1) };

            return deltas.SelectMany(delta =>
            {
                var result = new List<MoveWithDetails>();
                var steps = 0;
                var target = origin.Coordinate;
                while (steps++ < 1 || origin.Rank == "Scout")
                {
                    target = target + delta;
                    var targetCell = state.Board.FirstOrDefault(c => c.Coordinate == target);
                    if (targetCell == null) break; // Out of bounds
                    if (targetCell.IsWater) break; // Water ends our options
                    if (targetCell.Owner == MyColor) break; // Own pieces block the path

                    var move = new MoveWithDetails
                    {
                        From = origin.Coordinate,
                        To = target,
                        Rank = origin.Rank,
                        WillBeDecisiveVictory = targetCell.IsKnownPiece && targetCell.CanBeDefeatedBy(origin.Rank),
                        WillBeDecisiveLoss = targetCell.IsKnownPiece && targetCell.WillCauseDefeatFor(origin.Rank),
                        WillBeUnknownBattle = targetCell.IsUnknownPiece,
                        IsBattleOnOwnHalf = targetCell.IsPiece && targetCell.IsOnOwnHalf(MyColor),
                        IsBattleOnOpponentHalf = targetCell.IsPiece && targetCell.IsOnOpponentHalf(MyColor),
                        IsMoveTowardsOpponentHalf = IsMoveTowardsOpponentHalf(origin.Coordinate, target),
                        IsMoveWithinOpponentHalf = IsMoveWithinOpponentHalf(origin.Coordinate, target),
                    };

                    SetScoreForMove(move);

                    result.Add(move);

                    if (targetCell.Owner != null) break; // Can't jump over pieces, so this stops the line
                }
                return result;
            });
        }

        private bool IsMoveTowardsOpponentHalf(Point from, Point to) =>
            MyColor == Player.Red
                ? from.Y < 6 && to.Y > from.Y
                : from.Y > 3 && to.Y < from.Y;

        private bool IsMoveWithinOpponentHalf(Point from, Point to) =>
            MyColor == Player.Red
                ? from.Y > 5 && to.Y > 5
                : from.Y < 4 && to.Y < 4;

        private void SetScoreForMove(MoveWithDetails move)
        {
            if (move.WillBeDecisiveVictory) move.Score += strategyData.DecisiveVictoryPoints;
            if (move.WillBeDecisiveLoss) move.Score += strategyData.DecisiveLossPoints;
            if (move.WillBeUnknownBattle && move.IsBattleOnOwnHalf) move.Score += strategyData.UnknownBattleOwnHalfPoints;
            if (move.WillBeUnknownBattle && move.IsBattleOnOpponentHalf) move.Score += strategyData.UnknownBattleOpponentHalfPoints;
            if (move.IsMoveTowardsOpponentHalf) move.Score += strategyData.BonusPointsForMoveTowardsOpponent;
            if (move.IsMoveWithinOpponentHalf) move.Score += strategyData.BonusPointsForMoveWithinOpponentArea;

            var boost = 0;
            if (move.Rank == "Spy") boost = strategyData.BoostForSpy;
            if (move.Rank == "Scout") boost = strategyData.BoostForScout;
            if (move.Rank == "Miner") boost = strategyData.BoostForMiner;
            if (move.Rank == "General") boost = strategyData.BoostForGeneral;
            if (move.Rank == "Marshal") boost = strategyData.BoostForMarshal;

            double boostMultiplier = boost + 100;
            move.Score *= boostMultiplier / 100;

            double fuzzynessMultiplier = random.Next(strategyData.FuzzynessFactor) + 100;
            move.Score *= fuzzynessMultiplier / 100;
        }

        private void ProcessOpponentMove(GameState state)
        {
            // NO-OP for now, up to you to do something nice...
        }
    }
}
