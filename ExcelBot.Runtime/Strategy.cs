using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using ExcelBot.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelBot.Runtime
{
    public class Strategy
    {
        private bool hasInitialized = false;
        private readonly Random random;
        private readonly StrategyData strategyData;
        private readonly SetupStrategy setupStrategy;
        private readonly ISet<Point> possibleFlagCoordinates = new HashSet<Point>();
        private readonly ISet<Point> unmovedOwnPieceCoordinates = new HashSet<Point>();
        private readonly ISet<Point> unrevealedOwnPieceCoordinates = new HashSet<Point>();

        public Strategy(Random random, StrategyData strategyData)
        {
            this.random = random;
            this.strategyData = strategyData;
            this.setupStrategy = new SetupStrategy(strategyData, random);
        }

        public Player MyColor { get; set; }
        public Player OpponentColor { get; set; }

        public BoardSetup Initialize(GameInit data)
        {
            MyColor = data.You;
            OpponentColor = MyColor.Opponent();

            OpponentColor.GetAllHomeCoordinates().ForEach(c => possibleFlagCoordinates.Add(c));

            if (MyColor == Player.Blue) strategyData.TransposeAll();

            var pieces = setupStrategy.GetPieces();

            pieces.ForEach(p => unmovedOwnPieceCoordinates.Add(p.Position));
            pieces.ForEach(p => unrevealedOwnPieceCoordinates.Add(p.Position));

            hasInitialized = true;

            return new BoardSetup { Pieces = pieces };
        }

        public Move? Process(GameState state)
        {
            if (!hasInitialized)
            {
                throw new InvalidOperationException("Processing move before initialization is not possible");
            }

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
            var move = state.Board
                .Where(c => c.Owner == MyColor)
                .SelectMany(c => GetPossibleMovesFor(c, state))
                .OrderByDescending(move => move.Score)
                .First();

            unmovedOwnPieceCoordinates.Remove(move.From);

            if (unrevealedOwnPieceCoordinates.Contains(move.From))
            {
                unrevealedOwnPieceCoordinates.Remove(move.From);
                if (!state.Board.First(c => c.Coordinate == move.To).IsOpponentPiece(MyColor))
                {
                    unrevealedOwnPieceCoordinates.Add(move.To);
                }
            }

            return move;
        }

        private IEnumerable<MoveWithDetails> GetPossibleMovesFor(Cell origin, GameState state)
        {
            if (origin.Rank == "Flag" || origin.Rank == "Bomb")
            {
                return Enumerable.Empty<MoveWithDetails>();
            }

            var deltas = new Point[]
            {
                new Point(-1, 0),
                new Point(+1, 0),
                new Point(0, -1),
                new Point(0, +1),
            };

            return deltas.SelectMany(delta =>
            {
                var result = new List<MoveWithDetails>();
                var steps = 0;
                var target = origin.Coordinate;

                while (steps++ < 1 || origin.Rank == "Scout")
                {
                    target += delta;
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
                        IsBattleOnOwnHalf = targetCell.IsPiece && target.IsOnOwnHalfFor(MyColor),
                        IsBattleOnOpponentHalf = targetCell.IsPiece && target.IsOnOpponentHalfFor(MyColor),
                        IsMoveTowardsOpponentHalf = IsMoveTowardsOpponentHalf(origin.Coordinate, target),
                        IsMoveWithinOpponentHalf = IsMoveWithinOpponentHalf(origin.Coordinate, target),
                        IsMovingForFirstTime = unmovedOwnPieceCoordinates.Contains(origin.Coordinate),
                        IsMoveForUnrevealedPiece = unrevealedOwnPieceCoordinates.Contains(origin.Coordinate),
                        NetChangeInManhattanDistanceToPotentialFlag =
                            GetSmallestManhattanDistanceToPotentialFlag(state, target)
                            - GetSmallestManhattanDistanceToPotentialFlag(state, origin.Coordinate),
                        Steps = steps,
                    };

                    move.CalculateScore(strategyData, random);

                    result.Add(move);

                    // Can't jump over pieces, so once we see one we stop stepping
                    if (targetCell.Owner != null) break;
                }

                return result;
            });
        }

        private double GetSmallestManhattanDistanceToPotentialFlag(GameState state, Point source)
        {
            return state.Board
                .Where(cell => possibleFlagCoordinates.Contains(cell.Coordinate))
                .Select(cell =>
                {
                    double dist = source.DistanceTo(cell.Coordinate);

                    // We'll dislike moving into "water" columns on our own half
                    // because that might block us on a path to the other's flag.
                    // As a shortcut/hack we fake an increase in Manhattan Distance
                    // to avoid scoring this move well.
                    if (cell.Coordinate.IsOnOwnHalfFor(MyColor) && cell.Coordinate.IsWaterColumn())
                    {
                        dist += 3;
                    }

                    // The Excel sheet has probabilities for where the flag may be,
                    // and we translate this into a virtual decrease in "distance"
                    // by turning high probabilities into lower return values.
                    if (strategyData.OpponentFlagProbabilities.ContainsKey(cell.Coordinate))
                    {
                        int probability = strategyData.OpponentFlagProbabilities[cell.Coordinate];
                        double divider = (100 + probability) / 100;
                        dist /= divider;
                    }

                    return dist;
                })
                .Min();
        }

        private bool IsMoveTowardsOpponentHalf(Point from, Point to) =>
            MyColor == Player.Red
                ? from.Y < 6 && to.Y > from.Y
                : from.Y > 3 && to.Y < from.Y;

        private bool IsMoveWithinOpponentHalf(Point from, Point to) =>
            MyColor == Player.Red
                ? from.Y > 5 && to.Y > 5
                : from.Y < 4 && to.Y < 4;

        private void ProcessOpponentMove(GameState state)
        {
            state.Board
                .Where(c => !c.IsPiece || !c.IsUnknownPiece || !c.Coordinate.IsOnOpponentHalfFor(MyColor))
                .ForEach(c => possibleFlagCoordinates.Remove(c.Coordinate));

            if (state.LastMove != null)
            {
                possibleFlagCoordinates.Remove(state.LastMove.To);
                possibleFlagCoordinates.Remove(state.LastMove.From);
                unrevealedOwnPieceCoordinates.Remove(state.LastMove.To);
            }
        }
    }
}
