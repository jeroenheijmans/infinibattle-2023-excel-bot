using Newtonsoft.Json;

namespace ExcelBot.Models
{
    public class GameInit
    {
        public Player You { get; set; } = Player.Red;
        public string[] AvailablePieces { get; set; } = Array.Empty<string>();

        public static GameInit FromJson(string json) =>
            JsonConvert.DeserializeObject<GameInit>(json)
            ?? throw new Exception("GameInit deserialization error");
    }
}
