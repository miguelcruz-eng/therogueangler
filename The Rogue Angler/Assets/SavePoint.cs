using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    public bool inRange;
    public bool interacted;

    private void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            SaveData.Instance.saveSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.savePos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SaveData.Instance.SaveLocation();
            SaveData.Instance.SavePlayerData();

            Debug.Log("Salvo");
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player")) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"));
        {
            inRange = false;
            interacted = false;
        }
    }
}
