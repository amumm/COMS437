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

        public Node(Player state, Node parent)
        {
            this.state = state;
            this.parent = parent;
            children = new ArrayList();
        }

        public void addChild(Node child)
        {
            children.Add(child);
        }
    }
}
