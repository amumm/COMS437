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
    private GameObject[,] pieces;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[numberOfRows, numberOfColumns];

        pieces[3, 3] = Instantiate(piece, new Vector3(-0.5f, baseTileHeight, 0.5f), Quaternion.identity);
        pieces[3, 4] = Instantiate(piece, new Vector3(0.5f, baseTileHeight, 0.5f), Quaternion.identity);
        pieces[3, 3] = Instantiate(piece, new Vector3(-0.5f, baseTileHeight, -0.5f), Quaternion.identity);
        pieces[4, 4] = Instantiate(piece, new Vector3(0.5f, baseTileHeight, -0.5f), Quaternion.identity);

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
                Debug.Log(hit.transform.name);
                if (hit.transform.name == "Model_Board")
                {
                    Debug.Log(hit.point);
                }
            }
        }
    }
}
