using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition();
    }

    void playerPosition()
    {
        Vector3 targetPosition = PlayerController.Instance.transform.position + offset;
        // targetPosition.y = transform.position.y; // Mantém a posição y da câmera

        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        transform.position = newPosition;
        // transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
    }
}
