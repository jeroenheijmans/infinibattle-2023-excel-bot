using ExcelBot.Runtime.Models;
using System.Collections.Generic;

namespace ExcelBot.Runtime.Util
{
    public static class Helpers
    {
        public static bool IsOnOpponentHalfFor(this Point coordinate, Player attacker)
        {
            return attacker == Player.Red
                ? coordinate.Y > 5
                : coordinate.Y < 4;
        }

        public static bool IsOnOwnHalfFor(this Point coordinate, Player attacker)
        {
            return attacker == Player.Red
                ? coordinate.Y < 4
                : coordinate.Y > 5;
        }

        public static bool IsWaterColumn(this Point p)
        {
            return p.X == 2 || p.X == 3 || p.X == 6 || p.X == 7;
        }

        public static Player Opponent(this Player player)
        {
            return player == Player.Red ? Player.Blue : Player.Red;
        }

        public static IEnumerable<Point> GetAllHomeCoordinates(this Player player)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    yield return
                        player == Player.Red
                        ? new Point(x, y)
                        : new Point(x, y).Transpose();
                }
            }
        }
    }
}
