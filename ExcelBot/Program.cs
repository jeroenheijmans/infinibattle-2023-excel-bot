using Newtonsoft.Json;

Console.WriteLine("bot-start");

var initData = Console.ReadLine() ?? throw new Exception("No GameInit");
var gameInit = JsonConvert.DeserializeObject<GameInit>(initData) ?? throw new Exception("GameInit deserialization error");
var strategy = new Strategy();
var boardSetup = strategy.initialize(gameInit);
var setupMessage = JsonConvert.SerializeObject(boardSetup);

Console.WriteLine(setupMessage);

while (true)
{
    var stateData = Console.ReadLine() ?? throw new Exception("No GameState");
    var gameState = JsonConvert.DeserializeObject<GameState>(stateData) ?? throw new Exception("GameState deserialization error");
    var move = strategy.Process(gameState);
    if (move != null)
    {
        var moveMessage = JsonConvert.SerializeObject(move);
        Console.WriteLine(moveMessage);
    }
}

// ---------------------------------------------------
// ---------------------------------------------------

public enum Player
{
    Red = 0,
    Blue = 1,
}

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
            .OrderByDescending(c => c.Score) // quick and dirty strategy
            .First();
    }

    private IEnumerable<MoveWithScore> GetPossibleMovesFor(Cell origin, GameState state)
    {
        if (origin.Rank == "Flag" || origin.Rank == "Bomb") return Enumerable.Empty<MoveWithScore>();

        var deltas = new Point[] { new Point(-1, 0), new Point(+1, 0), new Point(0, -1), new Point(0, +1) };

        return deltas.SelectMany(delta =>
        {
            var result = new List<MoveWithScore>();
            var steps = 0;
            var target = origin.Coordinate;
            while (steps++ < 1 || origin.Rank == "Scout")
            {
                target = target + delta;
                var targetCell = state.Board.FirstOrDefault(c => c.Coordinate == target);
                if (targetCell == null) break; // Out of bounds
                if (targetCell.IsWater) break; // Water ends our options
                if (targetCell.Owner == MyColor) break; // Own pieces block the path

                var entropyChange = state.Board
                    .Where(c => c.Owner.HasValue && c.Owner.Value != MyColor)
                    .Select(c => c.Coordinate.DistanceTo(origin.Coordinate) - c.Coordinate.DistanceTo(target))
                    .Sum();

                result.Add(new MoveWithScore { From = origin.Coordinate, To = target, Score = entropyChange });
                
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

// ---------------------------------------------------
// ---------------------------------------------------

public class GameInit
{
    public Player You { get; set; } = Player.Red;
    public string[] AvailablePieces { get; set; } = Array.Empty<string>();
}

public class GameState
{
    public Player ActivePlayer { get; set; }
    public int TurnNumber { get; set; }
    public Cell[] Board { get; set; } = Array.Empty<Cell>();
    public Move LastMove { get; set; } = new Move();
    public BattleResult BattleResult { get; set; } = new BattleResult();
}

public class BoardSetup
{
    public Piece[] Pieces { get; set; } = Array.Empty<Piece>();
}

public class Piece
{
    public string Rank { get; set; } = "";
    public Point Position { get; set; }
}

public class Cell
{
    public string Rank { get; set; } = "";
    public Player? Owner { get; set; } = null;
    public bool IsWater { get; set; }
    public Point Coordinate { get; set; }
}

public class Move
{
    public Point From { get; set; }
    public Point To { get; set; }
}

public class MoveWithScore : Move
{
    public int Score { get; set; }
}

public class BattleResult
{
    public Player? Winner { get; set; }
    public Point Position { get; set; }
    public RankAndPlayer Attacker { get; set; } = new RankAndPlayer();
    public RankAndPlayer Defender { get; set; } = new RankAndPlayer();
}

public class RankAndPlayer
{
    public string Rank { get; set; } = "";
    public Player Player { get; set; }
}

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y) { X = x; Y = y; }

    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Point a, Point b) => !(a == b);
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public bool Equals(Point other) => this == other;
    public override bool Equals(object? other) => other is Point && this.Equals((Point)other);
    public int DistanceTo(Point other) => Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
}
