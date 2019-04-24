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

        public static Node simulateMoves(Node[,] board, int maxDepth)
        {
            int currentDepth = 0;

            ArrayList moves = findMoves(board, Player.white);

            Node root = null;
            if (moves.Capacity > 0)
            {
                root = new Node(Player.white, -1, -1, null);
                simulateMovesRec(root, moves, board, maxDepth, currentDepth);
            }
            return root;
        }

        private static void simulateMovesRec(Node root, ArrayList moves, Node[,] board, int maxDepth, int depth)
        {
            if (++depth >= maxDepth)
            {
                setScore(root, board);
                return;
            }
            foreach(Move move in moves){
                // Set up a parent and child relationship with the nodes.
                Node child = new Node(move.player, move.row, move.col, root);
                root.addChild(child);

                Node[,] tempBoard = new Node[8, 8];
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

        public static void setScore(Node node, Node[,] board)
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

        public static Node[,] createNodeBoard(GameObject[,] pieces)
        {
            Node[,] board = new Node[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    GameObject piece = pieces[x, y];
                    if (piece == null)
                        continue;

                    Player player;
                    var rotation = piece.transform.eulerAngles.z;
                    if (rotation > -1 && rotation < 1)
                    {
                        player = Player.white;
                    } else { 
                        player = Player.black;
                    }

                    board[x, y] = new Node(player, x, y, null);
                }
            }
            return board;
        }

        public static Node minMax(Node[,] board, int maxDepth)
        {
            Node root = simulateMoves(board, maxDepth);
            if (root == null)
                return root;

            return minMaxRec(true, root, 0, maxDepth);
        }

        public static Node minMaxRec(bool isMaximizer, Node root, int currentDepth, int maxDepth)
        {
            // Break recursion if a leaf node is reached
            if (currentDepth == maxDepth)
                return root;

            ArrayList childResults = new ArrayList();
            foreach(Node child in root.children)
            {
                Node childResult = minMaxRec(!isMaximizer, child, currentDepth++, maxDepth);
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

        public static Node findMax(ArrayList children)
        {
            Node maxNode = null;
            int max = 0;
            foreach(Node child in children)
            {
                if (child.numWhite > max)
                {
                    max = child.numWhite;
                    maxNode = child;
                }
            }
            return maxNode;
        }

        public static Node findMin(ArrayList children)
        {
            Node minNode = null;
            int min = 64;
            foreach (Node child in children)
            {
                if (child.numWhite < min)
                {
                    min = child.numWhite;
                    minNode = child;
                }
            }
            return minNode;
        }
    }
}
