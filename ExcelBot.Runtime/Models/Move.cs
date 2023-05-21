using Newtonsoft.Json;

namespace ExcelBot.Runtime.Models
{
    public class Move
    {
        public Point From { get; set; }
        public Point To { get; set; }
    }

    public class MoveWithDetails : Move
    {
        [JsonIgnore] public string Rank { get; set; } = "";

        [JsonIgnore] public bool WillBeDecisiveVictory { get; set; }
        [JsonIgnore] public bool WillBeDecisiveLoss { get; set; }
        [JsonIgnore] public bool WillBeUnknownBattle { get; set; }
        [JsonIgnore] public bool IsBattleOnOpponentHalf { get; set; }
        [JsonIgnore] public bool IsBattleOnOwnHalf { get; set; }
        [JsonIgnore] public bool IsMoveTowardsOpponentHalf { get; set; }
        [JsonIgnore] public bool IsMoveWithinOpponentHalf { get; set; }

        [JsonIgnore] public double Score { get; set; }
    }
}
