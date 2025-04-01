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

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private FadeUI endScreen;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

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

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void EndGame()
    {
        endScreen.FadeUIIn(fadeTime);
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
        Debug.Log("saved" + currentSceneName);
    }

    public void Quit()
    {
        Application.Quit();
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

    public void AtivarObjetoPeloNome(string nomeDoObjeto)
    {
        Debug.Log("entrou com " + nomeDoObjeto);
        // Procura o objeto na hierarquia pelo nome
        GameObject objeto = GameObject.Find(nomeDoObjeto);

        // Verifica se o objeto foi encontrado
        if (objeto != null)
        {
            foreach (Transform filho in objeto.transform)
            {
                // Ativa o filho
                filho.gameObject.SetActive(true);
            }
        }
        else
        {
            // Se o objeto não foi encontrado, exibe um aviso
            Debug.Log("Objeto com o nome " + nomeDoObjeto + " não foi encontrado na hierarquia.");
        }
    }

    public void DesativarObjetoPeloNome(string nomeDoObjeto)
    {
        Debug.Log("entrou com " + nomeDoObjeto);
        // Procura o objeto na hierarquia pelo nome
        GameObject objeto = GameObject.Find(nomeDoObjeto);

        // Verifica se o objeto foi encontrado
        if (objeto != null)
        {
            foreach (Transform filho in objeto.transform)
            {
                // Ativa o filho
                filho.gameObject.SetActive(false);
            }
        }
        else
        {
            // Se o objeto não foi encontrado, exibe um aviso
            Debug.Log("Objeto com o nome " + nomeDoObjeto + " não foi encontrado na hierarquia.");
        }
    }
}
