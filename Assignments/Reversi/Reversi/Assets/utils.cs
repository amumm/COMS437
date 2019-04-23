using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class utils
    {
        public static bool isInBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        public static bool checkDirection(Node[,] board, int row, int col, int x, int z, Player player)
        {
            if (!isInBounds(row + z, col + x))
                return false;

            Node cur = board[row + z, col + x];
            if (cur == null)
                return false;

            bool foundOppositeColor = false;
            while (cur.state == player)
            {
                foundOppositeColor = true;
                x += x;
                z += z;
                if (isInBounds(row + z, col + x))
                    cur = board[row + z, col + x];
                else
                    return false;

                if (cur == null)
                    return false;
            }
            return foundOppositeColor;
        }

        public static Node[,] flipDirection(Node[,] board, int row, int col, int x, int z, Player player)
        {
            if (!isInBounds(row + z, col + x))
                return board;

            Node cur = board[row + z, col + x];
            if (cur == null)
                return board;

            while (cur.state != player)
            {
                cur.state = player;
                x += x;
                z += z;
                if (isInBounds(row + z, col + x))
                    cur = board[row + z, col + x];
                else
                    return board;

                if (cur == null)
                    return board;
            }

            return board;
        }
    }
}
