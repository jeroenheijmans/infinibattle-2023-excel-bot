using ExcelBot.Runtime.Models;
using GemBox.Spreadsheet;
using System.Collections.Generic;

namespace ExcelBot.Runtime.ExcelModels
{
    public static class ExcelLoader
    {
        private static readonly (int, int, string)[] ProbabilitiesGridLocations = new[]
        {
            ( 2,  2, "Flag"),
            (14,  2, "Bomb"),
            ( 2, 13, "Scout"),
            (14, 13, "Miner"),
            ( 2, 24, "Scout"),
            (14, 24, "Marshal"),
            ( 2, 35, "General"),
            (14, 35, "Spy"),
        };

        private static readonly (int, int)[] FixedStartGridLocations = new[]
        {
            (27,  2),
            (27, 13),
            (27, 24),
        };

        private const int OpponentFlagProbabilitiesRow = 27;
        private const int OpponentFlagProbabilitiesCol = 35;

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

            // Load expected flag location probabilities
            for (int i = 0; i < 10; i++)
            {
                for (int j = 6; j < 10; j++)
                {
                    var cell = sheet.Cells[OpponentFlagProbabilitiesRow + j, OpponentFlagProbabilitiesCol + i];
                    var probability = cell.ValueType == CellValueType.Int ? cell.IntValue : 0;
                    var pos = new Point(i, j);
                    excelStrategy.OpponentFlagProbabilities.Add(pos, probability);
                }
            }

            // Load strategy variables
            int GetChanceValue(string reference)
            {
                var cell = sheet.Cells[reference];
                return cell.ValueType == CellValueType.Int ? cell.IntValue : 0;
            }

            bool GetBoolValue(string reference)
            {
                var cell = sheet.Cells[reference];
                return cell.ValueType == CellValueType.Bool ? cell.BoolValue : false;
            }

            excelStrategy.ChanceAtFixedStartingPosition = GetChanceValue("AV3");

            excelStrategy.FuzzynessFactor = GetChanceValue("AV9");

            excelStrategy.DecisiveVictoryPoints = GetChanceValue("AV10");
            excelStrategy.DecisiveLossPoints = GetChanceValue("AV11");
            excelStrategy.UnknownBattleOwnHalfPoints = GetChanceValue("AV12");
            excelStrategy.UnknownBattleOpponentHalfPoints = GetChanceValue("AV13");
            excelStrategy.BonusPointsForMoveTowardsOpponent = GetChanceValue("AV14");
            excelStrategy.BonusPointsForMoveWithinOpponentArea = GetChanceValue("AV15");
            excelStrategy.BonusPointsForMovesGettingCloserToPotentialFlags = GetChanceValue("AV16");
            excelStrategy.ScoutJumpsToPotentialFlagsMultiplication = GetBoolValue("AV17");
            excelStrategy.BonusPointsForMovingPieceForTheFirstTime = GetChanceValue("AV18");
            excelStrategy.BonusPointsForMovingUnrevealedPiece = GetChanceValue("AV19");

            excelStrategy.BoostForSpy = GetChanceValue("AV28");
            excelStrategy.BoostForScout = GetChanceValue("AV29");
            excelStrategy.BoostForMiner = GetChanceValue("AV30");
            excelStrategy.BoostForGeneral = GetChanceValue("AV31");
            excelStrategy.BoostForMarshal = GetChanceValue("AV32");

            return excelStrategy;
        }
    }
}
