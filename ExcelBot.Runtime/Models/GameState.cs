using Newtonsoft.Json;
using System;

namespace ExcelBot.Runtime.Models
{
    public class GameState
    {
        public Player ActivePlayer { get; set; }
        public int TurnNumber { get; set; }
        public Cell[] Board { get; set; } = Array.Empty<Cell>();
        public Move LastMove { get; set; } = new Move();
        public BattleResult BattleResult { get; set; } = new BattleResult();

        public static GameState FromJson(string json) =>
            JsonConvert.DeserializeObject<GameState>(json)
            ?? throw new Exception("GameState deserialization error");
    }
}
