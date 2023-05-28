using ExcelBot.Runtime.ExcelModels;
using ExcelBot.Runtime.Models;
using System;

namespace ExcelBot.Runtime
{
    public class MoveOption
    {
        public Point From { get; set; }
        public Point To { get; set; }

        public string Rank { get; set; } = "";

        public int Steps { get; set; } = 1;
        public bool WillBeDecisiveVictory { get; set; }
        public bool WillBeDecisiveLoss { get; set; }
        public bool WillBeUnknownBattle { get; set; }
        public bool IsBattleOnOpponentHalf { get; set; }
        public bool IsBattleOnOwnHalf { get; set; }
        public bool IsMoveTowardsOpponentHalf { get; set; }
        public bool IsMoveWithinOpponentHalf { get; set; }
        public bool IsMovingForFirstTime { get; set; }
        public bool IsMoveForUnrevealedPiece { get; set; }

        public double NetChangeInManhattanDistanceToPotentialFlag { get; set; }

        public double Score { get; set; }

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

        public Move ToMove()
        {
            return new Move
            {
                From = From,
                To = To,
            };
        }
    }
}
