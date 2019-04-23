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
            Move move = new Move(player, row, col);
            return move;
        }
        private static ArrayList findMoves(Node[,] board, Player player)
        {
            ArrayList availableMoves = new ArrayList();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    canPlacePiece(board, x, y, player);
                }
            }

            return new ArrayList();
        }

        private static ArrayList flipPieces()
        {

            return new ArrayList();
        }

        public static void simulateMoves()
        {

        }

        private static void simuateMovesRec()
        {

        }

        public static ArrayList minMax()
        {

            return new ArrayList();
        }
    }
}
