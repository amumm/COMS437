using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class Move
    {
        bool flipTopLeft;
        bool flipTop;
        bool flipTopRight;
        bool flipLeft;
        bool flipRight;
        bool flipBottomLeft;
        bool flipBottom;
        bool flipBottomRight;

        int row;
        int col;

        Player player;

        public Move(Player player, int row, int col)
        {
            this.player = player;
            this.row = row;
            this.col = col;
        }
    }
}
