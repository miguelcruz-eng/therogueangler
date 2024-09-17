using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public bool interacted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && Input.GetButtonDown("Interact"));
        {
            interacted = true;
        }
    }
}
