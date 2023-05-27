namespace ExcelBot.Runtime.Models
{
    public class Cell
    {
        public string Rank { get; set; } = "";
        public Player? Owner { get; set; } = null;
        public bool IsWater { get; set; }
        public Point Coordinate { get; set; }

        public bool IsPiece => Owner != null;
        public bool IsUnknownPiece => Owner != null && Rank == "?";
        public bool IsKnownPiece => Owner != null && Rank != "?";

        public bool CanBeDefeatedBy(string attacker)
        {
            if (Owner == null) return false;
            if (Rank == "?") return false;
            if (attacker == "?") return false;

            if (Rank == "Flag") return true;
            if (Rank == "Bomb") return attacker == "Miner";
            if (Rank == "Marshal") return attacker == "Spy";
            if (Rank == "General") return attacker == "Marshal";
            if (Rank == "Miner") return attacker == "Marshal" || attacker == "General";
            if (Rank == "Scout") return attacker == "Marshal" || attacker == "General" || attacker == "Miner";
            return true; // Spy is always defeated by an attacker
        }

        public bool WillCauseDefeatFor(string attacker)
        {
            if (Owner == null) return false;
            if (Rank == "?") return false;
            if (attacker == "?") return false;

            if (Rank == "Flag") return false;
            if (Rank == "Bomb") return attacker != "Miner";
            if (Rank == "Marshal") return attacker != "Spy";
            if (Rank == "General") return attacker == "Spy" || attacker == "Scout" || attacker == "Miner";
            if (Rank == "Miner") return attacker == "Scout" || attacker == "Spy";
            if (Rank == "Scout") return attacker == "Spy";
            return false; // Spy never defeats an attacker
        }

        public bool IsOnOpponentHalf(Player attacker)
        {
            return attacker == Player.Red
                ? Coordinate.Y > 5
                : Coordinate.Y < 4;
        }

        public static bool IsOnOpponentHalf(Player attacker, Point coordinate)
        {
            return attacker == Player.Red
                ? coordinate.Y > 5
                : coordinate.Y < 4;
        }

        public bool IsOnOwnHalf(Player attacker)
        {
            return attacker == Player.Red
                ? Coordinate.Y < 4
                : Coordinate.Y > 5;
        }

        public static bool IsOnOwnHalf(Player attacker, Point coordinate)
        {
            return attacker == Player.Red
                ? coordinate.Y < 4
                : coordinate.Y > 5;
        }

        public bool IsOpponentPiece(Player myColor)
        {
            return Owner.HasValue && Owner.Value != myColor;
        }
    }
}
