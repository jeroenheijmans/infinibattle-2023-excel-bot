using ExcelBot.Models;

namespace ExcelBot
{
    public class Strategy
    {
        public Player MyColor { get; set; }

        public BoardSetup initialize(GameInit data)
        {
            MyColor = data.You;
            var row = MyColor == Player.Red ? 0 : 6;
            return new BoardSetup
            {
                Pieces = data.AvailablePieces
                    .OrderBy(_ => Guid.NewGuid()) // Quick and dirty Shuffle()
                    .Select((rank, idx) => new Piece { Rank = rank, Position = new Point(idx + 1, row + Random.Shared.Next(3)) })
                    .ToArray()
            };
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

        public Move DecideNextMove(GameState state)
        {
            return state.Board
                .Where(c => c.Owner == MyColor) // only my pieces can be moved
                .SelectMany(c => GetPossibleMovesFor(c, state)) // all options from all starting points
                .OrderBy(_ => Guid.NewGuid()) // Quick and dirty Shuffle()
                .First();
        }

        private IEnumerable<Move> GetPossibleMovesFor(Cell origin, GameState state)
        {
            if (origin.Rank == "Flag" || origin.Rank == "Bomb") return Enumerable.Empty<Move>();

            var deltas = new Point[] { new Point(-1, 0), new Point(+1, 0), new Point(0, -1), new Point(0, +1) };

            return deltas.SelectMany(delta =>
            {
                var result = new List<Move>();
                var steps = 0;
                var target = origin.Coordinate;
                while (steps++ < 1 || origin.Rank == "Scout")
                {
                    target = target + delta;
                    var targetCell = state.Board.FirstOrDefault(c => c.Coordinate == target);
                    if (targetCell == null) break; // Out of bounds
                    if (targetCell.IsWater) break; // Water ends our options
                    if (targetCell.Owner == MyColor) break; // Own pieces block the path

                    result.Add(new Move { From = origin.Coordinate, To = target });

                    if (targetCell.Owner != null) break; // Can't jump over pieces, so this stops the line
                }
                return result;
            });
        }

        public void ProcessOpponentMove(GameState state)
        {
            // NO-OP for now, up to you to do something nice...
        }
    }
}
