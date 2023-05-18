using ExcelBot;
using ExcelBot.Models;
using Newtonsoft.Json;

Console.WriteLine("bot-start");

var initData = Console.ReadLine() ?? throw new Exception("No GameInit");
var gameInit = GameInit.FromJson(initData);
var strategy = new Strategy(new Random());
var boardSetup = strategy.initialize(gameInit);
var setupMessage = JsonConvert.SerializeObject(boardSetup);

Console.WriteLine(setupMessage);

while (true)
{
    var stateData = Console.ReadLine() ?? throw new Exception("No GameState");
    var gameState = GameState.FromJson(stateData);
    var move = strategy.Process(gameState);
    if (move != null)
    {
        var moveMessage = JsonConvert.SerializeObject(move);
        Console.WriteLine(moveMessage);
    }
}
