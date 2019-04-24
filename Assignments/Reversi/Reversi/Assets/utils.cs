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

        public static bool checkDirection(StateNode[,] board, int row, int col, int x, int z, Player player)
        {
            row += z;
            col += x;
            if (!isInBounds(row, col))
                return false;

            var cur = board[row, col];
            if (cur == null)
                return false;

            bool foundOppositeColor = false;
            while (cur.state == player)
            {
                foundOppositeColor = true;
                row += z;
                col += x;
                if (isInBounds(row, col))
                    cur = board[row, col];
                else
                    return false;

                if (cur == null)
                    return false;
            }
            return foundOppositeColor;
        }

        public static StateNode[,] flipDirection(StateNode[,] board, int row, int col, int x, int z, Player player)
        {
            row += z;
            col += x;
            if (!isInBounds(row, col))
                return board;

            StateNode cur = board[row, col];
            if (cur == null)
                return board;

            while (cur.state != player)
            {
                cur.state = player;
                row += z;
                col += x;
                if (isInBounds(row, col))
                    cur = board[row, col];
                else
                    return board;

                if (cur == null)
                    return board;
            }

            return board;
        }
    }
}
