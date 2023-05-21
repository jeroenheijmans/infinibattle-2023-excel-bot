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
                        WillBeDecisiveVictory = targetCell.IsKnownPiece && targetCell.CanBeDefeatedBy(origin.Rank),
                        WillBeDecisiveLoss = targetCell.IsKnownPiece && targetCell.WillCauseDefeatFor(origin.Rank),
                        WillBeUnknownBattle = targetCell.IsUnknownPiece,
                        IsBattleOnOwnHalf = targetCell.IsPiece && targetCell.IsOnOwnHalf(MyColor),
                        IsBattleOnOpponentHalf = targetCell.IsPiece && targetCell.IsOnOpponentHalf(MyColor),
                        IsMoveTowardsOpponentHalf = IsMoveTowardsOpponentHalf(origin.Coordinate, target),
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
                ? to.Y > 5 || to.Y > from.Y
                : to.Y < 4 || to.Y < from.Y;

        private void SetScoreForMove(MoveWithDetails move)
        {
            if (move.WillBeDecisiveVictory) move.Score += 100;
            if (move.WillBeDecisiveLoss) move.Score += -10000;
            if (move.WillBeUnknownBattle && move.IsBattleOnOwnHalf) move.Score += -50;
            if (move.WillBeUnknownBattle && move.IsBattleOnOpponentHalf) move.Score += +250;
            if (move.IsMoveTowardsOpponentHalf) move.Score += 25;

            double fuzzynessMultiplier = random.Next(strategyData.FuzzynessFactor) + 100;

            move.Score *= fuzzynessMultiplier / 100;
        }

        private void ProcessOpponentMove(GameState state)
        {
            // NO-OP for now, up to you to do something nice...
        }
    }
}
