using ExcelBot;
using ExcelBot.Models;
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
