using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    enum Player
    {
        black = 0,
        white = 1
    }


    public class Node
    {
        int state;

        public Node(int state)
        {
            this.state = state;
        }
    }
}
