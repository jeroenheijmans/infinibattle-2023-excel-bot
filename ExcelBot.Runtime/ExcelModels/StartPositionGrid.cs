using ExcelBot.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelBot.Runtime.ExcelModels
{
    public class StartPositionGrid
    {
        public string Rank { get; set; } = "";
        public IDictionary<Point, int> Probabilities { get; set; } = new Dictionary<Point, int>();

        // TODO: Unit tests
        public Piece PickStartingPosition(int randomInt)
        {
            var total = Probabilities.Values.Sum();
            var choice = randomInt % total;
            var current = 0;
            foreach (var (position, probability) in Probabilities)
            {
                current += probability;
                if (current >= choice) return new Piece
                {
                    Position = position,
                    Rank = Rank
                };
            }
            throw new Exception("Mistake in the algorithm");
            // TODO: IF RELEASE then pick anything to not crash
        }

        // TODO: Unit tests
        public void Transpose()
        {
            var newProbabilities = new Dictionary<Point, int>();
            foreach (var (point, value) in Probabilities)
            {
                newProbabilities[point.Transpose()] = value;
            }
            Probabilities = newProbabilities;
        }
    }
}
