using ExcelBot.Runtime.Models;
using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public class FixedStartGrid
    {
        public ICollection<(string, Point)> StartingPositions { get; set; } = new List<(string, Point)>();

        // TODO: Unit tests
        public void Transpose()
        {
            var newStartingPositions = new List<(string, Point)>();
            foreach (var (rank, point) in StartingPositions)
            {
                newStartingPositions.Add((rank, point.Transpose()));
            }
            StartingPositions = newStartingPositions;
        }
    }
}
