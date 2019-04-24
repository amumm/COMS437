using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public static class MiniMax
    {
        private static Move canPlacePiece(StateNode[,] board, int row, int col, Player player)
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
        private static ArrayList findMoves(StateNode[,] board, Player player)
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

        private static StateNode[,] flipPieces(StateNode[,] board, Move move)
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

        public static StateNode simulateMoves(StateNode[,] board, int maxDepth)
        {
            int currentDepth = 0;

            ArrayList moves = findMoves(board, Player.white);

            StateNode root = null;
            if (moves.Capacity > 0)
            {
                root = new StateNode(Player.white, -1, -1, null);
                simulateMovesRec(root, moves, board, maxDepth, currentDepth);
            }
            return root;
        }

        private static void simulateMovesRec(StateNode root, ArrayList moves, StateNode[,] board, int maxDepth, int depth)
        {
            if (++depth > maxDepth)
            {
                setScore(root, board);
                return;
            }
            foreach(Move move in moves){
                // Set up a parent and child relationship with the nodes.
                StateNode child = new StateNode(move.player, move.row, move.col, root);
                root.addChild(child);

                StateNode[,] tempBoard = new StateNode[8, 8];
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        // TODO: Add deep copy
                        tempBoard[x, y] = board[x, y];
                    }
                }

                // Add move to the game board
                tempBoard[move.row, move.col] = child;

                // Flip the simulated pieces for the move.
                flipPieces(tempBoard, move);

                // Simulate the opponent's possible counter moves.
                Player nextPlayer;
                if (move.player == Player.black)
                    nextPlayer = Player.white;
                else
                    nextPlayer = Player.black;

                ArrayList tempMoves = findMoves(tempBoard, nextPlayer);
                if (tempMoves.Capacity > 0)
                {
                    simulateMovesRec(child, tempMoves, tempBoard, maxDepth, depth);
                }
            }
        }

        public static void setScore(StateNode node, StateNode[,] board)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y] == null)
                        continue;
                    if (board[x, y].state == Player.black)
                        node.numBlack++;
                    else 
                        node.numWhite++;
                }
            }
        }

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

        public static StateNode minMax(StateNode[,] board, int maxDepth)
        {
            StateNode root = simulateMoves(board, maxDepth);
            if (root == null)
                return root;

            return minMaxRec(true, root, 0, maxDepth);
        }

        public static StateNode minMaxRec(bool isMaximizer, StateNode root, int currentDepth, int maxDepth)
        {
            // Break recursion if a leaf node is reached
            if (currentDepth == maxDepth)
                return root;

            ArrayList childResults = new ArrayList();
            foreach(StateNode child in root.children)
            {
                StateNode childResult = minMaxRec(!isMaximizer, child, currentDepth++, maxDepth);
                childResults.Add(childResult);
            }

            // Find the max
            if (isMaximizer)
            {
                return findMax(childResults);
            }

            // Find the min
            return findMin(childResults);

        }

        public static StateNode findMax(ArrayList children)
        {
            StateNode maxStateNode = null;
            int max = 0;
            foreach(StateNode child in children)
            {
                if (child.numWhite > max)
                {
                    max = child.numWhite;
                    maxStateNode = child;
                }
            }
            return maxStateNode;
        }

        public static StateNode findMin(ArrayList children)
        {
            StateNode minStateNode = null;
            int min = 64;
            foreach (StateNode child in children)
            {
                if (child.numWhite < min)
                {
                    min = child.numWhite;
                    minStateNode = child;
                }
            }
            return minStateNode;
        }
    }
}
