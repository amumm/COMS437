namespace Assets
{
    public class Move
    {
        public bool flipTopLeft;
        public bool flipTop;
        public bool flipTopRight;
        public bool flipLeft;
        public bool flipRight;
        public bool flipBottomLeft;
        public bool flipBottom;
        public bool flipBottomRight;

       public int row;
       public int col;
        
       public Player player;

        public Move(Player player, int row, int col)
        {
            this.player = player;
            this.row = row;
            this.col = col;
        }
    }
}
