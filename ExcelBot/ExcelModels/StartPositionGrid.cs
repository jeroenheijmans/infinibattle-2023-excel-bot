using ExcelBot.Models;
using GemBox.Spreadsheet;

namespace ExcelBot.ExcelModels
{
    public class StartPositionGrid
    {
        public string Rank { get; set; } = "";
        public IDictionary<Point, int> Probabilities { get; set; } = new Dictionary<Point, int>();
    }

    public class ExcelStrategy
    {
        public ICollection<StartPositionGrid> StartPositionGrids { get; set; } = new List<StartPositionGrid>();

        // TODO: Fixed Starting Positions
        // TODO: Strategy Variables
    }

    public static class ExcelLoader
    {
        private static readonly (int, int, string)[] GridLocations = new[]
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

        public static ExcelStrategy Load(string path)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            ExcelStrategy excelStrategy = new();

            var workbook = ExcelFile.Load(path);
            var sheet = workbook.Worksheets[0];

            foreach (var (fromRow, fromCol, rank) in GridLocations)
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

            return excelStrategy;
        }
    }
}
