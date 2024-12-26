using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject[] maps;

    SavePoint savePoint;
    // Start is called before the first frame update
    private void OnEnable()
    {
        savePoint = FindObjectOfType<SavePoint>();
        if(savePoint != null)
        {
            if(savePoint.interacted)
            {
                UpdateMap();
            }
        }
        else
        {
            Debug.LogWarning("Nenhum SavePoint encontrado na cena.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateMap()
    {
        var savedScenes = SaveData.Instance.sceneNames;

        for(int i = 0; i < maps.Length; i++)
        {
            Debug.Log("Entrou no mapa: " + string.Join(", ", savedScenes));
            if(savedScenes.Contains("Fase" + (i + 1)))
            {
                maps[i].SetActive(true);
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }
}
