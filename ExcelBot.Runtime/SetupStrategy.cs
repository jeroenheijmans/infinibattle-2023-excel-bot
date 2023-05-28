using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelBot.Runtime
{
    public class SetupStrategy
    {
        private readonly StrategyData strategyData;
        private readonly Random random;

        public SetupStrategy(StrategyData strategyData, Random random)
        {
            this.strategyData = strategyData;
            this.random = random;
        }

        public Piece[] GetPieces()
        {
            return random.Next(100) < strategyData.ChanceAtFixedStartingPosition
                ? this.FromFixedPosition()
                : this.WithProbabilities();
        }

        private Piece[] FromFixedPosition()
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
                .ToArray();
        }

        private Piece[] WithProbabilities()
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

            return pieces.ToArray();
        }
    }
}
