using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fase1 : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Enemy;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Player, new Vector3(-8, 3, 0), Quaternion.identity);
        Instantiate(Enemy[0], new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(Enemy[1], new Vector3(-4, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
