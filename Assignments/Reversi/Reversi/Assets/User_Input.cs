﻿using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class User_Input : MonoBehaviour
{
    public GameObject blackPiece;
    public GameObject whitePiece;

    private const int numberOfRows = 8;
    private const int numberOfColumns = 8;
    private const float baseTileHeight = 1.1f;
    private Vector3 blackOrientation = new Vector3(0, 0, 180);
    private Vector3 whiteOrientation = new Vector3(0, 0, 0);
    private PieceNode[,] pieces;

    public GameObject scoreTextBlack;
    public GameObject scoreTextWhite;

    private int difficulty;
    public Button difficultyButton1;
    public Button difficultyButton2;
    public Button difficultyButton3;
    public Button difficultyButton4;
    public Button difficultyButton5;
    public GameObject difficultyPanel;

    private float waitTime = 2f;
    private float timeBetween = 0f;

    private bool playersTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new PieceNode[numberOfRows, numberOfColumns];
        
        var gameObject1 = Instantiate(blackPiece, new Vector3(3.5f, baseTileHeight, -3.5f), Quaternion.Euler(blackOrientation));
        var gameObject2 = Instantiate(whitePiece, new Vector3(4.5f, baseTileHeight, -3.5f), Quaternion.Euler(whiteOrientation));
        var gameObject3 = Instantiate(whitePiece, new Vector3(3.5f, baseTileHeight, -4.5f), Quaternion.Euler(whiteOrientation));
        var gameObject4 = Instantiate(blackPiece, new Vector3(4.5f, baseTileHeight, -4.5f), Quaternion.Euler(blackOrientation));

        pieces[3, 3] = new PieceNode(Player.black, 3, 3, gameObject1);
        pieces[3, 4] = new PieceNode(Player.white, 3, 4, gameObject2);
        pieces[4, 3] = new PieceNode(Player.white, 4, 3, gameObject3);
        pieces[4, 4] = new PieceNode(Player.black, 4, 4, gameObject4);

        scoreTextBlack.GetComponent<Text>().text = "# Black: 2";
        scoreTextWhite.GetComponent<Text>().text = "# White: 2";

        difficultyButton1.onClick.AddListener(delegate { setDifficulty(1); });
        difficultyButton2.onClick.AddListener(delegate { setDifficulty(2); });
        difficultyButton3.onClick.AddListener(delegate { setDifficulty(3); });
        difficultyButton4.onClick.AddListener(delegate { setDifficulty(4); });
        difficultyButton5.onClick.AddListener(delegate { setDifficulty(5); });
    }

    // Update is called once per frame
    void Update()
    {
        timeBetween += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && playersTurn)
            //if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == "Board")
                    {
                        getCell(hit.point);
                    }
                }
                timeBetween = 0f;
            }
        }
        else if (!playersTurn && timeBetween > waitTime)
        {
            StateNode[,] board = MiniMax.createNodeBoard(pieces);
            StateNode bestMove = MiniMax.minMax(board, difficulty);
            if (bestMove != null)
            {
                tryPlacePiece(bestMove.row, bestMove.col);
                setScore();
            }
            else
                playersTurn = !playersTurn;

            timeBetween = 0f;
        }
    }

    private void setDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
        difficultyPanel.SetActive(false);
    }

    void setScore()
    {
        int numBlack = 0;
        int numWhite = 0;
        foreach(PieceNode piece in pieces)
        {
            if (piece == null)
                continue;
            if (piece.state == Player.black)
                numBlack++;
            else
                numWhite++;
        }

        scoreTextBlack.GetComponent<Text>().text = "# Black: " + numBlack;
        scoreTextWhite.GetComponent<Text>().text = "# White: " + numWhite;
    }

    void getCell(Vector3 point)
    {
        int row = (int)(point.z / -1.0f);
        int col = (int)(point.x / 1.0f);

        if (utils.isInBounds(row, col) && pieces[row, col] == null)
        {
            tryPlacePiece(row, col);
        }
    }

    void tryPlacePiece(int row, int col)
    {
        bool didPlacePiece = false;

        Player opponent;
        if (playersTurn)
        {
            opponent = Player.white;
        } else {
            opponent = Player.black;
        }

        bool flipTopLeft = checkDirection(row, col, -1, -1, opponent);
        bool flipTop = checkDirection(row, col, 0, -1, opponent);
        bool flipTopRight = checkDirection(row, col, 1, -1, opponent);
        bool flipLeft = checkDirection(row, col, -1, 0, opponent);
        bool flipRight = checkDirection(row, col, 1, 0, opponent);
        bool flipBottomLeft = checkDirection(row, col, -1, 1, opponent);
        bool flipBottom = checkDirection(row, col, 0, 1, opponent);
        bool flipBottomRight = checkDirection(row, col, 1, 1, opponent);

        if (flipTopLeft || flipTop || flipTopRight || flipLeft || flipRight || flipBottomLeft || flipBottom || flipBottomRight)
        {
            if (playersTurn)
            {
                var gameObject = Instantiate(blackPiece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(blackOrientation));
                pieces[row, col] = new PieceNode(Player.black, row, col, gameObject);

            } else {
                var gameObject = Instantiate(whitePiece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(whiteOrientation));
                pieces[row, col] = new PieceNode(Player.white, row, col, gameObject);
            }

            didPlacePiece = true;
        }

        if (flipTopLeft)
            flipDirection(row, col, -1, -1, opponent);
        if (flipTop)
            flipDirection(row, col, 0, -1, opponent);
        if (flipTopRight)
            flipDirection(row, col, 1, -1, opponent);
        if (flipLeft)
            flipDirection(row, col, -1, 0, opponent);
        if (flipRight)
            flipDirection(row, col, 1, 0, opponent);
        if (flipBottomLeft)
            flipDirection(row, col, -1, 1, opponent);
        if (flipBottom)
            flipDirection(row, col, 0, 1, opponent);
        if (flipBottomRight)
            flipDirection(row, col, 1, 1, opponent);

        if (didPlacePiece)
        {
            playersTurn = !playersTurn;
            setScore();
        }
    }

    bool checkDirection(int row, int col, int x, int z, Player opponent)
    {
        row += z;
        col += x;
        if (!utils.isInBounds(row, col))
            return false;

        var cur = pieces[row, col];
        if (cur == null)
            return false;

        bool foundOppositeColor = false;
        while(cur.state == opponent)
        {
            foundOppositeColor = true;
            row += z;
            col += x;
            if (utils.isInBounds(row, col))
                cur = pieces[row, col];
            else
                return false;

            if (cur == null)
                return false;
        }
        return foundOppositeColor;
    }

    void flipDirection(int row, int col, int x, int z, Player opponent)
    {
        row += z;
        col += x;
        if (!utils.isInBounds(row, col))
            return;

        PieceNode cur = pieces[row, col];
        if (cur == null)
            return;

        while (cur.state == opponent)
        {
            Animator animator = cur.gameObject.GetComponent<Animator>();
            if (playersTurn)
            {
                cur.state = Player.black;
                animator.SetTrigger("flipWhiteToBlack");
            }
            else {
                cur.state = Player.white;
                animator.SetTrigger("flipBlackToWhite");
            }

            row += z;
            col += x;
            if (utils.isInBounds(row, col))
                cur = pieces[row, col];
            else
                return;

            if (cur == null)
                return;
        }
    }

}
