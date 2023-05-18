namespace ExcelBot.Models
{
    public class GameState
    {
        public Player ActivePlayer { get; set; }
        public int TurnNumber { get; set; }
        public Cell[] Board { get; set; } = Array.Empty<Cell>();
        public Move LastMove { get; set; } = new Move();
        public BattleResult BattleResult { get; set; } = new BattleResult();
    }

}
