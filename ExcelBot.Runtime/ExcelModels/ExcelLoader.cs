using ExcelBot.Runtime.Models;
using GemBox.Spreadsheet;
using System.Collections;
using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public static class ExcelLoader
    {
        private static readonly (int, int, string)[] ProbabilitiesGridLocations = new[]
        {
            ( 2,  1, "Flag"),
            (14,  1, "Bomb"),
            ( 2, 12, "Scout"),
            (14, 12, "Miner"),
            ( 2, 23, "Scout"),
            (14, 23, "Marshal"),
            ( 2, 34, "General"),
            (14, 34, "Spy"),
        };

        private static readonly (int, int)[] FixedStartGridLocations = new[]
        {
            (26,  1),
            (26, 12),
            (26, 23),
            (26, 34),
        };

        private static readonly IDictionary<string, string> RankShorthands = new Dictionary<string, string>
        {
            { "FL", "Flag" },
            { "BO", "Bomb" },
            { "SP", "Spy" },
            { "SC", "Scout" },
            { "MI", "Miner" },
            { "GE", "General" },
            { "MA", "Marshal" },
        };

        public static StrategyData Load(string path)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            StrategyData excelStrategy = new();

            var workbook = ExcelFile.Load(path);
            var sheet = workbook.Worksheets[0];

            // Load probabilities
            foreach (var (fromRow, fromCol, rank) in ProbabilitiesGridLocations)
            {
                var grid = new StartPositionGrid { Rank = rank };
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var cell = sheet.Cells[fromRow + j, fromCol + i];
                        grid.Probabilities[new Point(i, j)] =
                            cell.ValueType == CellValueType.Int
                            ? cell.IntValue
                            : 0;
                    }
                }

                excelStrategy.StartPositionGrids.Add(grid);
            }

            // Load fixed positions
            foreach (var (fromRow, fromCol) in FixedStartGridLocations)
            {
                var grid = new FixedStartGrid();
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var cell = sheet.Cells[fromRow + j, fromCol + i];
                        if (cell.ValueType == CellValueType.String)
                        {
                            var key = cell.StringValue;
                            var pos = new Point(i, j);
                            var rank = RankShorthands[key];
                            grid.StartingPositions.Add((rank, pos));
                        }
                    }
                }

                if (grid.StartingPositions.Count == 8)
                {
                    excelStrategy.FixedStartGrids.Add(grid);
                }
            }

            return excelStrategy;
        }
    }
}
