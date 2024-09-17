using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    
    public Vector2 respawnPoint;
    public Vector2 savePoint;

    [SerializeField] SavePoint savedLocation;

    public static GameManager Instance { get; private set; }
    public void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        savedLocation = FindObjectOfType<SavePoint>();
    }

    public void RespawnPlayer()
    {
        if(savedLocation != null)
        {
            if(savedLocation.interacted)
            {
                savePoint = savedLocation.transform.position;
            }
            else
            {
                savePoint = respawnPoint;
            }
        }
        else
        {
            savePoint = respawnPoint;
        }
        PlayerController.Instance.transform.position = savePoint;
        
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}
