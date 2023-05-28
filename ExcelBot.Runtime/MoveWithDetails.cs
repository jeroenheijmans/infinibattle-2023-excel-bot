using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using Newtonsoft.Json;
using System;

namespace ExcelBot.Runtime
{
    public class MoveWithDetails : Move
    {
        [JsonIgnore] public string Rank { get; set; } = "";

        [JsonIgnore] public int Steps { get; set; } = 1;
        [JsonIgnore] public bool WillBeDecisiveVictory { get; set; }
        [JsonIgnore] public bool WillBeDecisiveLoss { get; set; }
        [JsonIgnore] public bool WillBeUnknownBattle { get; set; }
        [JsonIgnore] public bool IsBattleOnOpponentHalf { get; set; }
        [JsonIgnore] public bool IsBattleOnOwnHalf { get; set; }
        [JsonIgnore] public bool IsMoveTowardsOpponentHalf { get; set; }
        [JsonIgnore] public bool IsMoveWithinOpponentHalf { get; set; }
        [JsonIgnore] public bool IsMovingForFirstTime { get; set; }
        [JsonIgnore] public bool IsMoveForUnrevealedPiece { get; set; }

        [JsonIgnore] public double NetChangeInManhattanDistanceToPotentialFlag { get; set; }

        [JsonIgnore] public double Score { get; set; }

        public void CalculateScore(StrategyData strategyData, Random random)
        {
            if (WillBeDecisiveVictory) Score += strategyData.DecisiveVictoryPoints;
            if (WillBeDecisiveLoss) Score += strategyData.DecisiveLossPoints;
            if (WillBeUnknownBattle && IsBattleOnOwnHalf) Score += strategyData.UnknownBattleOwnHalfPoints;
            if (WillBeUnknownBattle && IsBattleOnOpponentHalf) Score += strategyData.UnknownBattleOpponentHalfPoints;
            if (IsMoveTowardsOpponentHalf) Score += strategyData.BonusPointsForMoveTowardsOpponent;
            if (IsMoveWithinOpponentHalf) Score += strategyData.BonusPointsForMoveWithinOpponentArea;
            if (IsMovingForFirstTime) Score += strategyData.BonusPointsForMovingPieceForTheFirstTime;
            if (IsMoveForUnrevealedPiece) Score += strategyData.BonusPointsForMovingUnrevealedPiece;

            if (NetChangeInManhattanDistanceToPotentialFlag < 0)
                Score += strategyData.ScoutJumpsToPotentialFlagsMultiplication
                    ? strategyData.BonusPointsForMovesGettingCloserToPotentialFlags
                        * (Steps > 1 ? Math.Abs(NetChangeInManhattanDistanceToPotentialFlag) : 1)
                    : strategyData.BonusPointsForMovesGettingCloserToPotentialFlags;

            var boost = 0;
            if (Rank == "Spy") boost = strategyData.BoostForSpy;
            if (Rank == "Scout") boost = strategyData.BoostForScout;
            if (Rank == "Miner") boost = strategyData.BoostForMiner;
            if (Rank == "General") boost = strategyData.BoostForGeneral;
            if (Rank == "Marshal") boost = strategyData.BoostForMarshal;

            double boostMultiplier = (Score < 0 ? -boost : +boost) + 100;
            Score *= boostMultiplier / 100;

            double fuzzynessMultiplier = random.Next(strategyData.FuzzynessFactor) + 100;
            Score *= fuzzynessMultiplier / 100;
        }
    }
}
