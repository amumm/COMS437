using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_Input : MonoBehaviour
{
    public GameObject piece;

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
        if (row >= 0 && row < 8 && col >= 0 && col < 8 && pieces[row, col] == null)
        {
            pieces[row, col] = Instantiate(piece, new Vector3(col + 0.5f, baseTileHeight, -row - 0.5f), Quaternion.Euler(blackOrientation));
        }
    }
}
