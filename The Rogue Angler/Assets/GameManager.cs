using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    
    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;

    [SerializeField] SavePoint savedLocation;

    public static GameManager Instance { get; private set; }
    public void Awake()
    {
        SaveData.Instance.Initialize();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        SaveScene();

        DontDestroyOnLoad(gameObject);
        savedLocation = FindObjectOfType<SavePoint>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
        Debug.Log("saved" + currentSceneName);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadSave();
        if(SaveData.Instance.saveSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.saveSceneName);
        }
        if(SaveData.Instance.savePos != null)
        {
            respawnPoint = SaveData.Instance.savePos;
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;
        
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}
