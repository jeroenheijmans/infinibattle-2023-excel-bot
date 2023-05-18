namespace ExcelBot.Models
{
    public class GameInit
    {
        public Player You { get; set; } = Player.Red;
        public string[] AvailablePieces { get; set; } = Array.Empty<string>();
    }

}
