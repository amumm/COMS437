using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public enum Player
    {
        black = 0,
        white = 1
    }


    public class Node
    {
        public Player state;
        public Node parent;
        public ArrayList children;
        public int row;
        public int col;
        public int numBlack;
        public int numWhite;

        public Node(Player state, int row, int col, Node parent)
        {
            this.state = state;
            this.row = row;
            this.col = col;
            this.parent = parent;
            children = new ArrayList();
            numBlack = 0;
            numWhite = 0;
        }

        public void addChild(Node child)
        {
            children.Add(child);
        }
    }
}
