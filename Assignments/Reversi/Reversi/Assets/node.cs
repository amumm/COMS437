using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public enum Player
    {
        black = 0,
        white = 1
    }


    public class StateNode
    {
        public Player state;
        public StateNode parent;
        public ArrayList children;
        public int row;
        public int col;
        public int numBlack;
        public int numWhite;

        public StateNode(Player state, int row, int col, StateNode parent)
        {
            this.state = state;
            this.row = row;
            this.col = col;
            this.parent = parent;
            children = new ArrayList();
            numBlack = 0;
            numWhite = 0;
        }

        public void addChild(StateNode child)
        {
            children.Add(child);
        }
    }

    public class PieceNode
    {
        public Player state;
        public int row;
        public int col;
        public GameObject gameObject;

        public PieceNode(Player state, int row, int col, GameObject gameObject)
        {
            this.state = state;
            this.row = row;
            this.col = col;
            this.gameObject = gameObject;
        }

    }
}
