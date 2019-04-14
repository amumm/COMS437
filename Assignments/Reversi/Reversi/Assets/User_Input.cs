using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_Input : MonoBehaviour
{
    public GameObject piece;

    private const int numberOfRows = 8;
    private const int numberOfColumns = 8;
    private const float baseTileHeight = 1.5f;
    private Vector3 blackOrientation = new Vector3(0, 0, 180);
    private Vector3 whiteOrientation = new Vector3(0, 0, 0);
    private GameObject[,] pieces;

    private bool playersTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[numberOfRows, numberOfColumns];

        pieces[3, 3] = Instantiate(piece, new Vector3(3.5f, baseTileHeight, -3.5f), Quaternion.Euler(blackOrientation));
        pieces[3, 4] = Instantiate(piece, new Vector3(4.5f, baseTileHeight, -3.5f), Quaternion.Euler(whiteOrientation));
        pieces[4, 3] = Instantiate(piece, new Vector3(3.5f, baseTileHeight, -4.5f), Quaternion.Euler(whiteOrientation));
        pieces[4, 4] = Instantiate(piece, new Vector3(4.5f, baseTileHeight, -4.5f), Quaternion.Euler(blackOrientation));
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

        Debug.Log("Row: " + row + " Col: " + col);
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
        bool flipTopLeft = checkDirection(row, col, -1, -1);
        bool flipTop = checkDirection(row, col, 0, -1);
        bool flipTopRight = checkDirection(row, col, 1, -1);
        bool flipLeft = checkDirection(row, col, -1, 0);
        bool flipRight = checkDirection(row, col, 1, 0);
        bool flipBottomLeft = checkDirection(row, col, -1, 1);
        bool flipBottom = checkDirection(row, col, 0, 1);
        bool flipBottomRight = checkDirection(row, col, 1, 1);

        if (flipTopLeft || flipTop || flipTopRight || flipLeft || flipRight || flipBottomLeft || flipBottom || flipBottomRight)
            pieces[row, col] = Instantiate(piece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(blackOrientation));

        if (flipTopLeft)
            flipDirection(row, col, -1, -1);
        if (flipTop)
            flipDirection(row, col, 0, -1);
        if (flipTopRight)
            flipDirection(row, col, 1, -1);
        if (flipLeft)
            flipDirection(row, col, -1, 0);
        if (flipRight)
            flipDirection(row, col, 1, 0);
        if (flipBottomLeft)
            flipDirection(row, col, -1, 1);
        if (flipBottom)
            flipDirection(row, col, 0, 1);
        if (flipBottomRight)
            flipDirection(row, col, 1, 1);
    }

    bool checkDirection(int row, int col, int x, int z)
    {
        if (!isInBounds(row + z, col + x))
            return false;

        GameObject cur = pieces[row + z, col + x];
        if (cur == null)
            return false;

        bool foundOppositeColor = false;
        var rotation = cur.transform.rotation;
        while(rotation.z > -1 && rotation.z < 1)
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
            rotation = cur.transform.rotation;
        }
        return foundOppositeColor;
    }

    void flipDirection(int row, int col, int x, int z)
    {
        if (!isInBounds(row + z, col + x))
            return;

        GameObject cur = pieces[row + z, col + x];
        if (cur == null)
            return;

        var rotation = cur.transform.rotation;
        while (rotation.z > -1 && rotation.z < 1)
        {
            //cur.transform.Rotate(180, 180, 180);
            Animator animator = cur.GetComponent<Animator>();
            animator.SetTrigger("flipWhiteToBlack");
            x += x;
            z += z;
            if (isInBounds(row + z, col + x))
                cur = pieces[row + z, col + x];
            else
                return;

            if (cur == null)
                return;
            rotation = cur.transform.rotation;
        }
    }

}
