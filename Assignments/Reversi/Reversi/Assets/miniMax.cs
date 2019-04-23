using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class miniMax
    {
        private static Move canPlacePiece(Node[,] board, int row, int col, Player player)
        {
            Player checkSide;
            if (player == Player.black)
                checkSide = Player.white;
            else
                checkSide = Player.black;

            bool flipTopLeft = utils.checkDirection(board, row, col, -1, -1, checkSide);
            bool flipTop = utils.checkDirection(board, row, col, 0, -1, checkSide);
            bool flipTopRight = utils.checkDirection(board, row, col, 1, -1, checkSide);
            bool flipLeft = utils.checkDirection(board, row, col, -1, 0, checkSide);
            bool flipRight = utils.checkDirection(board, row, col, 1, 0, checkSide);
            bool flipBottomLeft = utils.checkDirection(board, row, col, -1, 1, checkSide);
            bool flipBottom = utils.checkDirection(board, row, col, 0, 1, checkSide);
            bool flipBottomRight = utils.checkDirection(board, row, col, 1, 1, checkSide);

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
        private static ArrayList findMoves(Node[,] board, Player player)
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

        private static Node[,] flipPieces(Node[,] board, Move move)
        {
            if (move.flipTopLeft)
                utils.flipDirection(board, move.row, move.col, -1, -1, move.player);
            if (move.flipTop)
                utils.flipDirection(board, move.row, move.col, 0, -1, move.player);
            if (move.flipTopRight)
                utils.flipDirection(board, move.row, move.col, 1, -1, move.player);
            if (move.flipLeft)
                utils.flipDirection(board, move.row, move.col, -1, 0, move.player);
            if (move.flipRight)
                utils.flipDirection(board, move.row, move.col, 1, 0, move.player);
            if (move.flipBottomLeft)
                utils.flipDirection(board, move.row, move.col, -1, 1, move.player);
            if (move.flipBottom)
                utils.flipDirection(board, move.row, move.col, 0, 1, move.player);
            if (move.flipBottomRight)
                utils.flipDirection(board, move.row, move.col, 1, 1, move.player);

            return board;
        }

        public static void simulateMoves(Node[,] board, int maxDepth)
        {
            int currentDepth = 0;

            ArrayList moves = findMoves(board, Player.white);

            if (moves.Capacity > 0)
            {
                simulateMovesRec(new Node(Player.white), moves, board);
            }
            return;
        }

        private static void simulateMovesRec(Node root, ArrayList moves, Node[,] board)
        {

        }

        public static ArrayList minMax()
        {

            return new ArrayList();
        }
    }
}
