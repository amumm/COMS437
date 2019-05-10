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
        Vector3 start = getRandomStart();
        Vector3 end = getRandomEnd();
        Instantiate(StartZone, start, Quaternion.Euler(new Vector3(90, 0, 0)));
        Instantiate(player, start, Quaternion.Euler(new Vector3(0, 90, 0)));

        Instantiate(EndZone, end, Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 getRandomStart()
    {
        float x = Random.Range(10f, 30);
        float z = Random.Range(0f, 60);
        Vector3 start = new Vector3(-x, 0.01f, z);
        return start;
    }

    Vector3 getRandomEnd()
    {
        float x = Random.Range(10f, 30);
        float z = Random.Range(0f, 60);
        Vector3 end = new Vector3(x, 0.5f, z);
        return end;
    }

}
