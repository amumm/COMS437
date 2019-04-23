using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class utils
    {
        public static bool isInBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }
    }
}
