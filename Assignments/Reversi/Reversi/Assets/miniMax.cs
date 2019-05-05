using System.Collections;

namespace Assets
{
    public static class MiniMax
    {
        private static StateNode simulateMoves(StateNode[,] board, int maxDepth)
        {
            int currentDepth = 0;

            ArrayList moves = utils.findMoves(board, Player.white);

            StateNode root = null;
            if (moves.Count > 0)
            {
                root = new StateNode(Player.white, -1, -1, null);
                simulateMovesRec(root, moves, board, maxDepth, currentDepth + 1);
            }
            return root;
        }

        private static void simulateMovesRec(StateNode root, ArrayList moves, StateNode[,] board, int maxDepth, int depth)
        {
            if (depth > maxDepth || moves.Count == 0)
            {
                utils.setScore(root, board);
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
                utils.flipPieces(tempBoard, move);

                // Simulate the opponent's possible counter moves.
                Player nextPlayer;
                if (move.player == Player.black)
                    nextPlayer = Player.white;
                else
                    nextPlayer = Player.black;

                ArrayList tempMoves = utils.findMoves(tempBoard, nextPlayer);
                simulateMovesRec(child, tempMoves, tempBoard, maxDepth, depth + 1);
            }
        }

        public static StateNode minMax(StateNode[,] board, int maxDepth)
        {
            StateNode root = simulateMoves(board, maxDepth);
            if (root == null)
                return root;

            return minMaxRec(true, root, 0, maxDepth);
        }

        private static StateNode minMaxRec(bool isMaximizer, StateNode root, int currentDepth, int maxDepth)
        {
            // Break recursion if a leaf node is reached
            if (currentDepth > maxDepth || root.children.Count == 0)
                return root;

            ArrayList childResults = new ArrayList();
            foreach(StateNode child in root.children)
            {
                StateNode childResult = minMaxRec(!isMaximizer, child, currentDepth + 1, maxDepth);
                childResults.Add(childResult);
            }

            // Find the max
            if (isMaximizer)
            {
                StateNode max = findMax(childResults);
                if (root.row != -1 && root.col != -1)
                {
                    max.row = root.row;
                    max.col = root.col;
                }
                return max;
            }

            // Find the min
            StateNode min = findMin(childResults);
            if (root.row != -1 && root.col != -1)
            {
                min.row = root.row;
                min.col = root.col;
            }
            return min;

        }

        private static StateNode findMax(ArrayList children)
        {
            StateNode maxStateNode = null;
            int max = -1;
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

        private static StateNode findMin(ArrayList children)
        {
            StateNode minStateNode = null;
            int min = 65;
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
