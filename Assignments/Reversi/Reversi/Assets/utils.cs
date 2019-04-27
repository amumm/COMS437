using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class utils
    {
        public static StateNode[,] createNodeBoard(PieceNode[,] pieces)
        {
            StateNode[,] board = new StateNode[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    PieceNode piece = pieces[x, y];
                    if (piece == null)
                        continue;

                    board[x, y] = new StateNode(piece.state, x, y, null);
                }
            }
            return board;
        }

        public static bool isInBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        public static Move canPlacePiece(StateNode[,] board, int row, int col, Player player)
        {
            if (board[row, col] != null)
                return null;

            Player checkSide;
            if (player == Player.black)
                checkSide = Player.white;
            else
                checkSide = Player.black;

            bool flipTopLeft = checkDirection(board, row, col, -1, -1, checkSide);
            bool flipTop = checkDirection(board, row, col, 0, -1, checkSide);
            bool flipTopRight = checkDirection(board, row, col, 1, -1, checkSide);
            bool flipLeft = checkDirection(board, row, col, -1, 0, checkSide);
            bool flipRight = checkDirection(board, row, col, 1, 0, checkSide);
            bool flipBottomLeft = checkDirection(board, row, col, -1, 1, checkSide);
            bool flipBottom = checkDirection(board, row, col, 0, 1, checkSide);
            bool flipBottomRight = checkDirection(board, row, col, 1, 1, checkSide);

            Move move = null;
            if (flipTopLeft || flipTop || flipTopRight || flipLeft || flipRight || flipBottomLeft || flipBottom || flipBottomRight)
            {
                move = new Move(player, row, col);
                move.flipTopLeft = flipTopLeft;
                move.flipTop = flipTop;
                move.flipTopRight = flipTopRight;
                move.flipLeft = flipLeft;
                move.flipRight = flipRight;
                move.flipBottomLeft = flipBottomLeft;
                move.flipBottom = flipBottom;
                move.flipBottomRight = flipBottomRight;
            }

            return move;
        }

        public static ArrayList findMoves(StateNode[,] board, Player player)
        {
            ArrayList availableMoves = new ArrayList();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Move move = canPlacePiece(board, x, y, player);
                    if (move != null)
                        availableMoves.Add(move);
                }
            }

            return availableMoves;
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
