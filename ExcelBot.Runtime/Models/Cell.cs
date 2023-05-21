namespace ExcelBot.Runtime.Models
{
    public class Cell
    {
        public string Rank { get; set; } = "";
        public Player? Owner { get; set; } = null;
        public bool IsWater { get; set; }
        public Point Coordinate { get; set; }
    }

}
