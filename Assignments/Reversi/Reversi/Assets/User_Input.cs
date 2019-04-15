using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_Input : MonoBehaviour
{
    public GameObject blackPiece;
    public GameObject whitePiece;

    private const int numberOfRows = 8;
    private const int numberOfColumns = 8;
    private const float baseTileHeight = 1.1f;
    private Vector3 blackOrientation = new Vector3(0, 0, 180);
    private Vector3 whiteOrientation = new Vector3(0, 0, 0);
    private GameObject[,] pieces;

    private bool playersTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[numberOfRows, numberOfColumns];

        pieces[3, 3] = Instantiate(blackPiece, new Vector3(3.5f, baseTileHeight, -3.5f), Quaternion.Euler(blackOrientation));
        pieces[3, 4] = Instantiate(whitePiece, new Vector3(4.5f, baseTileHeight, -3.5f), Quaternion.Euler(whiteOrientation));
        pieces[4, 3] = Instantiate(whitePiece, new Vector3(3.5f, baseTileHeight, -4.5f), Quaternion.Euler(whiteOrientation));
        pieces[4, 4] = Instantiate(blackPiece, new Vector3(4.5f, baseTileHeight, -4.5f), Quaternion.Euler(blackOrientation));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        }
    }

    void getCell(Vector3 point)
    {
        int row = (int)(point.z / -1.0f);
        int col = (int)(point.x / 1.0f);

        if (isInBounds(row, col) && pieces[row, col] == null)
        {
            tryPlacePiece(row, col);
        }
    }

    bool isInBounds(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    void tryPlacePiece(int row, int col)
    {
        bool didPlacePiece = false;

        Func<float, bool> checkSide;
        if (playersTurn)
            checkSide = angle => angle > -1 && angle < 1;
        else
            checkSide = angle => angle > 179 && angle < 181;

        bool flipTopLeft = checkDirection(row, col, -1, -1, checkSide);
        bool flipTop = checkDirection(row, col, 0, -1, checkSide);
        bool flipTopRight = checkDirection(row, col, 1, -1, checkSide);
        bool flipLeft = checkDirection(row, col, -1, 0, checkSide);
        bool flipRight = checkDirection(row, col, 1, 0, checkSide);
        bool flipBottomLeft = checkDirection(row, col, -1, 1, checkSide);
        bool flipBottom = checkDirection(row, col, 0, 1, checkSide);
        bool flipBottomRight = checkDirection(row, col, 1, 1, checkSide);

        if (flipTopLeft || flipTop || flipTopRight || flipLeft || flipRight || flipBottomLeft || flipBottom || flipBottomRight)
        {
            if (playersTurn)
                pieces[row, col] = Instantiate(blackPiece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(blackOrientation));
            else
                pieces[row, col] = Instantiate(whitePiece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(whiteOrientation));

            didPlacePiece = true;
        }

        if (flipTopLeft)
            flipDirection(row, col, -1, -1, checkSide);
        if (flipTop)
            flipDirection(row, col, 0, -1, checkSide);
        if (flipTopRight)
            flipDirection(row, col, 1, -1, checkSide);
        if (flipLeft)
            flipDirection(row, col, -1, 0, checkSide);
        if (flipRight)
            flipDirection(row, col, 1, 0, checkSide);
        if (flipBottomLeft)
            flipDirection(row, col, -1, 1, checkSide);
        if (flipBottom)
            flipDirection(row, col, 0, 1, checkSide);
        if (flipBottomRight)
            flipDirection(row, col, 1, 1, checkSide);

        if (didPlacePiece)
            playersTurn = !playersTurn;
    }

    bool checkDirection(int row, int col, int x, int z, Func<float, bool> checkSide)
    {
        if (!isInBounds(row + z, col + x))
            return false;

        GameObject cur = pieces[row + z, col + x];
        if (cur == null)
            return false;

        bool foundOppositeColor = false;
        var rotation = cur.transform.eulerAngles;
        while(checkSide(rotation.z))
        {
            foundOppositeColor = true;
            x += x;
            z += z;
            if (isInBounds(row + z, col + x))
                cur = pieces[row + z, col + x];
            else
                return false;

            if (cur == null)
                return false;
            rotation = cur.transform.eulerAngles;
        }
        return foundOppositeColor;
    }

    void flipDirection(int row, int col, int x, int z, Func<float, bool> checkSide)
    {
        if (!isInBounds(row + z, col + x))
            return;

        GameObject cur = pieces[row + z, col + x];
        if (cur == null)
            return;

        var rotation = cur.transform.eulerAngles;
        while (checkSide(rotation.z))
        {
            Animator animator = cur.GetComponent<Animator>();
            if (playersTurn)
                animator.SetTrigger("flipWhiteToBlack");
            else
                //animator.SetTrigger("flipWhiteToBlack");
                animator.SetTrigger("flipBlackToWhite");

            x += x;
            z += z;
            if (isInBounds(row + z, col + x))
                cur = pieces[row + z, col + x];
            else
                return;

            if (cur == null)
                return;
            rotation = cur.transform.eulerAngles;
        }
    }

}
