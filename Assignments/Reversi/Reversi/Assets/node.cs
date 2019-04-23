using System;
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

        public Node(Player state)
        {
            this.state = state;
        }
    }
}
