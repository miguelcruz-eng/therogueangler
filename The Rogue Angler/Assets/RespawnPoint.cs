using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.respawnPoint = transform.position;
        }
    }
}
