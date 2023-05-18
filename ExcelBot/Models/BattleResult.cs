namespace ExcelBot.Models
{
    public class BattleResult
    {
        public Player? Winner { get; set; }
        public Point Position { get; set; }
        public RankAndPlayer Attacker { get; set; } = new RankAndPlayer();
        public RankAndPlayer Defender { get; set; } = new RankAndPlayer();
    }

}
