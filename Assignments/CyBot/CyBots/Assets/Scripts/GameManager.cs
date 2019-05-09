using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject StartZone;
    public GameObject EndZone;

    // Start is called before the first frame update
    void Start()
    {

        Instantiate(StartZone, new Vector3(0, .01f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        Instantiate(player, new Vector3(0, .01f, 0), Quaternion.Euler(new Vector3(0, 90, 0)));

        Instantiate(EndZone, new Vector3(10, .01f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
